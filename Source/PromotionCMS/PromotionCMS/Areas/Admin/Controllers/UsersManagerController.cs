using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PromotionCMS.Areas.Admin.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class UsersManagerController : BaseController
    {
        //thangnh 25/02/2018
        // GET: /Admin/UsersManager/
        [HasCredential(FunctionName = "VIEW_USER")]
        public ActionResult Index()
        {
            var roles = (from r in Dbcontext.CMS_ROLES select r);
            var rspList = roles.ToList();
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View(rspList);
        }
        [Route("user/get-user-list")]
        [HasCredential(FunctionName = "VIEW_USER")]
        public JsonResult Search(string userName, string status, int roleId, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = null;
            }
            if (string.IsNullOrEmpty(status))
            {
                status = null;
            }

            roleId = roleId == 0 ? -1 : roleId;
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_CMS_USERS_SEARCH(listLength, pageNo, totalRow, userName, status, roleId);
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalRow.Value,
                        RspList = rspList
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(
                    new
                    {
                        Success = false
                    });
            }
        }

        //
        // GET: /Admin/UsersManager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Admin/UsersManager/Create
        [HttpPost]
        public ActionResult ChekUserName(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    var success1 = false;
                    var checkid = (from u in Dbcontext.CMS_USERS where (u.USERNAME == userName) select u)
                        .FirstOrDefault();
                    if (checkid == null)
                        success1 = true;
                    return Json(new { success = success1, responseText = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Create");
            }
        }
        [HasCredential(FunctionName = "ADD_USER")]
        public ActionResult Create()
        {
            var roles = (from r in Dbcontext.CMS_ROLES select r);
            var rspList = roles.ToList();
            return View(rspList);
        }

        //
        // POST: /Admin/UsersManager/Create
        [HttpPost]
        [HasCredential(FunctionName = "ADD_USER")]
        public ActionResult Create(FormCollection collection, UsersManagerModel model)
        {
            try
            {
                var checkid = (from u in Dbcontext.CMS_USERS where (u.USERNAME == model.Username) select u)
                    .FirstOrDefault();
                if (checkid == null)
                {
                    var encryPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                    var rs = Dbcontext.PROC_CMS_USERS_INSERT(model.Username, model.Email, model.Firstname, model.Lastname, model.Fullname, encryPassword, DateTime.Now, model.RoleId, "1", CurrentUser.UserName);
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    if (rs < 1)
                    {
                        TempData["RspCode"] = "99";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    TempData["RspCode"] = "99";
                    return RedirectToAction("Create");
                }

            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Create");
            }
            TempData["RspCode"] = "00";
            TempData["Message"] = "Thêm mới người dùng thành công";
            return RedirectToAction("Index");
        }

        //
        // GET: /Admin/UsersManager/Edit/5
        [HasCredential(FunctionName = "EDIT_USER")]
        public ActionResult Edit(int id)
        {
            var roles = (from r in Dbcontext.CMS_ROLES select r);
            ViewBag.ListRoles = roles.ToList();
            var user = (from cmsUsers in Dbcontext.CMS_USERS where (cmsUsers.USER_ID == id) select cmsUsers)
                .FirstOrDefault();
            return View(user);
        }

        //
        // POST: /Admin/UsersManager/Edit/5
        [HttpPost]
        [HasCredential(FunctionName = "EDIT_USER")]
        public ActionResult Edit(FormCollection collection,UsersManagerModel model)
        {
            try
            {
                var rs = Dbcontext.PROC_CMS_USERS_UPDATE(model.UserId,model.Email,model.Fullname,DateTime.Now, model.RoleId,CurrentUser.UserName);
                if (rs < 1)
                {
                    TempData["RspCode"] = "99";
                    return RedirectToAction("Edit", new { userId = model.UserId });
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Edit", new { userId = model.UserId });
            }
            TempData["RspCode"] = "00";
            TempData["Message"] = "Cập nhật người dùng thành công";
            var dataLog = ser.Serialize(model);
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
            return RedirectToAction("Index");
        }

        [Route("user/update-status")]
        [HasCredential(FunctionName = "EDIT_USER")]
        public JsonResult UpdateStatus(int userId, string isLocked)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = isLocked == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_CMS_USERS_STATUS(userId, statusUpdate);
                rspCode = rs < 1 ? "99" : "00";
                var dataLog = ser.Serialize(isLocked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Người dùng cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho người dùng thành công";
                }
                else
                {
                    message = "Đã xảy ra lỗi trong quá trình cập nhật. Vui lòng thực hiện lại sau";
                }
                return Json(
                    new
                    {
                        Success = true,
                        RspCode = rspCode,
                        Message = message
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(
                    new
                    {
                        Success = false
                    });
            }
        }
        [HasCredential(FunctionName = "EDIT_PASS_USER")]
        public ActionResult ChangePass(int id)
        {
            var user = (from cmsUsers in Dbcontext.CMS_USERS where (cmsUsers.USER_ID == id) select cmsUsers)
                .FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        [HasCredential(FunctionName = "EDIT_PASS_USER")]
        public ActionResult ChangePass(FormCollection collection,UsersManagerModel model)
        {
            try
            {
                var encryPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                var rs = Dbcontext.PROC_CMS_USERS_UPDATE_PASS(model.UserId, encryPassword);
                if (rs < 1)
                {
                    TempData["RspCode"] = "99";
                    return RedirectToAction("ChangePass", new { userId = model.UserId });
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("ChangePass", new { userId = model.UserId });
            }
            TempData["RspCode"] = "00";
            TempData["Message"] = "Cập nhật password thành công";
            var dataLog = ser.Serialize(model);
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", dataLog, MyIp(), "Change Pass", DateTime.Now);
            return RedirectToAction("Index");
        }
        public ActionResult LogUser()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "USERMANAGER", "", "", MyIp(), "VIEW LOG USER", DateTime.Now);
            return View();
        }
        [Route("user/get-user-log-list")]
        [HasCredential(FunctionName = "VIEW_USER_LOG")]
        public JsonResult SearchLogUser(string userName1, string logType, string txtFromDate, string txtToDate, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(userName1))
            {
                userName1 = null;
            }
            if (string.IsNullOrEmpty(logType))
            {
                logType = null;
            }
            var fTo = !string.IsNullOrEmpty(txtFromDate)
                            ? DateTime.ParseExact(txtFromDate, "dd-MM-yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture)
                            : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture);
            var cTo = !string.IsNullOrEmpty(txtToDate)
                ? DateTime.ParseExact(txtToDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59", "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_CMS_USER_LOG_SEARCH(listLength,pageNo,totalRow,userName1,logType,fTo,cTo);
                var rspList = model.ToList();
               
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalRow.Value,
                        RspList = rspList
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(
                    new
                    {
                        Success = false
                    });
            }
        }

    }
}

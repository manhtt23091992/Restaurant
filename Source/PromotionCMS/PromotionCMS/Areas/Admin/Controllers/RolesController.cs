using PromotionCMS.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class RolesController : BaseController
    {
        //thangnh 29/07/2018
        // GET: /Admin/Roles/
        [HasCredential(FunctionName = "VIEW_ROLES")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
        [Route("roles/get-roles-list")]
        [HasCredential(FunctionName = "VIEW_ROLES")]
        public JsonResult Search(string roleName, string status, string roleKey, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                roleName = null;
            }
            if (string.IsNullOrEmpty(roleKey))
            {
                roleKey = null;
            }
            if (string.IsNullOrEmpty(status))
            {
                status = null;
            }
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_CMS_ROLES_SEARCH(listLength, pageNo, totalRow, roleKey, status, roleName);
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
        // GET: /Admin/Roles/Details/5
        [HasCredential(FunctionName = "DETAILS_ROLES")]
        public ActionResult Details(int id)
        {
            try
            {
                if (id > 0)
                {
                    var rolesId =
                        (from rolesEdit in Dbcontext.CMS_ROLES where (rolesEdit.ROLE_ID == id) select rolesEdit)
                        .FirstOrDefault();
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", id.ToString(), MyIp(), "Xem chi tiết", DateTime.Now);
                    return View(rolesId);
                }
                else
                {
                    return RedirectToAction("Details");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Details");
            }
        }
        [HttpPost]
        public ActionResult ChekUserName(string roleKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(roleKey))
                {
                    var success1 = false;
                    var checkid = (from r in Dbcontext.CMS_ROLES where (r.ROLE_KEY == roleKey) select r)
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

        [HttpPost]
        public ActionResult LoadTabRoles(int funcId)
        {
            try
            {
                var listRoles = GeRoleDataList(funcId, 0, 0);
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listRoles
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabRolesDetails(int funcId, int roleId)
        {
            try
            {
                var listRoles = GeRoleDataList(funcId, roleId, 1);
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listRoles
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabRolesEdit(int funcId, int roleId)
        {
            try
            {
                var listRoles = GeRoleDataList(funcId, roleId, 0);
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listRoles
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }

        #region LOAD ROLES

        public List<JsTreeModel> GeRoleDataList(int funcCode, int roleId, int disabled)
        {
            try
            {
                var listData = new List<JsTreeModel>();
                var listR = (from f in Dbcontext.CMS_FUNCTIONS
                             where (f.PARENT_ID == funcCode)
                             orderby f.ORD ascending
                             select f).ToList();
                if (listR.Count <= 0) return listData;
                foreach (var item in listR)
                {
                    var node = new JsTreeModel();
                    var state = new JsTreeAttribute();
                    node.text = item.FUN_NAME;
                    node.id = item.FUN_ID.ToString();
                    var isCheck = (from cmsFunRoles in Dbcontext.CMS_FUNCT_ROLE
                                   where (cmsFunRoles.FUN_ID == item.FUN_ID) && (cmsFunRoles.ROLE_ID == roleId)
                                   select cmsFunRoles).FirstOrDefault();
                    if (isCheck != null && isCheck.ID > 0)
                    {
                        state.selected = true;
                    }
                    else
                    {
                        state.selected = false;
                    }
                    state.disabled = disabled == 1;
                    var isCheckUn = (from cmsFunRoles in Dbcontext.FUNC_ROLES
                                   where (cmsFunRoles.PARENT_ID == item.FUN_ID) && (cmsFunRoles.ROLE_ID == roleId)
                                   select cmsFunRoles).Count();
                    var isCheckU1 = (from cmsFunRoles in Dbcontext.CMS_FUNCTIONS
                                     where (cmsFunRoles.PARENT_ID == item.FUN_ID) 
                                     select cmsFunRoles).Count();
                    if (isCheckUn == isCheckU1)
                    {
                        state.undetermined = false;
                    }
                    else
                    {
                        if (isCheckUn > 0)
                        {
                            state.undetermined = true;
                            state.selected = false;
                        }
                    }
                    node.state = state;
                    node.children = RecursiveFill(item.FUN_ID, roleId, disabled);
                    listData.Add(node);
                }

                return listData;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        private List<JsTreeModel> RecursiveFill(int parentId, int roleId, int disabled)
        {
            var listData = new List<JsTreeModel>();
            var listFunc = (from f in Dbcontext.CMS_FUNCTIONS where (f.PARENT_ID == parentId) orderby f.ORD ascending select f).ToList();
            if (listFunc.Count <= 0) return listData;
            foreach (var item in listFunc)
            {
                var node = new JsTreeModel();
                var state = new JsTreeAttribute();
                node.text = item.FUN_NAME;
                node.id = item.FUN_ID.ToString();
                var isCheck = (from cmsFunRoles in Dbcontext.CMS_FUNCT_ROLE
                               where (cmsFunRoles.FUN_ID == item.FUN_ID) && (cmsFunRoles.ROLE_ID == roleId)
                               select cmsFunRoles).FirstOrDefault();
                if (isCheck != null && isCheck.ID > 0)
                {
                    state.selected = true;
                }
                else
                {
                    state.selected = false;
                }
                state.disabled = disabled == 1;
                var isCheckUn = (from cmsFunRoles in Dbcontext.FUNC_ROLES
                                 where (cmsFunRoles.PARENT_ID == item.FUN_ID) && (cmsFunRoles.ROLE_ID == roleId)
                                 select cmsFunRoles).Count();
                var isCheckU1 = (from cmsFunRoles in Dbcontext.CMS_FUNCTIONS
                                 where (cmsFunRoles.PARENT_ID == item.FUN_ID)
                                 select cmsFunRoles).Count();
                if (isCheckUn == isCheckU1)
                {
                    state.undetermined = false;
                }
                else
                {
                    if (isCheckUn > 0)
                    {
                        state.undetermined = true;
                        state.selected = false;
                    }
                }
                node.state = state;
                node.children = RecursiveFill(item.FUN_ID, roleId, disabled);
                listData.Add(node);
                RecursiveFill(item.FUN_ID, roleId, disabled);
            }
            return listData;
        }

        #endregion

        #region delete
        [Route("roles/delete")]
        [HasCredential(FunctionName = "DELETE_ROLES")]
        public JsonResult Delete(string idRoles)
        {
            try
            {
                var success1 = false;
                var i = 0;
                if (!string.IsNullOrEmpty(idRoles))
                {
                    i = int.Parse(idRoles);
                }
                var checkdelete = (from user in Dbcontext.CMS_USERS join roles in Dbcontext.CMS_ROLES on user.ROLE_ID equals roles.ROLE_ID where (user.ROLE_ID == i) select user).FirstOrDefault();
                if (checkdelete != null && checkdelete.ROLE_ID > 0)
                {
                    success1 = true;
                }
                else
                {
                    Dbcontext.PROC_CMS_FUNCT_ROLE_DELETEBYID(i);
                    Dbcontext.PROC_CMS_ROLES_DELETEBYID(i);
                }
                var dataLog = ser.Serialize(idRoles);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", dataLog, MyIp(), "Xóa", DateTime.Now);
                return Json(new { success = success1, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        [Route("roles/deleteall")]
        [HasCredential(FunctionName = "DELETE_ROLES")]
        public JsonResult DeleteAll(string idRole)
        {
            try
            {
                var i = 0;
                if (!string.IsNullOrEmpty(idRole))
                {
                    i = int.Parse(idRole);
                }
                Dbcontext.PROC_CMS_USERS_DELETEBYROLESID(i);
                Dbcontext.PROC_CMS_FUNCT_ROLE_DELETEBYID(i);
                Dbcontext.PROC_CMS_ROLES_DELETEBYID(i);
                var dataLog = ser.Serialize(idRole);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", dataLog, MyIp(), "Xóa", DateTime.Now);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        //
        // GET: /Admin/Roles/Create
        [HasCredential(FunctionName = "ADD_ROLES")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Roles/Create
        [HttpPost]
        [HasCredential(FunctionName = "ADD_ROLES")]
        public ActionResult Create(FormCollection collection, string RoleKey, string RoleName, string RoleNote, IEnumerable<int> funcId)
        {
            try
            {
                if (funcId == null)
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Bạn chưa chọn nhóm quyền";
                    return RedirectToAction("Create");
                }
                var checkid = (from r in Dbcontext.CMS_ROLES where (r.ROLE_KEY == RoleKey) select r)
                    .FirstOrDefault();
                if (checkid == null)
                {
                    Dbcontext.PROC_CMS_ROLES_INSERT(RoleName, RoleKey, RoleNote, 1, CurrentUser.UserName, DateTime.Now);
                    var roleid =
                        (from r1 in Dbcontext.CMS_ROLES where (r1.ROLE_KEY == RoleKey) select r1.ROLE_ID)
                            .FirstOrDefault();
                    if (roleid <= 0) return RedirectToAction("Index");
                    foreach (var item in funcId)
                    {
                        Dbcontext.PROC_CMS_FUNCT_ROLE_INSERT(item, roleid, "1");
                    }
                    var dataLog = ser.Serialize(RoleName);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return Json(new RolesResult
                    {
                        Success = true,
                        Message = "",
                        ReturnUrl = Url.Action("Index")
                    });
                }
                else
                {
                    Log.ErrorFormat("Error:Role Key duplicate ");
                    return Json(new RolesResult
                    {
                        Success = false,
                        Message = "Mã nhóm quyền đã tồn tại",
                        ReturnUrl = ""
                    });
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new RolesResult
                {
                    Success = false,
                    Message = "Kết nối bị gián đoạn. Vui lòng thử lại",
                    ReturnUrl = ""
                });
            }
        }

        //
        // GET: /Admin/Roles/Edit/5
        [HasCredential(FunctionName = "EDIT_ROLES")]
        public ActionResult Edit(int id)
        {
            try
            {
                if (id > 0)
                {
                    var rolesId =
                        (from rolesEdit in Dbcontext.CMS_ROLES where (rolesEdit.ROLE_ID == id) select rolesEdit)
                        .FirstOrDefault();
                    return View(rolesId);
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Edit");
            }
        }

        //
        // POST: /Admin/Roles/Edit/5
        [HttpPost]
        [HasCredential(FunctionName = "EDIT_ROLES")]
        public ActionResult Edit(string RoleKey, string RoleName, string Status, string RoleNote, int RolesID, IEnumerable<int> funcId)
        {
            try
            {
                if (funcId == null)
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Bạn chưa chọn nhóm quyền";
                    return RedirectToAction("Edit");
                }
                var checkid = (from r in Dbcontext.CMS_ROLES where (r.ROLE_KEY == RoleKey) select r)
                    .FirstOrDefault();
                if (checkid != null)
                {
                    Dbcontext.PROC_CMS_ROLES_UPDATE(RolesID, RoleName, RoleKey, RoleNote, Status, CurrentUser.UserName);
                    Dbcontext.PROC_CMS_FUNCT_ROLE_DELETEBYID(RolesID);
                    foreach (var item in funcId)
                    {
                        Dbcontext.PROC_CMS_FUNCT_ROLE_INSERT(item, RolesID, "1");
                    }
                    var user = Dbcontext.CMS_USERS.Single(x => x.USERNAME == CurrentUser.UserName);
                    var data = (from a in Dbcontext.CMS_FUNCT_ROLE
                                join b in Dbcontext.CMS_ROLES on a.ROLE_ID equals b.ROLE_ID
                                join c in Dbcontext.CMS_FUNCTIONS on a.FUN_ID equals c.FUN_ID
                                where b.ROLE_ID == user.ROLE_ID
                                select new
                                {
                                    FunctionName = c.FUN_KEY,
                                    RoleID = a.ROLE_ID
                                }).AsEnumerable().Select(x => new Credential()
                                {
                                    FunctionName = x.FunctionName,
                                    RoleID = x.RoleID
                                });
                    Session.Add(CommonConstants.SESSION_CREDENTIALS, data.Select(x => x.FunctionName).ToList());
                    var dataLog = ser.Serialize(RoleName);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return Json(new RolesResult
                    {
                        Success = true,
                        Message = "",
                        ReturnUrl = Url.Action("Index")
                    });
                }
                else
                {
                    Log.ErrorFormat("Error:Role Key duplicate ");
                    return Json(new RolesResult
                    {
                        Success = false,
                        Message = "Mã nhóm quyền không tồn tại",
                        ReturnUrl = ""
                    });
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new RolesResult
                {
                    Success = false,
                    Message = "Kết nối bị gián đoạn. Vui lòng thử lại",
                    ReturnUrl = ""
                });
            }
        }

        #region UpdateStatus
        [Route("roles/update-status")]
        [HasCredential(FunctionName = "EDIT_ROLES")]
        public JsonResult UpdateStatus(int rolesId, string isLocked)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = isLocked == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_CMS_ROLES_UPDATE_STATUS(rolesId, statusUpdate);
                var dataLog = ser.Serialize(isLocked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ROLES", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Nhóm quyền đã cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho nhóm quyền thành công";
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

        #endregion
    }
}

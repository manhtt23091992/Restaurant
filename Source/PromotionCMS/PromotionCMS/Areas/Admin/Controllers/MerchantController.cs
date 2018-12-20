using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class MerchantController : BaseController
    {
        //thangnh 02/07/2018
        // GET: /Admin/Merchant/
        [HasCredential(FunctionName = "VIEW_MERCHANT")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }

        //
        // GET: /Admin/Merchant/Details/5
        [Route("merchant/get-merchant")]
        [HasCredential(FunctionName = "VIEW_MERCHANT")]
        public JsonResult Search(string merchantStatus, string merchantName, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(merchantStatus))
            {
                merchantStatus = null;
            }
            if (String.IsNullOrEmpty(merchantName))
            {
                merchantName = null;
            }
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_MERCHANT_SEARCH(listLength, pageNo, totalRow, merchantName, merchantStatus);
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalRow.Value,
                        ListData = rspList
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

        [HasCredential(FunctionName = "PRIORITY_MERCHANT")]
        public ActionResult Priority()
        {
            try
            {
                ViewBag.noprioritize = (from bank in Dbcontext.MERCHANT_CATEGORY where (bank.PRIORITIZE == "0") && (bank.BANNER_STATUS == "1") select bank).ToList();
                ViewBag.prioritize = (from bank in Dbcontext.MERCHANT_CATEGORY where (bank.PRIORITIZE == "1") && (bank.BANNER_STATUS == "1") orderby bank.ORD ascending select bank).ToList();
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", "", MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return View();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return View();
            }
        }
          [HasCredential(FunctionName = "PRIORITY_MERCHANT")]
        public ActionResult MerchantCheck(IEnumerable<string> bankchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bankchecked)
                {
                    var i = int.Parse(item);
                    var o = 0;
                    var checkitem = (from bank in Dbcontext.MERCHANT_CATEGORY where (bank.PRIORITIZE == "0") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        var Orl = (from bank in Dbcontext.MERCHANT_CATEGORY orderby bank.ORD descending select bank.ORD).Take(1).FirstOrDefault();
                        if (!string.IsNullOrEmpty(Orl.ToString()))
                        {
                            o = int.Parse(Orl.ToString()) + 1;
                        }
                        Dbcontext.PROC_BANNER_UPDATEBYID(i, "1", o);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }

                }
                var dataLog = ser.Serialize(bankchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
          [HasCredential(FunctionName = "PRIORITY_MERCHANT")]
        public ActionResult MerchantCheckNoPrioritize(IEnumerable<string> bankchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bankchecked)
                {
                    var i = int.Parse(item);
                    var checkitemno = (from bank in Dbcontext.MERCHANT_CATEGORY where (bank.PRIORITIZE == "1") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
                    if (checkitemno != null && checkitemno.BANNER_ID > 0)
                    {
                        Dbcontext.PROC_BANNER_UPDATEBYID(i, "0", 0);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }
                }
                var dataLog = ser.Serialize(bankchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ko ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_MERCHANT")]
        public ActionResult Priority(FormCollection collection)
        {

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_MERCHANT")]
        public JsonResult PriorityUpdateOrd(IEnumerable<MerchantPriority> tmPriority)
        {
            try
            {
                if(tmPriority==null)
                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                var enumerable = tmPriority.ToList();
                foreach (var itemid in enumerable)
                {
                    var i = int.Parse(itemid.Id.ToString());
                    var v = int.Parse(itemid.Ord.ToString());
                    var checkitem =
                        (from bank in Dbcontext.MERCHANT_CATEGORY
                         where (bank.BANNER_ID == i)
                         select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        Dbcontext.PROC_BANNER_UPDATEORDBYID(i, v);
                    }
                }
                var dataLog = ser.Serialize(tmPriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = false, responseText = "Bạn chưa click chọn đối tác liên kết" }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // GET: /Admin/Merchant/Create
          [HasCredential(FunctionName = "ADD_MERCHANT")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Merchant/Create
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_MERCHANT")]
        public ActionResult Create(FormCollection collection, MerchantModel model)
        {
            try
            {
                  var check =
                    (from b in Dbcontext.MERCHANT_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) select b)
                        .FirstOrDefault();
                if (check == null)
                {
                    var newGuid = Guid.NewGuid();
                    var guidbank = ConfigurationManager.AppSettings["guid_merchant"];
                    Dbcontext.PROC_BANNER_INSERT(model.BannerTitle, null, null, model.ImageUrl, model.Link,
                        model.LinkAndroid, model.LinkIos, "1", CurrentUser.UserName, newGuid, model.Ord);
                    Dbcontext.PROC_RELATION_INSERT("MERCHANT_CATE", newGuid, Guid.Parse(guidbank), "1");
                    TempData["RspCode"] = "00";
                    TempData["Message"] = "Thêm mới đối tác liên kết thành công";
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Đối tác đã tồn tại";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Create");
            }
        }

        //
        // GET: /Admin/Merchant/Edit/5
         [HasCredential(FunctionName = "EDIT_MERCHANT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from bank in Dbcontext.MERCHANT_CATEGORY where bank.BANNER_ID == id select bank).FirstOrDefault();
                if (model != null && model.BANNER_ID > 0)
                {
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Admin/Merchant/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_MERCHANT")]
        public ActionResult Edit(FormCollection collection, BankModel model)
        {
            try
            {
                 var check =
                   (from b in Dbcontext.MERCHANT_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) && (b.BANNER_ID == model.Bannerid) select b)
                       .FirstOrDefault();
                if (check != null)
                {
                Dbcontext.PROC_BANNER_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.BannerKeyword, model.ImageUrl, model.Link, model.LinkAndroid, model.LinkIos, CurrentUser.UserName);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Cập nhật đối tác liên kết thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                return RedirectToAction("Index");
                }
                else
                {
                    var check1 =
                    (from b in Dbcontext.MERCHANT_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) select b)
                        .FirstOrDefault();
                    if (check1 == null)
                    {
                        Dbcontext.PROC_BANNER_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.BannerKeyword, model.ImageUrl, model.Link, model.LinkAndroid, model.LinkIos, CurrentUser.UserName);
                        TempData["RspCode"] = "00";
                        TempData["Message"] = "Cập nhật đối tác liên kết thành công";
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Đối tác đã tồn tại";
                        return RedirectToAction("Edit", new { id = model.Bannerid });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Edit", new { id = model.Bannerid });
            }
        }


        [Route("merchant/update-status")]
        [HasCredential(FunctionName = "EDIT_MERCHANT")]
        public JsonResult UpdateStatus(int merchantId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_BANNER_UPDATE_STATUS(merchantId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "MERCHANT", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Đối tác liên kết cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho đối tác liên kết thành công";
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

    }
}

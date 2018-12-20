using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using PromotionCMS;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;


namespace PromotionCMS.Areas.Admin.Controllers
{

    public class BankController : BaseController
    {
        //thangnh 30/06/2018
        // GET: /Admin/Bank/
        [HasCredential(FunctionName = "VIEW_BANK")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", "", MyIp(), "Tìm kiếm", DateTime.Now);
            return View();
        }

        //
        // GET: /Admin/Bank/Details/5
        [Route("bank/get-bank")]
        [HasCredential(FunctionName = "VIEW_BANK")]
        public JsonResult Search(string bankName, string bankStatus, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(bankName))
            {
                bankName = null;
            }
            if (String.IsNullOrEmpty(bankStatus))
            {
                bankStatus = null;
            }
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_BANK_SEARCH(listLength, pageNo, totalRow, bankName, bankStatus);
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

        //
        // POST: /Admin/Bank/priority
        [HasCredential(FunctionName = "PRIORITY_BANK")]
        public ActionResult Priority()
        {
            try
            {
                ViewBag.noprioritize = (from bank in Dbcontext.BANK_CATEGORY where (bank.PRIORITIZE == "0") && (bank.BANNER_STATUS == "1") select bank).ToList();
                ViewBag.prioritize = (from bank in Dbcontext.BANK_CATEGORY where (bank.PRIORITIZE == "1") && (bank.BANNER_STATUS == "1") orderby bank.ORD ascending select bank).ToList();
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", "", MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return View();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return View();
            }
        }
        [HasCredential(FunctionName = "PRIORITY_BANK")]
        public ActionResult BankCheck(IEnumerable<string> bankchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bankchecked)
                {
                    var i = int.Parse(item);
                    var o = 0;
                    var checkitem = (from bank in Dbcontext.BANK_CATEGORY where (bank.PRIORITIZE == "0") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        var Orl = (from bank in Dbcontext.BANK_CATEGORY orderby bank.ORD descending select bank.ORD).Take(1).FirstOrDefault();
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
                var bankchecked1 = ser.Serialize(bankchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", bankchecked1, MyIp(), "Phân quyền Ưu Tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [HasCredential(FunctionName = "PRIORITY_BANK")]
        public ActionResult BankCheckNoPrioritize(IEnumerable<string> bankchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bankchecked)
                {
                    var i = int.Parse(item);
                    var checkitemno = (from bank in Dbcontext.BANK_CATEGORY where (bank.PRIORITIZE == "1") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
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
                var bankchecked1 = ser.Serialize(bankchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", bankchecked1, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ko ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_BANK")]
        public ActionResult Priority(FormCollection collection)
        {

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_BANK")]
        public JsonResult PriorityUpdateOrd(IEnumerable<BankPriority> bankPriority)
        {
            try
            {
                if (bankPriority == null)
                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                var enumerable = bankPriority.ToList();
                foreach (var itemid in enumerable)
                {
                    var i = int.Parse(itemid.Id.ToString());
                    var v = int.Parse(itemid.Ord.ToString());
                    var checkitem =
                        (from bank in Dbcontext.BANK_CATEGORY
                         where (bank.BANNER_ID == i)
                         select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        Dbcontext.PROC_BANNER_UPDATEORDBYID(i, v);
                    }
                }
                var bankchecked1 = ser.Serialize(bankPriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", bankchecked1, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = false, responseText = "Bạn chưa click chọn đối tác liên kết" }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // POST: /Admin/Bank/Create
        // GET: /Admin/Bank/Create
        [HasCredential(FunctionName = "ADD_BANK")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_BANK")]
        public ActionResult Create(FormCollection collection, BankModel model)
        {
            try
            {
                var check =
                    (from b in Dbcontext.BANK_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) select b)
                        .FirstOrDefault();
                if (check == null)
                {
                    var newGuid = Guid.NewGuid();
                    var guidbank = ConfigurationManager.AppSettings["guid_bank"];
                    Dbcontext.PROC_BANNER_INSERT(model.BannerTitle, null, null, model.ImageUrl, model.Link,
                        model.LinkAndroid, model.LinkIos, "1", CurrentUser.UserName, newGuid, model.Ord);
                    Dbcontext.PROC_RELATION_INSERT("BANK_CATE", newGuid, Guid.Parse(guidbank), "1");
                    TempData["RspCode"] = "00";
                    TempData["Message"] = "Thêm mới ngân hàng thành công";
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Ngân hàng đã tồn tại";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Create");
            }
        }
        [Route("bank/update-status")]
        [HasCredential(FunctionName = "EDIT_BANK")]
        public JsonResult UpdateStatus(int bankId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_BANNER_UPDATE_STATUS(bankId, statusUpdate);
                var dataLog = ser.Serialize(bankId);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Ngân hàng đã cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho ngân hàng thành công";
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

        //
        // GET: /Admin/Bank/Edit/5
        [HasCredential(FunctionName = "EDIT_BANK")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from bank in Dbcontext.BANK_CATEGORY where bank.BANNER_ID == id select bank).FirstOrDefault();
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
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Edit");
            }
        }

        //
        // POST: /Admin/Bank/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_BANK")]
        public ActionResult Edit(FormCollection collection, BankModel model)
        {
            try
            {
                var check =
                   (from b in Dbcontext.BANK_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) && (b.BANNER_ID == model.Bannerid) select b)
                       .FirstOrDefault();
                if (check != null)
                {
                    Dbcontext.PROC_BANNER_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.BannerKeyword, model.ImageUrl, model.Link, model.LinkAndroid, model.LinkIos, CurrentUser.UserName);
                    TempData["RspCode"] = "00";
                    TempData["Message"] = "Cập nhật ngân hàng thành công";
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                    return RedirectToAction("Index");
                }
                else
                {
                    var check1 =
                    (from b in Dbcontext.BANK_CATEGORY where (b.BANNER_TITLE == model.BannerTitle) select b)
                        .FirstOrDefault();
                    if (check1 == null)
                    {
                        Dbcontext.PROC_BANNER_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.BannerKeyword, model.ImageUrl, model.Link, model.LinkAndroid, model.LinkIos, CurrentUser.UserName);
                        TempData["RspCode"] = "00";
                        TempData["Message"] = "Cập nhật ngân hàng thành công";
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANK", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Ngân hàng đã tồn tại";
                        return RedirectToAction("Edit", new { id = model.Bannerid });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Edit", new { id = model.Bannerid });
            }
        }

    }
}

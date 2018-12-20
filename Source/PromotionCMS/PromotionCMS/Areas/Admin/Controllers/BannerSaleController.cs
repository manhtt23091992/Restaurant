using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using log4net;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class BannerSaleController : BaseController
    {
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Admin/BannerSale
        [HasCredential(FunctionName = "VIEW_BANNERSALE")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
        [Route("bannersale/get-bannersale")]
        [HasCredential(FunctionName = "VIEW_BANNERSALE")]
        public JsonResult Search(string BannerName, string BannerStatus, int pageNo, string tableListLength)
        {
            try
            {
                if (string.IsNullOrEmpty(BannerName))
                {
                    BannerName = null;
                }
                if (String.IsNullOrEmpty(BannerStatus))
                {
                    BannerStatus = null;
                }
                var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_BANNER_SALE_SEARCH(listLength, pageNo, totalRow, BannerName, BannerStatus);
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
                log.InfoFormat("Exception: {0}", ex);
                return Json(
                    new
                    {
                        Success = false
                    });
            }
        }

        [HasCredential(FunctionName = "ADD_BANNERSALE")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_BANNERSALE")]
        public ActionResult Create(FormCollection collection, BannerModel model)
        {
            try
            {
                var newGuid = Guid.NewGuid();
                var guidBannerSale = ConfigurationManager.AppSettings["gui_bannerSalepoint"];
                var checkBanner =(from i in Dbcontext.BANNER_SALE where (i.BANNER_TITLE == model.BannerTitle) select i)
                       .FirstOrDefault();
                if (checkBanner == null)
                {
                    Dbcontext.PROC_BANNER_SALE_INSERT(model.BannerTitle, model.BannerDescription, null, model.ImageUrl, "1",
                    CurrentUser.UserName, CurrentUser.UserName, DateTime.Now, newGuid, model.Ord);
                    Dbcontext.PROC_RELATION_INSERT("BANNER_SALE_CATE", newGuid, Guid.Parse(guidBannerSale), "1");
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return Json(
                            new
                            {
                                code = "00",
                                message = "success"

                            });
                }
                else
                {
                    return Json(
                            new
                            {
                                code = "02",
                                message = "fail"

                            });
                }
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception: {0}", ex);
                return Json(
                        new
                        {
                            code = "99",
                            message = "fail"

                        });
            }
        }
        // POST: /Admin/Bannersale/priority
        [HasCredential(FunctionName = "PRIORITY_BANNERSALE")]
        public ActionResult Priority()
        {
            try
            {
                ViewBag.noprioritize = (from bank in Dbcontext.BANNER_SALE where (bank.PRIORITIZE == "0") && (bank.BANNER_STATUS == "1") select bank).ToList();
                ViewBag.prioritize = (from bank in Dbcontext.BANNER_SALE where (bank.PRIORITIZE == "1") && (bank.BANNER_STATUS == "1") orderby bank.ORD ascending select bank).ToList();
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", "", MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return View();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return View();
            }
        }
        [HasCredential(FunctionName = "PRIORITY_BANNERSALE")]
        public ActionResult BannerCheck(IEnumerable<string> bannerchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bannerchecked)
                {
                    var i = int.Parse(item);
                    var o = 0;
                    var checkitem = (from bank in Dbcontext.BANNER_SALE where (bank.PRIORITIZE == "0") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        var Orl = (from bank in Dbcontext.BANNER_SALE orderby bank.ORD descending select bank.ORD).Take(1).FirstOrDefault();
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
                var dataLog = ser.Serialize(bannerchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [HasCredential(FunctionName = "PRIORITY_BANNERSALE")]
        public ActionResult BannerCheckNoPrioritize(IEnumerable<string> bannerchecked)
        {
            try
            {
                var success1 = false;
                foreach (var item in bannerchecked)
                {
                    var i = int.Parse(item);
                    var checkitemno = (from bank in Dbcontext.BANNER_SALE where (bank.PRIORITIZE == "1") && (bank.BANNER_ID == i) select bank).FirstOrDefault();
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
                var dataLog = ser.Serialize(bannerchecked);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ko ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_BANNERSALE")]
        public ActionResult Priority(FormCollection collection)
        {

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "PRIORITY_BANNERSALE")]
        public JsonResult PriorityUpdateOrd(IEnumerable<BannerPriority> BannerPriority)
        {
            try
            {
                if (BannerPriority == null)
                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                var enumerable = BannerPriority.ToList();
                foreach (var itemid in enumerable)
                {
                    var i = int.Parse(itemid.Id.ToString());
                    var v = int.Parse(itemid.Ord.ToString());
                    var checkitem =
                        (from bank in Dbcontext.BANNER_SALE
                         where (bank.BANNER_ID == i)
                         select bank).FirstOrDefault();
                    if (checkitem != null && checkitem.BANNER_ID > 0)
                    {
                        Dbcontext.PROC_BANNER_SALE_UPDATE_ORD_BYID(i, v);
                    }
                }
                var dataLog = ser.Serialize(BannerPriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = false, responseText = "Bạn chưa click chọn đối tác liên kết" }, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("bannersale/update-status")]
        [HasCredential(FunctionName = "EDIT_BANNERSALE")]
        public JsonResult UpdateStatus(int bannerId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_BANNER_UPDATE_STATUS(bannerId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                return Json(
                    new
                    {
                        Success = true,
                        RspCode = rspCode
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
        [HasCredential(FunctionName = "EDIT_BANNERSALE")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from p in Dbcontext.BANNER_SALE where p.BANNER_ID == id select p).FirstOrDefault();
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
        // POST: /Admin/Banner/Edit/5

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_BANNERSALE")]
        public ActionResult Edit(FormCollection collection, BannerModel model)
        {
            try
            {
                var checkBannerTitle = (from banner in Dbcontext.BANNER_SALE where (banner.BANNER_TITLE == model.BannerTitle && banner.BANNER_ID == model.Bannerid) select banner).FirstOrDefault();
                if (checkBannerTitle != null)
                {
                    Dbcontext.PROC_BANNER_SALE_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.ImageUrl, CurrentUser.UserName);
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                    // update success
                    return Json(
                    new
                    {
                        code = "00",
                        message = "success"

                    });
                }
                else
                {
                    var checkBn = (from bn in Dbcontext.BANNER_SALE where (bn.BANNER_TITLE == model.BannerTitle) select bn).FirstOrDefault();
                    if (checkBn == null)
                    {
                        Dbcontext.PROC_BANNER_SALE_UPDATE(model.Bannerid, model.BannerTitle, model.BannerDescription, model.ImageUrl, CurrentUser.UserName);
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                        // update success
                        return Json(
                        new
                        {
                            code = "00",
                            message = "success"

                        });
                    }
                    else
                    {
                        // BannerTitlet đã tồn tại
                            return Json(
                        new
                        {
                            code = "02",
                            message = "fail"

                        });
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
        // GET: /Admin/Banner/Details/
        [HasCredential(FunctionName = "DETAILS_BANNERSALE")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = (from p in Dbcontext.BANNER_SALE where p.BANNER_ID == id select p).FirstOrDefault();
                if (model != null)
                {
                    var dataLog = ser.Serialize("");
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_SALE", "", dataLog, MyIp(), "Xem chi tiết", DateTime.Now);
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Index");
            }
        }
    }
}
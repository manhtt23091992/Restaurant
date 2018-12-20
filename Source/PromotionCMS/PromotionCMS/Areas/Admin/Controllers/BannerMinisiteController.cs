using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PromotionCMS;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;
using log4net;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class BannerMinisiteController : BaseController
    {
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DateTime getDate;
        // GET: Admin/BannerMinisite
        [HasCredential(FunctionName = "VIEW_MINISITE")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_MINISITE", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }

        [Route("banner/get-banner")]
        [HasCredential(FunctionName = "VIEW_MINISITE")]
        public JsonResult Search(string BannerName, string BannerStatus, int pageNo, string tableListLength, string txtFromDate, string txtToDate)
        {
            if (string.IsNullOrEmpty(BannerName))
            {
                BannerName = null;
            }
            if (String.IsNullOrEmpty(BannerStatus))
            {
                BannerStatus = null;
            }
            if (string.IsNullOrEmpty(txtFromDate))
            {
                getDate = DateTime.ParseExact(DateTime.Now.ToString("01-MM-yyyy") + " 00:00:00",
                        "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                getDate = !string.IsNullOrEmpty(txtFromDate) ? DateTime.ParseExact(txtFromDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture) : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            }

            try
            {
                var eDate = !string.IsNullOrEmpty(txtToDate)
               ? DateTime.ParseExact(txtToDate, "dd-MM-yyyy HH:mm:ss",
                   CultureInfo.InvariantCulture)
               : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59", "dd-MM-yyyy HH:mm:ss",
                   CultureInfo.InvariantCulture);

                var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_BANNER_MINI_SEARCH(listLength, pageNo, totalRow, BannerName, BannerStatus, getDate, eDate);
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
        [HasCredential(FunctionName = "ADD_MINISITE")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_MINISITE")]
        public ActionResult Create(FormCollection collection, BannerModel model, string StartDate, string ToDate)
        {
            try
            {
                var eDate = !string.IsNullOrEmpty(ToDate)
                ? DateTime.ParseExact(ToDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);

                var fDate = !string.IsNullOrEmpty(StartDate)
                ? DateTime.ParseExact(StartDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
                var newGuid = Guid.NewGuid();
                var guidBannerMini = ConfigurationManager.AppSettings["gui_bannerMinisite"];

                if (fDate > eDate)
                {
                    return Json(
                   new
                   {
                       code = "04",
                       message = "fail"

                   });
                }
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var rs = Dbcontext.PROC_BANNER_MINI_SEARCH(10, 1, totalRow, model.BannerTitle, null, null, null);
                var rp = Dbcontext.PROC_BANNER_MINI_SEARCH(10, 1, totalRow, null, null, null, null).ToList();
                var checkBannerTitle = rs.ToList();

                if (checkBannerTitle.Count == 0)
                {
                    if (rp.Count > 0)
                    {
                        Dbcontext.PROC_BANNER_MINI_UPDATE_ALL("0");
                    }
                    else
                    {
                        Dbcontext.PROC_BANNER_MINISITE_INSERT(model.BannerTitle, model.ImageUrl, model.Link, "1", CurrentUser.UserName, DateTime.Now, CurrentUser.UserName, DateTime.Now, fDate, eDate, newGuid);
                        Dbcontext.PROC_RELATION_INSERT("BANNER_MINISITE_CATE", newGuid, Guid.Parse(guidBannerMini), "1");
                        // insert success
                        return Json(
                        new
                        {
                            code = "00",
                            message = "success"

                        });
                    }
                    Dbcontext.PROC_BANNER_MINISITE_INSERT(model.BannerTitle, model.ImageUrl, model.Link, "1", CurrentUser.UserName, DateTime.Now, CurrentUser.UserName, DateTime.Now, fDate, eDate, newGuid);
                    Dbcontext.PROC_RELATION_INSERT("BANNER_MINISITE_CATE", newGuid, Guid.Parse(guidBannerMini), "1");
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_MINISITE", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    // insert success
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
        [Route("banner/update-status")]
        [HasCredential(FunctionName = "EDIT_MINISITE")]
        public JsonResult UpdateStatus(int bannerId, string status)
        {
            try
            {
                    var statusUpdate = status == "1" ? "0" : "1";
                    var rs = Dbcontext.PROC_BANNER_UPDATE_STATUS(bannerId, statusUpdate);
                    var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                    var checkStatus = Dbcontext.PROC_BANNER_MINI_SEARCH(20, 1, totalRow, null, "1", null, null).ToList();
                    if (checkStatus.Count == 0)
                    {
                        Dbcontext.PROC_BANNER_UPDATE_STATUS(bannerId, "1");
                        var dataLog = ser.Serialize(status);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_MINISITE", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                    return Json(
                       new
                       {
                           Success = false,
                           code = "01"
                       });

                    }
                    return Json(
                        new
                        {
                            Success = true,
                            code = "00"
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

        [HasCredential(FunctionName = "EDIT_MINISITE")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from banner in Dbcontext.BANNER_MINISITE where banner.BANNER_ID == id select banner).FirstOrDefault();
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
                log.InfoFormat("Exception: {0}", ex);
                TempData["RspCode"] = "99";
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Admin/Bank/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_MINISITE")]
        public ActionResult Edit(FormCollection collection, BannerModel model, string StartDate, string ToDate)
        {
            try
            {
                var eDate = !string.IsNullOrEmpty(ToDate)
                ? DateTime.ParseExact(ToDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);

                var fDate = !string.IsNullOrEmpty(StartDate)
                ? DateTime.ParseExact(StartDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
                
                if (fDate > eDate)
                {
                    return Json(
                   new
                   {
                       code = "04",
                       message = "fail"

                   });
                }
                var checkBannerTitle = (from banner in Dbcontext.BANNER_MINISITE where (banner.BANNER_TITLE == model.BannerTitle && banner.BANNER_ID == model.Bannerid) select banner).FirstOrDefault();
                if (checkBannerTitle != null)
                {
                    Dbcontext.PROC_BANNER_MINI_UPDATE(model.Bannerid, model.BannerTitle, model.ImageUrl, model.Link, fDate, eDate, CurrentUser.UserName);
                    var dataLog = ser.Serialize(model);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_MINISITE", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
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
                    var checkBn =(from bn in Dbcontext.BANNER_MINISITE where (bn.BANNER_TITLE == model.BannerTitle) select bn).FirstOrDefault();
                    if (checkBn == null)
                    {
                        Dbcontext.PROC_BANNER_MINI_UPDATE(model.Bannerid, model.BannerTitle, model.ImageUrl, model.Link, fDate, eDate, CurrentUser.UserName);
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "BANNER_MINISITE", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
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
                log.InfoFormat("Exception: {0}", ex);
                TempData["RspCode"] = "99";
                return RedirectToAction("Edit", new { id = model.Bannerid });
            }
        }
    }
}
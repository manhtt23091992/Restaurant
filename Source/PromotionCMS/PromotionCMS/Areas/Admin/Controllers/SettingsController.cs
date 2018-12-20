using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;
using log4net;
namespace PromotionCMS.Areas.Admin.Controllers
{
    public class SettingsController : BaseController
    {
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HasCredential(FunctionName = "VIEW_SETTINGS")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SETTINGS", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
        [Route("settings/get-settings")]
        [HasCredential(FunctionName = "VIEW_SETTINGS")]
        public JsonResult SettingsSearch(string SetCode, string SetKey, string SetStatus, string tableListLength, int pageNo)
        {
            if (String.IsNullOrEmpty(SetCode))
            {
                SetCode = null;
            }
            if (String.IsNullOrEmpty(SetKey))
            {
                SetKey = null;
            }
            if (String.IsNullOrEmpty(SetStatus))
            {
                SetStatus = null;
            }
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_SETTINGS_SEARCH(listLength, pageNo, totalRow, SetCode, SetKey, SetStatus);
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
        [HasCredential(FunctionName = "ADD_SETTINGS")]
        public ActionResult SettingsCreate()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_SETTINGS")]
        public JsonResult SettingsCreate(FormCollection conllection, SettingsModel model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.SetCode))
                {
                    model.SetCode = null;
                }
                if (String.IsNullOrEmpty(model.SetKey))
                {
                    model.SetKey = null;
                }
                if (String.IsNullOrEmpty(model.SetValue))
                {
                    model.SetValue = null;
                }
                if (String.IsNullOrEmpty(model.SetNote))
                {
                    model.SetNote = null;
                }
                if (model.SetCode != null && model.SetKey != null && model.SetValue != null && model.SetNote != null)
                {
                    var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                    var checkCode = Dbcontext.PROC_SETTINGS_SEARCH(10, 1, totalRow, model.SetCode, null, null).ToList();
                    if (checkCode.Count == 0)
                    {
                        var rs = Dbcontext.PROC_SETTINGS_INSERT(model.SetCode, model.SetKey, model.SetValue, model.SetNote, "1");
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SETTINGS", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                        if (rs > 0)
                        {
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
                            // insert fail
                            return Json(
                            new
                            {
                                code = "01",
                                message = "fail"

                            });
                        }
                    }
                    else
                    {
                        // Code already
                        return Json(
                            new
                            {
                                code = "02",
                                message = "fail"

                            });
                    }
                }
                else
                {
                    // model null
                    return Json(
                             new
                             {
                                 code = "03",
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
        [HasCredential(FunctionName = "EDIT_SETTINGS")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = Dbcontext.PROC_SETTINGS_SELECT_BYID(id).FirstOrDefault();
                if (model.ST_ID > 0 && model != null)
                {
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    return View();
                }
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception: {0}", ex);
                return RedirectToAction("Index");
            }
        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_SETTINGS")]
        public ActionResult Edit(FormCollection collection, SettingsModel model)
        {
            try
            {
                Dbcontext.PROC_SETTINGS_UPDATE(model.SetID, model.SetKey, model.SetValue, model.SetNote);
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SETTINGS", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                // update success
                return Json(
                new
                {
                    code = "00",
                    message = "success"

                });
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception :{0}", ex);
                return Json(
                new
                {
                    code = "99",
                    message = "success"

                });
            }
        }

        [Route("settings/update-status")]
        [HasCredential(FunctionName = "EDIT_SETTINGS")]
        public JsonResult UpdateStatus(int stId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_SETTINGS_UPDATE_STATUS(stId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SETTINGS", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                return Json(
                new
                {
                    Success = true,
                    RspCode = "00"
                });
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception: {0}", ex);
                return Json(
                  new
                  {
                      Success = false,
                      RspCode = "99"
                  });
            }
        }

        [HasCredential(FunctionName = "DETAILS_BANNERSALE")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = (from p in Dbcontext.SETTINGS where p.ST_ID == id select p).FirstOrDefault();
                if (model != null)
                {
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SETTINGS", "", id.ToString(), MyIp(), "Xem chi tiết", DateTime.Now);
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
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class TradeMarkController : BaseController
    {
        private int _level = -1;
        StringBuilder _sb = new StringBuilder();
        readonly List<SelectListItem> _items = new List<SelectListItem>();
        //thangnh 01/07/2018
        // GET: /Admin/TradeMark/
         [HasCredential(FunctionName = "VIEW_TRADEMART")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
          [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public ActionResult Priority()
        {
            return View();
        }
        [Route("trademart/update-priority-ord")]
        [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public JsonResult PriorityUpdateOrd(IEnumerable<TradeMarkPriority> tmPriority)
        {
            try
            {
                if(tmPriority==null)
                    return Json(new { success = true, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                var enumerable = tmPriority.ToList();
                foreach (var itemid in enumerable)
                {
                    var i = int.Parse(itemid.Id.ToString());
                    var v = int.Parse(itemid.Ord.ToString());
                    var checkitem =
                        (from tmark in Dbcontext.TRADE_MARK
                         where (tmark.TM_ID == i)
                         select tmark).FirstOrDefault();
                    if (checkitem != null && checkitem.TM_ID > 0)
                    {
                        Dbcontext.PROC_TRADE_MARK_UPDATEORDBYID(i, v);
                    }
                }
                var dataLog = ser.Serialize(tmPriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = true, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return Json(new { success = false, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("trademark/update-priority")]
        [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public ActionResult UpdatePriority(IEnumerable<string> tmpriority)
        {
            try
            {
                var success1 = false;
                foreach (var item in tmpriority)
                {
                    var i = int.Parse(item);
                    var o = 0;
                    var checkitem = (from tm in Dbcontext.TRADE_MARK where (tm.TM_STATUS == "0") && (tm.TM_ID == i) select tm).FirstOrDefault();
                    if (checkitem != null && checkitem.TM_ID > 0)
                    {
                        var Orl = (from tm1 in Dbcontext.TRADE_MARK orderby tm1.ORD descending select tm1.ORD).Take(1).FirstOrDefault();
                        if (!string.IsNullOrEmpty(Orl.ToString()))
                        {
                            o = int.Parse(Orl.ToString()) + 1;
                        }
                        Dbcontext.PROC_TRADE_MARK_UPDATEBYID(i, "1", o);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }

                }
                var dataLog = ser.Serialize(tmpriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [Route("trademark/update-nopriority")]
        [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public ActionResult UpdateNoPriority(IEnumerable<string> tmnopriority)
        {
            try
            {
                var success1 = false;
                foreach (var item in tmnopriority)
                {
                    var i = int.Parse(item);
                    var checkitemno = (from tradeMark in Dbcontext.TRADE_MARK where (tradeMark.TM_STATUS == "1") && (tradeMark.TM_ID == i) select tradeMark).FirstOrDefault();
                    if (checkitemno != null && checkitemno.TM_ID > 0)
                    {
                        Dbcontext.PROC_TRADE_MARK_UPDATEBYID(i, "0", 0);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }
                }
                var dataLog = ser.Serialize(tmnopriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ko ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [Route("trademark/get-NoPriotity")]
        [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public JsonResult GetNoPriotity()
        {
            try
            {
                var model = (from pri in Dbcontext.TRADE_MARK where (pri.TM_STATUS == "0") select pri).ToList();
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
                        ListData1 = rspList
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
        [Route("trademark/get-Priotity")]
        [HasCredential(FunctionName = "PRIORITY_TRADEMART")]
        public JsonResult GetPriotity()
        {
            try
            {
                var model = (from pri in Dbcontext.TRADE_MARK where (pri.TM_STATUS == "1") orderby pri.ORD ascending select pri).ToList();
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
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

        #region Delete
        //
        // GET: /Admin/TradeMark/Delete/5
         [HasCredential(FunctionName = "DELETE_TRADEMART")]
        public ActionResult Delete()
        {
            return View();

        }

        //
        // POST: /Admin/TradeMark/Delete/5
        [HttpPost]
        [HasCredential(FunctionName = "DELETE_TRADEMART")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [Route("trademark/delete")]
        public JsonResult Delete(string idt)
        {
            try
            {
                var success1 = false;
                var i = 0;
                if (!string.IsNullOrEmpty(idt))
                {
                    i = Int32.Parse(idt.ToString());
                }
                var checkdelete = (from sp in Dbcontext.SALE_POINT join tm in Dbcontext.TRADE_MARK on sp.TM_ID equals tm.TM_ID where (sp.TM_ID == i) select tm).FirstOrDefault();

                if (checkdelete != null && checkdelete.TM_ID > 0)
                {
                    success1 = true;
                }
                else
                {
                    var checkdeleteparent = (from sp in Dbcontext.SALE_POINT join tm in Dbcontext.TRADE_MARK on sp.TM_ID equals tm.TM_ID where (tm.PARENT_ID == i) select sp).FirstOrDefault();
                    if (checkdeleteparent != null && checkdeleteparent.TM_ID > 0)
                    {
                        success1 = true;
                    }
                    else
                    {
                        Dbcontext.PROC_TRADE_MARK_DELETEBYID(i);
                        Dbcontext.PROC_TRADE_MARK_DELETEPARENTID(i);
                    }

                }
                var dataLog = ser.Serialize(idt);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Xóa", DateTime.Now);
                return Json(new { success = success1, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = false, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("trademark/deleteall")]
        public JsonResult DeleteAll(string idt)
        {
            try
            {
                var i = 0;
                if (!string.IsNullOrEmpty(idt))
                {
                    i = Int32.Parse(idt);
                }
                Dbcontext.PROC_TRADE_MARK_DELETEBYID(i);
                Dbcontext.PROC_TRADE_MARK_DELETEPARENTID(i);
                Dbcontext.PROC_SALE_POINT_DELETEBY_TMID(i);
                var dataLog = ser.Serialize(idt);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Xóa", DateTime.Now);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Search

        [Route("trademark/get-trademark")]
        [HasCredential(FunctionName = "VIEW_TRADEMART")]
        public JsonResult Search(string TmName, string Status, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(TmName))
            {
                TmName = null;
            }
            if (string.IsNullOrEmpty(Status))
            {
                Status = null;
            }
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_TRADE_MARK_SEARCH(listLength, pageNo, totalRow, TmName, Status);
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
        #endregion

        #region DropdownListParent
        public JsonResult Listparent()
        {
            var lstItem1 =
                (from item in Dbcontext.TRADE_MARK where (item.STATUS == "1") select item)
                .ToList();

            _items.Add(new SelectListItem { Text = "-- All --", Value = "", Selected = true });
            RecursiveFillTree(lstItem1, 0);
            IEnumerable<SelectListItem> ls = _items;
            return Json(new SelectList(ls, "Value", "Text"));
        }
        private void RecursiveFillTree(IEnumerable<TRADE_MARK> lstItem, int parentId)
        {
            _level += 1;
            var appender = new StringBuilder();

            for (var j = 0; j <= _level - 1; j++)
            {
                appender.Append("&nbsp;&nbsp;&nbsp;");
            }
            if (_level > 0)
            {
                appender.Append("|----");
            }
            var systemCategorys = lstItem as TRADE_MARK[] ?? lstItem.ToArray();
            var lstChild = systemCategorys.Where(p => p.PARENT_ID == parentId).ToList();
            if (lstChild.Count > 0)
            {

                for (var i = 0; i <= lstChild.Count - 1; i++)
                {
                    _items.Add(new SelectListItem { Text = appender + lstChild[i].TM_NAME, Value = lstChild[i].TM_ID.ToString() });
                    RecursiveFillTree(systemCategorys, lstChild[i].TM_ID);
                }
            }
            _level -= 1;
        }
     
        #endregion

        #region Create

        public ActionResult checkid(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var i = Int32.Parse(id);
                    var success1 = false;
                    var checkid = (from tradeMark in Dbcontext.TRADE_MARK where (tradeMark.TM_ID == i) select tradeMark)
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


        //
        // GET: /Admin/TradeMark/Create
         [HasCredential(FunctionName = "ADD_TRADEMART")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/TradeMark/Create
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_TRADEMART")]
        public ActionResult Create(FormCollection collection, TradeMarkModel model)
        {
            try
            {
                if (model.TmId != 0 && model.TmName != null)
                {
                    var checkid = (from tradeMark in Dbcontext.TRADE_MARK where (tradeMark.TM_ID == model.TmId) select tradeMark)
                        .FirstOrDefault();
                    if (checkid == null)
                    {
                        Dbcontext.PROC_TRADE_MARK_INSERT(model.TmId, model.TmName, model.ImageUrl, model.ParentId, CurrentUser.UserName);
                        TempData["RspCode"] = "00";
                        TempData["Message"] = "Thêm mới ngành hàng thành công";
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Log.ErrorFormat("Create TradeMart duplicate id:" + model.TmId);
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Ngành hàng đã tồn tại";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    Log.ErrorFormat("Create TradeMart data null");
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Dữ liệu null";
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

        #endregion

        #region Edit

        [Route("trademark/update-status")]
        [HasCredential(FunctionName = "EDIT_TRADEMART")]
        public JsonResult UpdateStatus(int tmId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_TRADE_MARK_UPDATE_STATUS(tmId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Ngành hàng cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho ngành hàng thành công";
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
        // GET: /Admin/TradeMark/Edit/5
           [HasCredential(FunctionName = "EDIT_TRADEMART")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from tm in Dbcontext.TRADE_MARK where tm.TM_ID == id select tm).FirstOrDefault();
                if (model != null && model.TM_ID > 0)
                {
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

        //
        // POST: /Admin/TradeMark/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_TRADEMART")]
        public ActionResult Edit(FormCollection collection, TradeMarkModel model)
        {
            try
            {

                Dbcontext.PROC_TRADE_MARK_UPDATE(model.TmId, model.TmName, model.ImageUrl, model.ParentId);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Cập nhật ngành hàng thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "TRADEMARK", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Edit");
            }
        }


        #endregion

    }
}

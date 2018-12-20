using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class ProgramController : BaseController
    {
        // thangnh 03/08/2018
        // GET: /Admin/Program/
        [HasCredential(FunctionName = "VIEW_PROGRAM")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "PROGRAM", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }

        [Route("program/get-program")]
        [HasCredential(FunctionName = "VIEW_PROGRAM")]
        public JsonResult Search(string pgTitle, string Status, string txtFromDate, string txtToDate, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(pgTitle))
            {
                pgTitle = null;
            }
            if (String.IsNullOrEmpty(Status))
            {
                Status = null;
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
                var model = Dbcontext.PROC_PROGRAM_SEARCH(pgTitle, Status, fTo, cTo, listLength, pageNo, totalRow);
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
        #region LOADJSTREE
        public ActionResult LoadTabTm()
        {
            try
            {
                var listTm = GeTmDataList(0, "");
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listTm
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabTmEdit(string pgId)
        {
            try
            {
                var listTm = GeTmDataList(0, pgId);
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listTm
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabTmDelete(string pgId)
        {
            try
            {
                var listTm = GeTmDataList(1, pgId);
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listTm
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabSpDelete(IEnumerable<int> tmId, string pgGuid)
        {
            try
            {
                var listSp = tmId.Select(list => GeSpDataList(list, 1, pgGuid)).SelectMany(listSp1 => listSp1).ToList();
                var result = Json(listSp);
                return new JsonResult()
                {
                    Data = result,
                    MaxJsonLength = 86753090
                };
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        [HttpPost]
        public ActionResult LoadTabSp(IEnumerable<int> tmId, string pgGuid)
        {
            try
            {
                var listSp = tmId.Select(list => GeSpDataList(list, 0, pgGuid)).SelectMany(listSp1 => listSp1).ToList();
                var result = Json(listSp);
                return new JsonResult()
                {
                    Data = result,
                    MaxJsonLength = 86753090
                };
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }

        public List<JsTreeModel> GeSpDataList(int spId, int disabled, string pgGuid)
        {
            try
            {
                var listData = new List<JsTreeModel>();
                var listR = (from f in Dbcontext.TRADE_MARK
                             where (f.TM_ID == spId) && (f.STATUS == "1")
                             select f).FirstOrDefault();
                if (listR == null) return listData;
                var node = new JsTreeModel();
                var state = new JsTreeAttribute();
                node.id = listR.TM_ID.ToString();
                node.text = listR.TM_NAME;
                if (!string.IsNullOrEmpty(pgGuid))
                {
                    var pgGuid1 = Guid.Parse(pgGuid);
                    var checkSp = (from r in Dbcontext.RELATIONs where (r.RELATION_RELATED_ID == pgGuid1) select r).Count();
                    var checkS = (from r in Dbcontext.SALE_POINT where (r.TM_ID == spId) select r).Count();
                    if (checkS == checkSp)
                    {
                        state.selected = true;
                    }
                    else
                    {
                        if (checkSp > 0)
                        {
                            state.undetermined = true;
                        }
                        else
                        {
                            state.selected = false;
                        }
                    }
                }
                state.opened = false;
                state.disabled = disabled == 1;
                node.state = state;
                node.children = GeSpDataList1(spId, disabled, pgGuid);
                listData.Add(node);
                return listData;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }
        public List<JsTreeModel> GeSpDataList1(int spId, int disabled, string pgGuid)
        {
            try
            {
                var listData = new List<JsTreeModel>();
                var listR = Dbcontext.PROC_PROGRAM_SELECTBY_TMID(spId).ToList();
                if (listR.Count <= 0) return listData;
                foreach (var item in listR)
                {
                    var node = new JsTreeModel();
                    var state = new JsTreeAttribute();
                    node.text = item.SP_NAME+" -- " + item.SP_ADDRESS + ", " + item.WARD_NAME + ", " + item.DISTRICT_NAME + ", " + item.PROVINCE_NAME; ;
                    node.id = item.SP_GUID.ToString();
                    var checkS = 0;
                    if (!string.IsNullOrEmpty(pgGuid))
                    {
                        var item1 = Guid.Parse(item.SP_GUID.ToString());
                        var pgGuid1 = Guid.Parse(pgGuid);
                        checkS = Dbcontext.PROC_PROGRAM_SELECTBY_RELATION(item1, pgGuid1).FirstOrDefault() ?? 0;
                    }
                    state.selected = checkS > 0;
                    state.disabled = disabled == 1;
                    node.state = state;
                    //var listData1 = new List<JsTreeModel>();
                    //var node1 = new JsTreeModel();
                    //var state1 = new JsTreeAttribute();
                    //var a_attr1 = new JsTreeAttr();
                    //node1.text = item.SP_ADDRESS + ", " + item.WARD_NAME + ", " + item.DISTRICT_NAME + ", " + item.PROVINCE_NAME;
                    //node1.icon = "fa fa-map-marker";
                    //state1.disabled = true;
                    //a_attr1.@class = "no_checkbox";
                    //node1.state = state1;
                    //node1.a_attr = a_attr1;
                    //listData1.Add(node1);
                    //node.children = listData1;
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

        
        public List<JsTreeModel> GeTmDataList(int disabled, string pgId)
        {
            try
            {
                var listData = new List<JsTreeModel>();
                var listR = (from f in Dbcontext.TRADE_MARK
                             where (f.STATUS == "1") && (f.PARENT_ID==0)
                             orderby f.TM_STATUS descending, f.ORD ascending
                             select f).ToList();
                if (listR.Count <= 0) return listData;
                foreach (var item in listR)
                {
                    var node = new JsTreeModel();
                    var state = new JsTreeAttribute();
                    node.text = item.TM_NAME;
                    node.id = item.TM_ID.ToString();
                    state.disabled = disabled == 1;
                    var checkS = (from r in Dbcontext.TM_PROGRAM where (r.PG_ID == pgId) && (r.TM_ID == item.TM_ID) select r.TM_ID).FirstOrDefault();
                    state.selected = checkS > 0;
                    var isCheckU = (from program in Dbcontext.TRADEMART_PROGRAM
                                    where (program.PARENT_ID == item.TM_ID) && (program.PG_ID == pgId)
                                    select program).Count();
                    var isCheckU1 = (from program in Dbcontext.TRADE_MARK
                                     where (program.PARENT_ID == item.TM_ID)
                                     select program).Count();
                    if (isCheckU == isCheckU1)
                    {
                        state.undetermined = false;
                    }
                    else
                    {
                        if (isCheckU > 0)
                        {
                            state.undetermined = true;
                            state.selected = false;
                        }
                    }
                    node.state = state;
                    node.children = RecursiveFill(item.TM_ID, disabled, pgId);
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
        private List<JsTreeModel> RecursiveFill(int parentId, int disabled, string pgId)
        {
            var listData = new List<JsTreeModel>();
            var listFunc = (from f in Dbcontext.TRADE_MARK where (f.PARENT_ID == parentId) orderby f.TM_STATUS descending, f.ORD ascending select f).ToList();
            if (listFunc.Count <= 0) return listData;
            foreach (var item in listFunc)
            {
                var node = new JsTreeModel();
                var state = new JsTreeAttribute();
                node.text = item.TM_NAME;
                node.id = item.TM_ID.ToString();
                state.disabled = disabled == 1;
                var checkS = (from r in Dbcontext.TM_PROGRAM where (r.PG_ID == pgId) && (r.TM_ID == item.TM_ID) select r.TM_ID).FirstOrDefault();
                var isCheckU = (from program in Dbcontext.TRADEMART_PROGRAM
                                where (program.PARENT_ID == item.TM_ID) && (program.PG_ID == pgId)
                                select program).Count();
                var isCheckU1 = (from program in Dbcontext.TRADE_MARK
                                 where (program.PARENT_ID == item.TM_ID)
                                 select program).Count();
                if (isCheckU == isCheckU1)
                {
                    state.undetermined = false;
                    state.selected = true;
                }
                else
                {
                    if (isCheckU > 0)
                    {
                        state.undetermined = true;
                        state.selected = false;
                    }
                }
                state.selected = checkS > 0;
                node.state = state;
                node.children = RecursiveFill(item.TM_ID, disabled, pgId);
                listData.Add(node);
                RecursiveFill(item.TM_ID, disabled, pgId);
            }
            return listData;
        }
        #endregion
        //
        // GET: /Admin/Program/Details/5
        [HasCredential(FunctionName = "DETAILS_PROGRAM")]
        public ActionResult Details(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var programId =
                        (from programEdit in Dbcontext.PROGRAMs where (programEdit.PG_ID == id) select programEdit)
                        .FirstOrDefault();
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "PROGRAM", "", id, MyIp(), "Xem chi tiết", DateTime.Now);
                    return View(programId);
                }
                else
                {
                    return RedirectToAction("Details");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Details");
            }
        }
        #region Thêm mới
        public ActionResult Checkid(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var success1 = false;
                    var checkid = (from program in Dbcontext.PROGRAMs where (program.PG_ID == id) select program)
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
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Create");
            }

        }
        //
        // GET: /Admin/Program/Create
        [HasCredential(FunctionName = "ADD_PROGRAM")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Program/Create
        [HttpPost]
        [HasCredential(FunctionName = "ADD_PROGRAM")]
        public ActionResult Create(FormCollection collection, string PgId, string PgName, string StartDate, string EndDate, IEnumerable<int> tmId, IEnumerable<string> spId)
        {
            try
            {
                if (tmId == null || spId == null)
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Bạn chưa chọn điểm bán cho chương trình";
                    return RedirectToAction("Create");
                }
                var checkid = (from program in Dbcontext.PROGRAMs where (program.PG_ID == PgId) select program)
                      .FirstOrDefault();
                if (checkid == null)
                {
                    var newGuid = Guid.NewGuid();
                    var fTo = !string.IsNullOrEmpty(StartDate)
              ? DateTime.ParseExact(StartDate, "dd-MM-yyyy HH:mm:ss",
                  CultureInfo.InvariantCulture)
              : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                  CultureInfo.InvariantCulture);
                    var cTo = !string.IsNullOrEmpty(EndDate)
               ? DateTime.ParseExact(EndDate, "dd-MM-yyyy HH:mm:ss",
                   CultureInfo.InvariantCulture)
               : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59", "dd-MM-yyyy HH:mm:ss",
                   CultureInfo.InvariantCulture);
                    Dbcontext.PROC_PROGRAM_INSERT(PgId, PgName, CurrentUser.UserName, fTo, cTo, newGuid, "0");
                    foreach (var itemTm in tmId)
                    {
                        Dbcontext.PROC_TM_PROGRAM_INSERT(PgId, itemTm);
                    }
                    foreach (var itemSp in spId)
                    {
                        if (Guid.TryParse(itemSp, out var newGuid1))
                        {
                            Dbcontext.PROC_RELATION_INSERT("SALEPOINT_PROGRAM", newGuid1, newGuid, "1");
                        }
                    }
                    var dataLog = ser.Serialize(PgName);
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "PROGRAM", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                    return Json(new RolesResult
                    {
                        Success = true,
                        Message = "",
                        ReturnUrl = Url.Action("Index")
                    });
                }
                else
                {
                    return Json(new RolesResult
                    {
                        Success = false,
                        Message = "Mã chương trình đã tồn tại",
                        ReturnUrl = ""
                    });
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

        #region SỬA
        //
        // GET: /Admin/Program/Edit/5
        [HasCredential(FunctionName = "EDIT_PROGRAM")]
        public ActionResult Edit(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var programId =
                        (from programEdit in Dbcontext.PROGRAMs where (programEdit.PG_ID == id) select programEdit)
                        .FirstOrDefault();
                    return View(programId);
                }
                else
                {
                    return RedirectToAction("Edit");
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
        // POST: /Admin/Program/Edit/5
        [HttpPost]
        [HasCredential(FunctionName = "EDIT_PROGRAM")]
        public ActionResult Edit(FormCollection collection, string PgId, string PgGuid, string PgName, string StartDate, string EndDate, IEnumerable<int> tmId, IEnumerable<string> spId)
        {
            try
            {
                if (tmId == null || spId == null)
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Bạn chưa chọn điểm bán cho chương trình";
                    return RedirectToAction("Create");
                }
                var g = Guid.Parse(PgGuid);
                var fTo = !string.IsNullOrEmpty(StartDate)
          ? DateTime.ParseExact(StartDate, "dd-MM-yyyy HH:mm:ss",
              CultureInfo.InvariantCulture)
          : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
              CultureInfo.InvariantCulture);
                var cTo = !string.IsNullOrEmpty(EndDate)
           ? DateTime.ParseExact(EndDate, "dd-MM-yyyy HH:mm:ss",
               CultureInfo.InvariantCulture)
           : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59", "dd-MM-yyyy HH:mm:ss",
               CultureInfo.InvariantCulture);
                Dbcontext.PROC_PROGRAM_UPDATE(PgId, PgName, CurrentUser.UserName, fTo, cTo);
                if (!string.IsNullOrEmpty(PgGuid))
                {
                    Dbcontext.PROC_RELATION_DELETEBYID(g);
                }
                if (!string.IsNullOrEmpty(PgId))
                {
                    Dbcontext.PROC_TM_PROGRAM_DELETEBYID(PgId);
                }
                foreach (var itemTm in tmId)
                {
                    Dbcontext.PROC_TM_PROGRAM_INSERT(PgId, itemTm);
                }
                foreach (var itemSp in spId)
                {
                    Guid newGuid1;
                    if (Guid.TryParse(itemSp, out newGuid1))
                    {
                        Dbcontext.PROC_RELATION_INSERT("SALEPOINT_PROGRAM",newGuid1, g, "1");
                    }
                }
                var dataLog = ser.Serialize(PgName);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "PROGRAM", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Cập nhật ngân hàng thành công";
                return Json(new RolesResult
                {
                    Success = true,
                    Message = "",
                    ReturnUrl = Url.Action("Index")
                });

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

        #region UpdateStatus
        [Route("program/update-status")]
        [HasCredential(FunctionName = "EDIT_PROGRAM")]
        public JsonResult UpdateStatus(string pgId, string status)
        {
            try
            {
                var check = (from pro in Dbcontext.PROGRAMs where (pro.STATUS == "1") select pro).FirstOrDefault();
                if (check != null)
                {
                    Dbcontext.PROC_PROGRAM_UPDATE_STATUS(check.PG_ID, "0");
                }
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_PROGRAM_UPDATE_STATUS(pgId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "PROGRAM", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Chương trình đã cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho chương trình thành công";
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

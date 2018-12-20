using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class SalePointController : BaseController
    {
        //thangnh 31/07/2018
        // GET: /Admin/SalePoint/
        private int _level = -1;
        StringBuilder _sb = new StringBuilder();
        readonly List<SelectListItem> _itemsTrademart = new List<SelectListItem>();
        readonly List<SelectListItem> _itemsProvince = new List<SelectListItem>();
        readonly List<SelectListItem> _itemsDistrict = new List<SelectListItem>();
        readonly List<SelectListItem> _itemsWard = new List<SelectListItem>();
        private string keyapi = ConfigurationManager.AppSettings["key_map"];
        [HasCredential(FunctionName = "VIEW_SALEPOINT")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
        public ActionResult LatLng()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", "", MyIp(), "Danh sách điểm bán chưa có kinh vĩ độ", DateTime.Now);
            return View();
        }
        [Route("salepoint/get-llnull")]
        [HasCredential(FunctionName = "UPLOAD_SALEPOINT")]
        public JsonResult SearchllNull(int pageNo, string tableListLength)
        {
            var listLength = string.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_SALE_POINT_SEARCH_LLNULL(listLength, pageNo, totalRow);
                var rspList = model.ToList();
                return Json(
                   new
                   {
                       Success = true,
                       TotalRow = totalRow.Value,
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
        [Route("salepoint/get-salepoint")]
        [HasCredential(FunctionName = "VIEW_SALEPOINT")]
        public JsonResult Search(string SpName, string SpMerchantName, string SpTmName, string TmId, string province, string district, string SpPhone, string SpWebsite, string Status, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(SpName))
            {
                SpName = null;
            }
            if (string.IsNullOrEmpty(SpMerchantName))
            {
                SpMerchantName = null;
            }
            if (string.IsNullOrEmpty(SpTmName))
            {
                SpTmName = null;
            }
            var tradeMark = string.IsNullOrEmpty(TmId) ? -1 : int.Parse(TmId);
            province = string.IsNullOrEmpty(province) ? null : (from item in Dbcontext.PROVINCEs where (item.PROVINCE_ID == province) select item.PROVINCE_NAME).FirstOrDefault();
            if (string.IsNullOrEmpty(district))
            {
                district = null;
            }
            if (string.IsNullOrEmpty(SpPhone))
            {
                SpPhone = null;
            }
            if (string.IsNullOrEmpty(SpWebsite))
            {
                SpWebsite = null;
            }
            if (string.IsNullOrEmpty(Status))
            {
                Status = null;
            }
            var listLength = string.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);
            try
            {

                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_SALE_POINT_SEARCH(listLength, pageNo, totalRow, SpName, SpMerchantName,
                    SpTmName, tradeMark, province, district, SpPhone, SpWebsite, Status);
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
        #region DropdownList
        public JsonResult Listparent()
        {
            var lstItem1 =
                (from item in Dbcontext.TRADE_MARK where (item.STATUS == "1") select item)
                .ToList();

            RecursiveFillTree(lstItem1, 0);
            IEnumerable<SelectListItem> lsTradeMart = _itemsTrademart;
            return Json(new SelectList(lsTradeMart, "Value", "Text"));
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
                    _itemsTrademart.Add(new SelectListItem { Text = appender + lstChild[i].TM_NAME, Value = lstChild[i].TM_ID.ToString() });
                    RecursiveFillTree(systemCategorys, lstChild[i].TM_ID);
                }
            }
            _level -= 1;
        }
        public JsonResult ListProvince()
        {
            var lstItem1 =
                (from item in Dbcontext.PROVINCEs select item)
                .ToList();
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsProvince.Add(new SelectListItem { Text = lstItem1[i].PROVINCE_NAME, Value = lstItem1[i].PROVINCE_ID });
            }
            IEnumerable<SelectListItem> lsProvince = _itemsProvince;
            return Json(new SelectList(lsProvince, "Value", "Text"));
        }
        public JsonResult ListDistrict(string provinceName)
        {
            var lstItem1 =
                (from item in Dbcontext.DISTRICTs where (item.PROVINCE_ID == provinceName) select item)
                .ToList();
            _itemsDistrict.Add(new SelectListItem { Text = "", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsDistrict.Add(new SelectListItem { Text = lstItem1[i].DISTRICT_NAME, Value = lstItem1[i].DISTRICT_NAME });
            }
            IEnumerable<SelectListItem> lsDistrict = _itemsDistrict;
            return Json(new SelectList(lsDistrict, "Value", "Text"));
        }
        public JsonResult ListDistrictSearch(string provinceName)
        {
            var lstItem1 =
                (from item in Dbcontext.DISTRICTs where (item.PROVINCE_ID == provinceName) select item)
                .ToList();
            _itemsDistrict.Add(new SelectListItem { Text = "-- Tất cả --", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsDistrict.Add(new SelectListItem { Text = lstItem1[i].DISTRICT_NAME, Value = lstItem1[i].DISTRICT_NAME });
            }
            IEnumerable<SelectListItem> lsDistrict = _itemsDistrict;
            return Json(new SelectList(lsDistrict, "Value", "Text"));
        }
        public JsonResult ListWard(string districtName)
        {
            districtName = (from item in Dbcontext.DISTRICTs where (item.DISTRICT_NAME == districtName) select item.DISTRICT_ID)
                .FirstOrDefault();
            var lstItem1 =
                (from item in Dbcontext.WARDs where (item.DISTRICT_ID == districtName) select item)
                .ToList();
            _itemsWard.Add(new SelectListItem { Text = "-- Tất cả --", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsWard.Add(new SelectListItem { Text = lstItem1[i].WARD_NAME, Value = lstItem1[i].WARD_NAME });
            }
            IEnumerable<SelectListItem> lsWard = _itemsWard;
            return Json(new SelectList(lsWard, "Value", "Text"));
        }
        public JsonResult ListTmName()
        {
            var lstItem1 =
                (from item in Dbcontext.SALE_POINT select item).GroupBy(item => item.SP_TM_NAME)
                .ToList();

            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsWard.Add(new SelectListItem { Text = lstItem1[i].Key, Value = lstItem1[i].Key });
            }
            IEnumerable<SelectListItem> lsWard = _itemsWard;
            return Json(new SelectList(lsWard, "Value", "Text"));
        }
        #endregion
        //
        // GET: /Admin/SalePoint/Details/5
        [HasCredential(FunctionName = "DETAILS_SALEPOINT")]
        public ActionResult Details(int id)
        {
            try
            {
                var salePoint = (from sp in Dbcontext.SALE_POINT where (sp.SP_ID == id) select sp).FirstOrDefault();
                if (salePoint == null) return View();
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", id.ToString(), MyIp(), "Xem chi tiết", DateTime.Now);
                return View(salePoint);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Create");
            }
        }
        #region Create
        //
        // GET: /Admin/SalePoint/Create
        [HasCredential(FunctionName = "ADD_SALEPOINT")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/SalePoint/Create
        [HttpPost]
        [HasCredential(FunctionName = "ADD_SALEPOINT")]
        public ActionResult Create(FormCollection collection, SalePointModel model)
        {
            try
            {
                model.ProvinceName = (from item in Dbcontext.PROVINCEs where (item.PROVINCE_ID == model.ProvinceName) select item.PROVINCE_NAME).FirstOrDefault();
                var checkSp = (from sp in Dbcontext.SALE_POINT
                               where
                                   (sp.SP_MERCHANT_NAME == model.SpMerchantName) && (sp.PROVINCE_NAME == model.ProvinceName) &&
                                   (sp.DISTRICT_NAME == model.DistrictName) && (sp.SP_ADDRESS == model.SpAddress)
                               select sp).FirstOrDefault();
                if (checkSp == null)
                {
                    var newGuid = Guid.NewGuid();
                    var fullAd = model.SpAddress + ", " + model.WardName + ", " + model.DistrictName + ", " + model.ProvinceName;
                    var latlng = GetLocateByAddress(fullAd);
                    if (!string.IsNullOrEmpty(latlng.Latitude) && !string.IsNullOrEmpty(latlng.Longitude))
                    {
                        Dbcontext.PROC_SALE_POINT_INSERT(model.SpMerchantName, model.SpTmName, model.TmId, model.SpName,
                            model.ProvinceName, model.DistrictName, model.WardName, model.SpAddress, fullAd, model.SpPhone,
                            model.SpWebsite, model.SpDescription, model.ImageUrl, latlng.Latitude,
                            latlng.Longitude, "1", CurrentUser.UserName, newGuid);
                        var dataLog = ser.Serialize(model);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                        TempData["RspCode"] = "00";
                        TempData["Message"] = "Thêm mới điểm bán thành công";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Log.ErrorFormat("Get latitude, longitude fail");
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Không lấy được kinh độ và vĩ độ bạn hãy thử click button LƯU sau 10 giây";
                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    Log.ErrorFormat("Duplicate Salepoint");
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Điểm bán đã tồn tại";
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
        #region edit
        //
        // GET: /Admin/SalePoint/Edit/5
        [HasCredential(FunctionName = "EDIT_SALEPOINT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var salePoint = (from sp in Dbcontext.SALE_POINT where (sp.SP_ID == id) select sp).FirstOrDefault();
                //  if (salePoint == null) return View();
                //  salePoint.PROVINCE_NAME = (from item in Dbcontext.PROVINCEs where (item.PROVINCE_NAME == salePoint.PROVINCE_NAME) select item.PROVINCE_NAME).FirstOrDefault();
                return View(salePoint);
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
        // POST: /Admin/SalePoint/Edit/5
        [HttpPost]
        [HasCredential(FunctionName = "EDIT_SALEPOINT")]
        public ActionResult Edit(FormCollection collection, SalePointModel model)
        {
            try
            {
                model.ProvinceName = (from item in Dbcontext.PROVINCEs where (item.PROVINCE_ID == model.ProvinceName) select item.PROVINCE_NAME).FirstOrDefault();
                var checkSp = (from sp in Dbcontext.SALE_POINT
                               where
                                   (sp.SP_ID == model.SpId)
                               select sp).FirstOrDefault();
                if (checkSp != null)
                {
                    var checkSp1 = (from sp in Dbcontext.SALE_POINT
                                    where
                                        (sp.SP_MERCHANT_NAME == model.SpMerchantName) && (sp.PROVINCE_NAME == model.ProvinceName) &&
                                        (sp.DISTRICT_NAME == model.DistrictName) && (sp.SP_ADDRESS == model.SpAddress) && (sp.SP_ID == model.SpId)
                                    select sp).FirstOrDefault();
                    if (checkSp1 != null)
                    {
                        var fullAd = model.SpAddress + ", " + model.WardName + ", " + model.DistrictName + ", " + model.ProvinceName;
                        var latlng = GetLocateByAddress(fullAd);
                        if (!string.IsNullOrEmpty(latlng.Latitude) && !string.IsNullOrEmpty(latlng.Longitude))
                        {
                            Dbcontext.PROC_SALE_POINT_UPDATE(model.SpId, model.SpMerchantName, model.SpTmName, model.TmId, model.SpName, model.ProvinceName, model.DistrictName, model.WardName, model.SpAddress, fullAd, model.SpPhone, model.SpWebsite, model.SpDescription, model.ImageUrl, latlng.Latitude, latlng.Longitude, CurrentUser.UserName, DateTime.Now);
                            TempData["RspCode"] = "00";
                            TempData["Message"] = "Cập nhật điểm bán thành công";
                            var dataLog = ser.Serialize(model);
                            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Log.ErrorFormat("Get latitude, longitude fail");
                            TempData["RspCode"] = "99";
                            TempData["Message"] = "Không lấy được kinh độ và vĩ độ bạn hãy thử lại sau 10 giây";
                            return RedirectToAction("Edit", new { id = model.SpId });
                        }
                    }
                    else
                    {
                        var checkSp2 = (from sp in Dbcontext.SALE_POINT
                                        where
                                            (sp.SP_MERCHANT_NAME == model.SpMerchantName) && (sp.PROVINCE_NAME == model.ProvinceName) &&
                                            (sp.DISTRICT_NAME == model.DistrictName) && (sp.SP_ADDRESS == model.SpAddress)
                                        select sp).FirstOrDefault();
                        if (checkSp2 == null)
                        {
                            var fullAd = model.SpAddress + ", " + model.WardName + ", " + model.DistrictName + ", " + model.ProvinceName;
                            var latlng = GetLocateByAddress(fullAd);
                            if (!string.IsNullOrEmpty(latlng.Latitude) && !string.IsNullOrEmpty(latlng.Longitude))
                            {
                                Dbcontext.PROC_SALE_POINT_UPDATE(model.SpId, model.SpMerchantName, model.SpTmName, model.TmId, model.SpName, model.ProvinceName, model.DistrictName, model.WardName, model.SpAddress, fullAd, model.SpPhone, model.SpWebsite, model.SpDescription, model.ImageUrl, latlng.Latitude, latlng.Longitude, CurrentUser.UserName, DateTime.Now);
                                TempData["RspCode"] = "00";
                                TempData["Message"] = "Cập nhật điểm bán thành công";
                                var dataLog = ser.Serialize(model);
                                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                Log.ErrorFormat("Get latitude, longitude fail");
                                TempData["RspCode"] = "99";
                                TempData["Message"] = "Không lấy được kinh độ và vĩ độ bạn hãy thử click button LƯU sau 10 giây";
                                return RedirectToAction("Edit", new { id = model.SpId });
                            }
                        }
                        else
                        {
                            Log.ErrorFormat("Duplicate Salepoint");
                            TempData["RspCode"] = "99";
                            TempData["Message"] = "Điểm bán đã tồn tại";
                            return RedirectToAction("Edit", new { id = model.SpId });
                        }
                    }
                }
                else
                {
                    Log.ErrorFormat("ID NULL");
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Mã Điểm Bán Trống";
                    return RedirectToAction("Edit", new { id = model.SpId });
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Edit", new { id = model.SpId });
            }
        }
        #endregion
        #region UpdateStatus
        [Route("salepoint/update-status")]
        [HasCredential(FunctionName = "EDIT_SALEPOINT")]
        public JsonResult UpdateStatus(int spId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_SALE_POINT_UPDATE_STATUS(spId, statusUpdate);
                var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Điểm bán đã cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho điểm bán thành công";
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
        #region Upload
        [HasCredential(FunctionName = "UPLOAD_SALEPOINT")]
        public ActionResult UploadSp()
        {
            return View();
        }
        [HttpPost]
        [HasCredential(FunctionName = "UPLOAD_SALEPOINT")]
        public ActionResult UploadSp(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                var httpPostedFileBase = Request.Files["file"];
                if (httpPostedFileBase == null || httpPostedFileBase.ContentLength <= 0)
                    return RedirectToAction("UploadSp");
                var postedFileBase = Request.Files["file"];
                if (postedFileBase == null) return RedirectToAction("UploadSp");
                var s = Path.GetExtension(postedFileBase.FileName);
                if (s == null) return RedirectToAction("UploadSp");
                var extension = s.ToLower();
                string[] validFileTypes = { ".xls", ".xlsx", ".csv" };
                var path1 = string.Format("{0}/{1}", Server.MapPath("~/Content/"), postedFileBase.FileName);
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/"));
                }
                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path1))
                    { System.IO.File.Delete(path1); }
                    postedFileBase.SaveAs(path1);
                    var connString = "";
                    switch (extension.Trim())
                    {
                        case ".csv":
                            ConvertCsVtoDataTable(path1);
                            break;
                        case ".xls":
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path1 + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            ConvertXslXtoDataTable(path1, connString);
                            break;
                        case ".xlsx":
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            ConvertXslXtoDataTable(path1, connString);
                            break;
                    }
                }
                else
                {
                    Log.ErrorFormat("File format wrong");
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "File không đúng định dạng";
                    return RedirectToAction("UploadSp");
                }
                return RedirectToAction("UploadSp");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("UploadSp");
            }
        }
        #endregion

        public ActionResult GetLngLat()
        {
            try
            {
                var message = "";
                var i = 0;
                var index = 0;
                var fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "apiMapsKeys.txt");
                var lines = System.IO.File.ReadAllLines(fileName);
                var data =
                    (from sp in Dbcontext.SALE_POINT where (sp.LATITUDE == null) && (sp.LONGITUDE == null) select sp)
                        .ToList();

                foreach (var item in data)
                {
                    var key = "";
                    if (i % 200 == 1 && i > 1000)
                    {
                        if (index < lines.Length)
                        {
                            index++;
                            key = lines[index];
                        }
                        else
                        {
                            index = 0;
                            key = lines[index];
                        }
                        Log.ErrorFormat("Key: {0}, I: {1}", key, i);
                    }
                    else
                    {
                        key = lines[index];
                    }
                    var fullAd = item.SP_ADDRESS + ", " + item.WARD_NAME + ", " + item.DISTRICT_NAME + ", " + item.PROVINCE_NAME;
                    var latlng = Locate(fullAd, key);
                    if (latlng.Latitude != null || latlng.Longitude != null)
                    {
                        Dbcontext.PROC_SALE_POINT_UPDATE_LATLNG(item.SP_ID, latlng.Latitude, latlng.Longitude);
                    }
                    i++;
                }
                message = "Số điểm bán lấy kinh độ vĩ độ thành công là: " + i;

                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", "", MyIp(), "Get Latitude Longitude", DateTime.Now);
                return Json(new RolesResult
                {
                    Success = true,
                    Message = message,
                    ReturnUrl = Url.Action("Index")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(new RolesResult
                {
                    Success = true,
                    Message = "",
                    ReturnUrl = Url.Action("Index")
                }, JsonRequestBehavior.AllowGet);
            }

        }
        #region Excel
        public ActionResult ConvertCsVtoDataTable(string fileName)
        {
            try
            {
                var r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                var sr = new StreamReader(fileName);
                var line = sr.ReadLine();
                var i = 1;
                var countCheckInsertF = 0;
                var countCheckInsertS = 0;
                var indexCheckInsertF = "";
                if (line != null && line.Split(',').Length == 12)
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        var row = r.Split(line);
                        if (row.Length != 12) continue;
                        var spMerchantName = row[0].Trim();
                        var spTmName = row[1].Trim();
                        var idtm = row[2].Trim();
                        var spName = row[3].Trim();
                        var provinceName = row[4].Trim();
                        var districtName = row[5].Trim();
                        var wardName = row[6].Trim();
                        var spAddress = row[7].Trim();
                        var spPhone = row[8].Trim();
                        var spWebsite = row[9].Trim();
                        var spDescription = row[10].Trim();
                        var imageUrl = "";
                        if (!string.IsNullOrEmpty(row[11]))
                        {
                            var keyImg = ConfigurationManager.AppSettings["key_image_salepoint"];
                            var linkImg =
                                (from st in Dbcontext.SETTINGS where (st.ST_KEY == keyImg) select st.ST_VALUE)
                                    .FirstOrDefault();
                            imageUrl = linkImg + row[11];
                        }
                        if (spMerchantName.Length > 300)
                        {
                            spMerchantName = spMerchantName.Substring(0, 300);
                        }
                        if (spName.Length > 300)
                        {
                            spName = spName.Substring(0, 300);
                        }
                        if (spTmName.Length > 300)
                        {
                            spTmName = spTmName.Substring(0, 300);
                        }
                        if (spPhone.Length > 12)
                        {
                            spPhone = spPhone.Substring(0, 12);
                        }
                        if (spWebsite.Length > 200)
                        {
                            spWebsite = spWebsite.Substring(0, 200);
                        }
                        if (spWebsite.Length > 300)
                        {
                            spDescription = spDescription.Substring(0, 300);
                        }
                        if (!string.IsNullOrEmpty(spName) && !string.IsNullOrEmpty(spMerchantName) &&
                            !string.IsNullOrEmpty(spTmName) && !string.IsNullOrEmpty(idtm) &&
                            !string.IsNullOrEmpty(districtName) &&
                            !string.IsNullOrEmpty(wardName) && !string.IsNullOrEmpty(spAddress))
                        {
                            var tmId = 0;
                            var proviceId =
                                  (from p in Dbcontext.PROVINCEs
                                   where (p.PROVINCE_NAME == provinceName)
                                   select p.PROVINCE_ID).FirstOrDefault();
                            if (proviceId != null)
                            {
                                var district =
                                    Dbcontext.PROC_DISTRICT_GETBYNAME(districtName, proviceId).FirstOrDefault();
                                if (district != null)
                                {
                                    districtName = district.DISTRICT_NAME;
                                    var ward =
                                        Dbcontext.PROC_WARD_GETBYNAME(wardName, district.DISTRICT_ID)
                                            .FirstOrDefault();
                                    if (ward != null)
                                    {
                                        wardName = ward;
                                    }
                                }
                            }
                            var checkSp = (from sp in Dbcontext.SALE_POINT
                                           where
                                               (sp.SP_MERCHANT_NAME == spMerchantName) && (sp.PROVINCE_NAME == provinceName) &&
                                               (sp.DISTRICT_NAME == districtName) && (sp.SP_ADDRESS == spAddress)
                                           select sp).FirstOrDefault();
                            var fullAd = spAddress + ", " + wardName + ", " + districtName + ", " + provinceName;
                            var gettmid = Dbcontext.PROC_TRADE_MARK_SELECTBYNAME(idtm).FirstOrDefault();
                            if (gettmid != null)
                            {
                                tmId = int.Parse(gettmid.ToString());
                                if (checkSp == null)
                                {
                                    var newGuid = Guid.NewGuid();
                                    var rs = Dbcontext.PROC_SALE_POINT_INSERT(spMerchantName, spTmName, tmId, spName,
                                        provinceName, districtName, wardName, spAddress, fullAd, spPhone,
                                        spWebsite, spDescription, imageUrl, null,
                                        null, "1", CurrentUser.UserName, newGuid);
                                    if (rs < 1)
                                    {
                                        countCheckInsertF++;
                                        var index = i + ", ";
                                        indexCheckInsertF = indexCheckInsertF + index;
                                    }
                                    else
                                    {
                                        countCheckInsertS++;
                                    }
                                }
                                else
                                {
                                    Dbcontext.PROC_SALE_POINT_UPDATE(checkSp.SP_ID, spMerchantName, spTmName, tmId, spName, provinceName, districtName, wardName, spAddress, fullAd, spPhone, spWebsite, spDescription, imageUrl, null, null, CurrentUser.UserName, DateTime.Now);
                                }
                            }
                            else
                            {
                                countCheckInsertF++;
                                var index = i + ", ";
                                indexCheckInsertF = indexCheckInsertF + index;
                            }

                        }
                        else
                        {
                            countCheckInsertF++;
                            var index = i + ", ";
                            indexCheckInsertF = indexCheckInsertF + index;
                        }
                        i++;
                    }
                }
                else
                {
                    Log.ErrorFormat("Wrong file structure");
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Cấu trúc file import chưa chính xác. Tham khảo file mẫu đính kèm trong chức năng";
                    return RedirectToAction("UploadSp");
                }
                TempData["indexCheckInsertF"] = indexCheckInsertF;
                TempData["countCheckInsertF"] = countCheckInsertF.ToString();
                TempData["countCheckInsertS"] = countCheckInsertS.ToString();
                sr.Dispose();
                var dataLog = ser.Serialize(countCheckInsertS);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "UPLOAD EXCEL CSV", DateTime.Now);
                return RedirectToAction("UploadSp");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("UploadSp");
            }
        }

        public ActionResult ConvertXslXtoDataTable(string strFilePath, string connString)
        {
            var oledbConn = new OleDbConnection(connString);
            try
            {
                oledbConn.Open();
                using (var cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn))
                {
                    var oleda = new OleDbDataAdapter { SelectCommand = cmd };
                    var ds = new DataSet();
                    oleda.Fill(ds);
                    var checkfomat = ds.Tables[0].Columns.Count;
                    if (checkfomat == 12)
                    {
                        var countCheckInsertF = 0;
                        var countCheckInsertS = 0;
                        var indexCheckInsertF = "";
                        for (var i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            var spMerchantName = ds.Tables[0].Rows[i][0].ToString().Trim();
                            var spTmName = ds.Tables[0].Rows[i][1].ToString().Trim();
                            var spName = ds.Tables[0].Rows[i][3].ToString().Trim();
                            var provinceName = ds.Tables[0].Rows[i][4].ToString().Trim();
                            var districtName = ds.Tables[0].Rows[i][5].ToString().Trim();
                            var wardName = ds.Tables[0].Rows[i][6].ToString().Trim();
                            var spAddress = ds.Tables[0].Rows[i][7].ToString().Trim();
                            var spPhone = ds.Tables[0].Rows[i][8].ToString().Trim();
                            var spWebsite = ds.Tables[0].Rows[i][9].ToString().Trim();
                            var spDescription = ds.Tables[0].Rows[i][10].ToString().Trim();
                            var imageUrl = "";
                            if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i][11].ToString()))
                            {
                                var keyImg = ConfigurationManager.AppSettings["key_image_salepoint"];
                                var linkImg =
                                    (from st in Dbcontext.SETTINGS where (st.ST_KEY == keyImg) select st.ST_VALUE)
                                        .FirstOrDefault();
                                imageUrl = linkImg + ds.Tables[0].Rows[i][11];
                            }
                            if (spMerchantName.Length > 300)
                            {
                                spMerchantName = spMerchantName.Substring(0, 300);
                            }
                            if (spName.Length > 300)
                            {
                                spName = spName.Substring(0, 300);
                            }
                            if (spTmName.Length > 300)
                            {
                                spTmName = spTmName.Substring(0, 300);
                            }
                            if (spPhone.Length > 12)
                            {
                                spPhone = spPhone.Substring(0, 12);
                            }
                            if (spWebsite.Length > 200)
                            {
                                spWebsite = spWebsite.Substring(0, 200);
                            }
                            if (spWebsite.Length > 300)
                            {
                                spDescription = spDescription.Substring(0, 300);
                            }
                            if (!string.IsNullOrEmpty(spName) && !string.IsNullOrEmpty(spMerchantName) &&
                                !string.IsNullOrEmpty(spTmName) && !string.IsNullOrEmpty(ds.Tables[0].Rows[i][2].ToString()) && !string.IsNullOrEmpty(districtName) &&
                                !string.IsNullOrEmpty(wardName) && !string.IsNullOrEmpty(spAddress))
                            {
                                var tmId = 0;
                                var proviceId =
                                    (from p in Dbcontext.PROVINCEs
                                     where (p.PROVINCE_NAME == provinceName)
                                     select p.PROVINCE_ID).FirstOrDefault();
                                if (proviceId != null)
                                {
                                    var district =
                                        Dbcontext.PROC_DISTRICT_GETBYNAME(districtName, proviceId).FirstOrDefault();
                                    if (district != null)
                                    {
                                        districtName = district.DISTRICT_NAME;
                                        var ward =
                                            Dbcontext.PROC_WARD_GETBYNAME(wardName, district.DISTRICT_ID)
                                                .FirstOrDefault();
                                        if (ward != null)
                                        {
                                            wardName = ward;
                                        }
                                    }
                                }
                                var checkSp = (from sp in Dbcontext.SALE_POINT
                                               where
                                                   (sp.SP_MERCHANT_NAME == spMerchantName) && (sp.PROVINCE_NAME == provinceName) &&
                                                   (sp.DISTRICT_NAME == districtName) && (sp.SP_ADDRESS == spAddress)
                                               select sp).FirstOrDefault();
                                var fullAd = spAddress + ", " + wardName + ", " + districtName + ", " + provinceName;

                                var gettmid = Dbcontext.PROC_TRADE_MARK_SELECTBYNAME(ds.Tables[0].Rows[i][2].ToString()).FirstOrDefault();
                                if (gettmid != null)
                                {
                                    tmId = int.Parse(gettmid.ToString());
                                    if (checkSp == null)
                                    {
                                        var newGuid = Guid.NewGuid();
                                        var rs = Dbcontext.PROC_SALE_POINT_INSERT(spMerchantName, spTmName, tmId, spName,
                                            provinceName, districtName, wardName, spAddress, fullAd, spPhone,
                                            spWebsite, spDescription, imageUrl, null,
                                            null, "1", CurrentUser.UserName, newGuid);
                                        if (rs < 1)
                                        {
                                            countCheckInsertF++;
                                            var index = (i + 2) + ", ";
                                            indexCheckInsertF = indexCheckInsertF + index;
                                        }
                                        else
                                        {
                                            countCheckInsertS++;
                                        }
                                    }
                                    else
                                    {
                                        Dbcontext.PROC_SALE_POINT_UPDATE(checkSp.SP_ID, spMerchantName, spTmName, tmId, spName, provinceName, districtName, wardName, spAddress, fullAd, spPhone, spWebsite, spDescription, imageUrl, null, null, CurrentUser.UserName, DateTime.Now);
                                    }
                                }
                                else
                                {
                                    countCheckInsertF++;
                                    var index = (i + 2) + ", ";
                                    indexCheckInsertF = indexCheckInsertF + index;
                                }
                            }
                            else
                            {
                                countCheckInsertF++;
                                var index = (i + 2) + ", ";
                                indexCheckInsertF = indexCheckInsertF + index;
                            }
                        }

                        TempData["indexCheckInsertF"] = indexCheckInsertF;
                        TempData["countCheckInsertF"] = countCheckInsertF.ToString();
                        TempData["countCheckInsertS"] = countCheckInsertS.ToString();
                        var dataLog = ser.Serialize(countCheckInsertS);
                        Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "SALE POINT", "", dataLog, MyIp(), "UPLOAD EXCEL XLS,XLSX", DateTime.Now);
                    }
                    else
                    {
                        Log.ErrorFormat("Wrong file structure");
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Cấu trúc file import chưa chính xác. Tham khảo file mẫu đính kèm trong chức năng";
                        return RedirectToAction("UploadSp");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("UploadSp");
            }
            finally
            {
                oledbConn.Close();
            }
            return RedirectToAction("UploadSp");
        }
        #endregion


    }
}

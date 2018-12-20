using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Library;
using PromotionCMS.Models;
using log4net;

namespace PromotionCMS.Controllers
{
    public class HomeController : BaseController
    {
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ObjectResult<PROC_RECEIVE_NEWS_SEARCH_Result> checkEmail;
        private ObjectResult<PROC_RECEIVE_NEWS_GETBY_PHONE_Result> checkPhone;
        //Thangnh 07/07/2018
        private int _level = -1;
        StringBuilder _sb = new StringBuilder();
        readonly List<SelectListItem> _items = new List<SelectListItem>();
        readonly List<SelectListItem> _itemschild = new List<SelectListItem>();
        readonly List<SelectListItem> _itemsProvince = new List<SelectListItem>();
        readonly List<SelectListItem> _itemsDistrict = new List<SelectListItem>();
        public string baseImg = ConfigurationManager.AppSettings["baseImgweb"];
        public ActionResult Index()
        {
            Session.Abandon();
            var newHot = (from news in Dbcontext.POSTs where (news.POST_STATUS == "1") orderby news.POST_PRIOTITY descending, news.ORD ascending,news.CREATED_DATE descending select news).Take(5).ToList();
            var listBank = (from bank in Dbcontext.BANK_CATEGORY
                            where (bank.BANNER_STATUS == "1")
                            orderby bank.PRIORITIZE descending, bank.ORD ascending,bank.CREATED_DATE descending 
                            select bank).ToList();
            var bannerMiniste =
                (from banner in Dbcontext.BANNER_MINISITE
                 where (banner.BANNER_STATUS == "1")
                 orderby banner.CREATED_ON
                 select banner).FirstOrDefault();
            var textb = ConfigurationManager.AppSettings["key_text_banner"];
            var lvideo = ConfigurationManager.AppSettings["key_list_video"];
            var textbanner = (from s in Dbcontext.SETTINGS where (s.ST_KEY == textb) && (s.STATUS == "1") select s).FirstOrDefault();
            var listVideo = (from s in Dbcontext.SETTINGS where (s.ST_KEY == lvideo) && (s.STATUS == "1") select s).FirstOrDefault();
            ViewBag.newHot = newHot;
            ViewBag.listBank = listBank;
            ViewBag.bannerMiniste = bannerMiniste;
            ViewBag.textbanner = textbanner;
            ViewBag.listVideo = listVideo;
            return View();
        }

        #region DropdownList
        public JsonResult ListTmName()
        {
            var lstItem1 = (from tm in Dbcontext.TM_SP select tm).GroupBy(tm => tm.SP_TM_NAME).ToList();
            _items.Add(new SelectListItem { Text = "Thương hiệu", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _items.Add(new SelectListItem { Text = lstItem1[i].Key, Value = lstItem1[i].Key });
            }
            IEnumerable<SelectListItem> ls = _items;
            return Json(new SelectList(ls, "Value", "Text"));
        }
        [HttpPost]
        public JsonResult ListTmName1(int parentid)
        {
            if (parentid > 0) { 
            var lstItem1 = (from tm in Dbcontext.TM_SP where (tm.TM_ID==parentid) select tm).GroupBy(tm => tm.SP_TM_NAME).ToList();
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _items.Add(new SelectListItem { Text = lstItem1[i].Key, Value = lstItem1[i].Key });
            }
            IEnumerable<SelectListItem> ls = _items;
            return Json(new SelectList(ls, "Value", "Text"));
            }
            else
            {
                return Json(new SelectList(_items, "Value", "Text"));
            }
        }
        public JsonResult Listparent()
        {
            var lstItem1 =
                (from item in Dbcontext.TRADE_MARK where (item.STATUS == "1") && (item.PARENT_ID == 0) orderby item.TM_STATUS descending, item.ORD ascending select item)
                .ToList();
            //_items.Add(new SelectListItem { Text = "Ngành hàng", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _items.Add(new SelectListItem { Text = lstItem1[i].TM_NAME, Value = lstItem1[i].TM_ID.ToString() });
            }
            IEnumerable<SelectListItem> ls = _items;
            return Json(new SelectList(ls, "Value", "Text"));
        }
        [HttpPost]
        public JsonResult ListTmChild(int parentid)
        {
            if (parentid > 0)
            {
                var lstItem1 = (from item in Dbcontext.TRADE_MARK where (item.STATUS == "1") && (item.PARENT_ID == parentid) orderby item.TM_STATUS descending, item.ORD ascending select item)
                    .ToList();
                for (var i = 0; i <= lstItem1.Count - 1; i++)
                {
                    _itemschild.Add(new SelectListItem { Text = lstItem1[i].TM_NAME, Value = lstItem1[i].TM_ID.ToString() });
                    ListTmChild(lstItem1[i].TM_ID);
                }
                IEnumerable<SelectListItem> ls = _itemschild;
                return Json(new SelectList(ls, "Value", "Text"));
            }
            else
            {
                return Json(new SelectList(_items, "Value", "Text"));
            }
        }
        public JsonResult ListProvince()
        {
            var lstItem1 =
                (from item in Dbcontext.PROVINCEs orderby item.TYPE, item.PROVINCE_NAME select item)
                .ToList();
            _itemsProvince.Add(new SelectListItem { Text = "Tỉnh/Thành phố", Value = "", Selected = true });
            for (var i = 0; i <= lstItem1.Count - 1; i++)
            {
                _itemsProvince.Add(new SelectListItem { Text = lstItem1[i].PROVINCE_NAME, Value = lstItem1[i].PROVINCE_NAME });
            }
            IEnumerable<SelectListItem> lsProvince = _itemsProvince;
            return Json(new SelectList(lsProvince, "Value", "Text"));
        }
        [HttpPost]
        public JsonResult ListDistrict(string provinceName)
        {
            var province =
                (from p in Dbcontext.PROVINCEs where (p.PROVINCE_NAME == provinceName)  select p.PROVINCE_ID)
                    .FirstOrDefault();
            if (province != null)
            {
                var lstItem1 =
                    (from item in Dbcontext.DISTRICTs where (item.PROVINCE_ID == province) orderby item.DISTRICT_NAME select item)
                    .ToList();
                _itemsDistrict.Add(new SelectListItem { Text = "Quận/huyện", Value = "", Selected = true });
                for (var i = 0; i <= lstItem1.Count - 1; i++)
                {
                    _itemsDistrict.Add(new SelectListItem { Text = lstItem1[i].DISTRICT_NAME, Value = lstItem1[i].DISTRICT_NAME });
                }
                IEnumerable<SelectListItem> lsDistrict = _itemsDistrict;
                return Json(new SelectList(lsDistrict, "Value", "Text"));
            }
            else
            {
                return Json(new SelectList(_itemsDistrict, "Value", "Text"));
            }
        }
        #endregion

        public ActionResult ReceiveNewsCreate(ReceiveNewsModel model)
        {
            try
            {
                //check email
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(Int32));
                const string infomationSource = "Minisite Khuyến Mại";
                if (string.IsNullOrEmpty(model.RnName))
                {
                    model.RnName = null;
                }
                if (string.IsNullOrEmpty(model.RnPhone))
                {
                    model.RnPhone = null;
                }
                if (string.IsNullOrEmpty(model.RnEmail))
                {
                    model.RnEmail = null;
                }
                if (model.RnPhone != null)
                {
                    checkPhone = Dbcontext.PROC_RECEIVE_NEWS_GETBY_PHONE(10, 1, totalRow, model.RnPhone);

                }
                if (model.RnEmail != null)
                {
                    checkEmail = Dbcontext.PROC_RECEIVE_NEWS_SEARCH(10, 1, totalRow, null, model.RnEmail);
                }

                if (model.RnEmail != null || model.RnPhone != null)
                {
                    if (model.RnEmail == null && model.RnPhone != null)
                    {
                        var checkListPhone = checkPhone.ToList();


                        if (checkListPhone.Count == 0)
                        {
                            var rs = Dbcontext.PROC_RECEIVE_NEWS_INSERT(model.RnName, model.RnEmail, model.RnPhone, infomationSource, DateTime.Now);
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
                                  code = "03",
                                  message = "fail"
                              });
                        }
                    }
                    else if (model.RnEmail != null && model.RnPhone != null)
                    {
                        var checkListPhone = checkPhone.ToList();
                        var checkListEmail = checkEmail.ToList();
                        if (checkListPhone.Count == 0 && checkListEmail.Count == 0)
                        {
                            var rs = Dbcontext.PROC_RECEIVE_NEWS_INSERT(model.RnName, model.RnEmail, model.RnPhone, infomationSource, DateTime.Now);
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
                    else
                    {
                        var checkListEmail = checkEmail.ToList();
                        if (checkListEmail.Count == 0)
                        {
                            var rs = Dbcontext.PROC_RECEIVE_NEWS_INSERT(model.RnName, model.RnEmail, model.RnPhone, infomationSource, DateTime.Now);
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
                                  code = "04",
                                  message = "fail"
                              });
                        }
                    }
                }
                else
                {
                    return Json(
                      new
                      {
                          code = "01",
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
        #region SearchType2
        [HttpPost, ValidateInput(false)]
        public ActionResult GetSearchLocationAdd(string txtAddress, string nganhhang, int distance, string lat, string lng, string province, string district, string ward, string addressUser, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var spId = "";
                var spName = "";
                var spTmName = "";
                var spMerchantName = "";
                var tmId = "";
                var provinceName = "";
                var districtName = "";
                var wardName = "";
                var spAddress = "";
                var spPhone = "";
                var spWebsite = "";
                var spDescription = "";
                var imageUrl = "";
                var longitude = "";
                var latitude = "";
                var status = "";
                var createdBy = "";
                var tmName = "";
                var i = 0;
                double to = 0;
                double totalPages = 0;
                if (string.IsNullOrEmpty(txtAddress) && string.IsNullOrEmpty(province) &&
                    string.IsNullOrEmpty(district) && string.IsNullOrEmpty(ward) && string.IsNullOrEmpty(lat) &&
                    string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(addressUser))
                {
                    var ll = GetLocateByAddress(addressUser);
                    if (ll != null)
                    {
                        lat = ll.Latitude;
                        lng = ll.Longitude;
                        var ad = GetLocateByLngLat(lat, lng);
                        Session["txtAddress"] = ad.Address;
                        Session["province"] = ad.Province;
                        Session["district"] = ad.District;
                        Session["ward"] = ad.Ward;
                    }

                }
                if (string.IsNullOrEmpty(txtAddress) && string.IsNullOrEmpty(province) &&
                    string.IsNullOrEmpty(district) && string.IsNullOrEmpty(ward) && !string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                {
                    if (string.IsNullOrEmpty(Session["ward"] as string) &&
                        string.IsNullOrEmpty(Session["txtAddress"] as string) &&
                        string.IsNullOrEmpty(Session["province"] as string) &&
                        string.IsNullOrEmpty(Session["district"] as string))
                    {
                        var ad = GetLocateByLngLat(lat, lng);
                        Session["txtAddress"] = ad.Address;
                        Session["province"] = ad.Province;
                        Session["district"] = ad.District;
                        Session["ward"] = ad.Ward;
                    }
                }
                else
                {
                    Session["txtAddress"] = txtAddress;
                    Session["province"] = province;
                    Session["district"] = district;
                    Session["ward"] = ward;
                }
                txtAddress = string.IsNullOrEmpty(Session["txtAddress"] as string) ? "" : Session["txtAddress"].ToString();
                province = string.IsNullOrEmpty(Session["province"] as string) ? "" : Session["province"].ToString();
                district = string.IsNullOrEmpty(Session["district"] as string) ? "" : Session["district"].ToString();
                ward = string.IsNullOrEmpty(Session["ward"] as string) ? "" : Session["ward"].ToString();
                //if (string.IsNullOrEmpty(ward))
                //{
                //    ward = addressUser;
                //}
                var province1 = Utils.LocDau(province).Trim();
                var addressUser1 = Utils.LocDau(addressUser).Trim();
                if (string.Equals(province1, addressUser1, StringComparison.CurrentCultureIgnoreCase))
                {
                    txtAddress = "";
                    district = "";
                    ward = "";
                }
                var district1 = Utils.LocDau(district).Trim();
                if (string.Equals(district1, addressUser1, StringComparison.CurrentCultureIgnoreCase))
                {
                    txtAddress = "";
                    province = "";
                    ward = "";
                }
                var ward1 = Utils.LocDau(ward).Trim();
                if (string.Equals(ward1, addressUser1, StringComparison.CurrentCultureIgnoreCase))
                {
                    txtAddress = "";
                    province = "";
                    district = "";
                }
                var txtAddress1 = Utils.LocDau(txtAddress).Trim();
                if (string.Equals(txtAddress1, addressUser1, StringComparison.CurrentCultureIgnoreCase))
                {
                    province = "";
                    district = "";
                    ward = "";
                }
                var nh = string.IsNullOrEmpty(nganhhang) ? -1 : int.Parse(nganhhang);
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                Array[] listMc;
                if (distance > 0)
                {
                    var listItemIndex = Dbcontext
                        .FRONT_SP_SEARCH(100000000, 1, totalRow, province, district, ward, "", nh).ToList();
                    var listItem1 = new List<SpointModel>();
                    foreach (var itemIndex in listItemIndex)
                    {
                        var km = Utils.CalculateDistance(double.Parse(lat), double.Parse(lng),
                            double.Parse(itemIndex.LATITUDE),
                            double.Parse(itemIndex.LONGITUDE));
                        if (!(distance >= km)) continue;
                        var item1 = new SpointModel
                        {
                            SpId = itemIndex.SP_ID.ToString(),
                            SpName = itemIndex.SP_NAME,
                            SpMerchantName = itemIndex.SP_MERCHANT_NAME,
                            TmId = itemIndex.TM_ID.ToString(),
                            ProvinceName = itemIndex.PROVINCE_NAME,
                            DistrictName = itemIndex.DISTRICT_NAME,
                            WardName = itemIndex.WARD_NAME,
                            SpAddress = itemIndex.SP_ADDRESS,
                            SpPhone = itemIndex.SP_PHONE,
                            SpWebsite = itemIndex.SP_WEBSITE,
                            SpDescription = itemIndex.SP_DESCRIPTION,
                            ImageUrl = itemIndex.IMAGE_URL,
                            Longitude = itemIndex.LONGITUDE,
                            Latitude = itemIndex.LATITUDE,
                            Status = itemIndex.STATUS
                        };
                        listItem1.Add(item1);
                    }
                    var listDistance = listItem1.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                    to = double.Parse(listItem1.Count.ToString());
                    totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                    listMc = new Array[listDistance.Count];
                    foreach (var item in listDistance)
                    {
                        spId = item.SpId; //0
                        spName = item.SpName; //1
                        spTmName = item.SpTmName; //2
                        spMerchantName = item.SpMerchantName; //3
                        tmId = item.TmId; //4
                        provinceName = item.ProvinceName; //5
                        districtName = item.DistrictName; //6
                        wardName = item.WardName; //7
                        spAddress = item.SpAddress + "," + item.WardName + "," + item.DistrictName + "," + item.ProvinceName; //8
                        spPhone = item.SpPhone; //9
                        spWebsite = item.SpWebsite; //10
                        spDescription = item.SpDescription; //11
                        if (!string.IsNullOrEmpty(item.ImageUrl))
                        {
                            imageUrl = item.ImageUrl;
                        }
                        else
                        {
                            imageUrl = baseImg + "nophoto.png";
                        }
                        longitude = item.Longitude; //13
                        latitude = item.Latitude; //14
                        status = item.Status; //15
                        createdBy = item.CreatedBy; //16
                        var spIcon = baseImg + "pinblue.svg"; //17
                        const string spCss = "hot"; //18
                        string[] returnView =
                            {
                                spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName,
                                spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status,
                                createdBy, spIcon, spCss
                            };
                        listMc[i] = returnView;
                        i++;
                    }

                }
                else
                {
                    var listItem = Dbcontext.FRONT_SP_SEARCH(pageSize, pageNumber, totalRow, province, district, ward, "", nh).ToList();
                    to = double.Parse(totalRow.Value.ToString());
                    totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                    listMc = new Array[listItem.Count];
                    i = 0;
                    foreach (var item in listItem)
                    {
                        spId = item.SP_ID.ToString();
                        spName = item.SP_NAME;
                        spTmName = item.SP_TM_NAME;
                        spMerchantName = item.SP_MERCHANT_NAME;
                        tmId = item.TM_ID.ToString();
                        provinceName = item.PROVINCE_NAME;
                        districtName = item.DISTRICT_NAME;
                        wardName = item.WARD_NAME;
                        spAddress = item.SP_ADDRESS + "," + item.WARD_NAME + "," + item.DISTRICT_NAME + "," + item.PROVINCE_NAME; //8
                        spPhone = item.SP_PHONE;
                        spWebsite = item.SP_WEBSITE;
                        spDescription = item.SP_DESCRIPTION;
                        if (!string.IsNullOrEmpty(item.IMAGE_URL))
                        {
                            imageUrl = item.IMAGE_URL;
                        }
                        else
                        {
                            imageUrl = baseImg + "nophoto.png";
                        }
                        longitude = item.LONGITUDE;
                        latitude = item.LATITUDE;
                        status = item.STATUS;
                        createdBy = item.CREATED_BY;
                        var spIcon = baseImg + "pinblue.svg";
                        const string spCss = "hot";
                        string[] returnView = { spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName, spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status, createdBy, spIcon, spCss };
                        listMc[i] = returnView;
                        i++;
                    }
                }
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalPages,
                        RspList = listMc,
                        indexList = to
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }
        }

       [HttpPost, ValidateInput(false)]
        public ActionResult GetSearchLocation(string txtAddress, string nganhhang, int distance, string lat, string lng, string province, string district, string ward, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var spId = "";
                var spName = "";
                var spTmName = "";
                var spMerchantName = "";
                var tmId = "";
                var provinceName = "";
                var districtName = "";
                var wardName = "";
                var spAddress = "";
                var spPhone = "";
                var spWebsite = "";
                var spDescription = "";
                var imageUrl = "";
                var longitude = "";
                var latitude = "";
                var status = "";
                var createdBy = "";
                var i = 0;
                double to = 0;
                double totalPages = 0;
                if (string.IsNullOrEmpty(txtAddress) && string.IsNullOrEmpty(province) &&
                    string.IsNullOrEmpty(district) && string.IsNullOrEmpty(ward) && !string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                {
                    if (string.IsNullOrEmpty(Session["ward"] as string) &&
                        string.IsNullOrEmpty(Session["txtAddress"] as string) &&
                        string.IsNullOrEmpty(Session["province"] as string) &&
                        string.IsNullOrEmpty(Session["district"] as string))
                    {
                        var ad = GetLocateByLngLat(lat, lng);
                        Session["txtAddress"] = ad.Address;
                        Session["province"] = ad.Province;
                        Session["district"] = ad.District;
                        Session["ward"] = ad.Ward;
                    }
                }
                else
                {
                    Session["txtAddress"] = txtAddress;
                    Session["province"] = province;
                    Session["district"] = district;
                    Session["ward"] = ward;
                }
                txtAddress = string.IsNullOrEmpty(Session["txtAddress"] as string) ? "" : Session["txtAddress"].ToString();
                province = string.IsNullOrEmpty(Session["province"] as string) ? "" : Session["province"].ToString();
                district = string.IsNullOrEmpty(Session["district"] as string) ? "" : Session["district"].ToString();
                ward = string.IsNullOrEmpty(Session["ward"] as string) ? "" : Session["ward"].ToString();
                //if (string.IsNullOrEmpty(ward))
                //{
                //    ward = txtAddress;
                //}
                var nh = string.IsNullOrEmpty(nganhhang) ? -1 : int.Parse(nganhhang);
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                Array[] listMc;
                if (distance > 0)
                {
                    var listItemIndex = Dbcontext
                        .FRONT_SP_SEARCH(100000000, 1, totalRow, province, district, ward, "", nh).ToList();
                    var listItem1 = new List<SpointModel>();
                    foreach (var itemIndex in listItemIndex)
                    {
                        var km = Utils.CalculateDistance(double.Parse(lat), double.Parse(lng),
                            double.Parse(itemIndex.LATITUDE),
                            double.Parse(itemIndex.LONGITUDE));
                        if (!(distance >= km)) continue;
                        var item1 = new SpointModel
                        {
                            SpId = itemIndex.SP_ID.ToString(),
                            SpName = itemIndex.SP_NAME,
                            SpMerchantName = itemIndex.SP_MERCHANT_NAME,
                            TmId = itemIndex.TM_ID.ToString(),
                            ProvinceName = itemIndex.PROVINCE_NAME,
                            DistrictName = itemIndex.DISTRICT_NAME,
                            WardName = itemIndex.WARD_NAME,
                            SpAddress = itemIndex.SP_ADDRESS,
                            SpPhone = itemIndex.SP_PHONE,
                            SpWebsite = itemIndex.SP_WEBSITE,
                            SpDescription = itemIndex.SP_DESCRIPTION,
                            ImageUrl = itemIndex.IMAGE_URL,
                            Longitude = itemIndex.LONGITUDE,
                            Latitude = itemIndex.LATITUDE,
                            Status = itemIndex.STATUS
                        };
                        listItem1.Add(item1);
                    }
                    var listDistance = listItem1.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                    to = double.Parse(listItem1.Count.ToString());
                    totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                    listMc = new Array[listDistance.Count];
                    foreach (var item in listDistance)
                    {
                        spId = item.SpId; //0
                        spName = item.SpName; //1
                        spTmName = item.SpTmName; //2
                        spMerchantName = item.SpMerchantName; //3
                        tmId = item.TmId; //4
                        provinceName = item.ProvinceName; //5
                        districtName = item.DistrictName; //6
                        wardName = item.WardName; //7
                        spAddress = item.SpAddress + "," + item.WardName + "," + item.DistrictName + "," + item.ProvinceName; //8
                        spPhone = item.SpPhone; //9
                        spWebsite = item.SpWebsite; //10
                        spDescription = item.SpDescription; //11
                        if (!string.IsNullOrEmpty(item.ImageUrl))
                        {
                            imageUrl = item.ImageUrl;
                        }
                        else
                        {
                            imageUrl = baseImg + "nophoto.png";
                        }
                        longitude = item.Longitude; //13
                        latitude = item.Latitude; //14
                        status = item.Status; //15
                        createdBy = item.CreatedBy; //16
                        var spIcon = baseImg + "pinblue.svg"; //17
                        const string spCss = "hot"; //18
                        string[] returnView =
                            {
                                spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName,
                                spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status,
                                createdBy, spIcon, spCss
                            };
                        listMc[i] = returnView;
                        i++;
                    }

                }
                else
                {
                    var listItem = Dbcontext.FRONT_SP_SEARCH(pageSize, pageNumber, totalRow, province, district, ward, "", nh).ToList();
                    to = double.Parse(totalRow.Value.ToString());
                    totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                    listMc = new Array[listItem.Count];
                    i = 0;
                    foreach (var item in listItem)
                    {
                        spId = item.SP_ID.ToString();
                        spName = item.SP_NAME;
                        spTmName = item.SP_TM_NAME;
                        spMerchantName = item.SP_MERCHANT_NAME;
                        tmId = item.TM_ID.ToString();
                        provinceName = item.PROVINCE_NAME;
                        districtName = item.DISTRICT_NAME;
                        wardName = item.WARD_NAME;
                        spAddress = item.SP_ADDRESS + "," + item.WARD_NAME + "," + item.DISTRICT_NAME + "," + item.PROVINCE_NAME; //8
                        spPhone = item.SP_PHONE;
                        spWebsite = item.SP_WEBSITE;
                        spDescription = item.SP_DESCRIPTION;
                        if (!string.IsNullOrEmpty(item.IMAGE_URL))
                        {
                            imageUrl = item.IMAGE_URL;
                        }
                        else
                        {
                            imageUrl = baseImg + "nophoto.png";
                        }
                        longitude = item.LONGITUDE;
                        latitude = item.LATITUDE;
                        status = item.STATUS;
                        createdBy = item.CREATED_BY;
                        var spIcon = baseImg + "pinblue.svg";
                        const string spCss = "hot";
                        string[] returnView = { spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName, spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status, createdBy, spIcon, spCss };
                        listMc[i] = returnView;
                        i++;
                    }
                }
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalPages,
                        RspList = listMc,
                        indexList = to
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GetSearchAllLocation(string nganhhang, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var i = 0;
                double to = 0;
                double totalPages = 0;
                var nh = string.IsNullOrEmpty(nganhhang) ? -1 : int.Parse(nganhhang);
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var listItem = Dbcontext.FRONT_SP_SEARCH(pageSize, pageNumber, totalRow, "", "", "", "", nh).ToList();
                to = double.Parse(totalRow.Value.ToString());
                totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                var listMc = new Array[listItem.Count];
                i = 0;
                foreach (var item in listItem)
                {
                    var spId = item.SP_ID.ToString();
                    var spName = item.SP_NAME;
                    var spTmName = item.SP_TM_NAME;
                    var spMerchantName = item.SP_MERCHANT_NAME;
                    var tmId = item.TM_ID.ToString();
                    var provinceName = item.PROVINCE_NAME;
                    var districtName = item.DISTRICT_NAME;
                    var wardName = item.WARD_NAME;
                    var spAddress = item.SP_ADDRESS + "," + item.WARD_NAME + "," + item.DISTRICT_NAME + "," + item.PROVINCE_NAME; //8
                    var spPhone = item.SP_PHONE;
                    var spWebsite = item.SP_WEBSITE;
                    var spDescription = item.SP_DESCRIPTION;
                    var imageUrl = "";
                    if (!string.IsNullOrEmpty(item.IMAGE_URL))
                    {
                        imageUrl = item.IMAGE_URL;
                    }
                    else
                    {
                        imageUrl = baseImg + "nophoto.png";
                    }
                    var longitude = item.LONGITUDE;
                    var latitude = item.LATITUDE;
                    var status = item.STATUS;
                    var createdBy = item.CREATED_BY;
                    var spIcon = baseImg + "pinblue.svg";
                    const string spCss = "hot";
                    string[] returnView = { spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName, spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status, createdBy, spIcon, spCss };
                    listMc[i] = returnView;
                    i++;
                }

                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalPages,
                        RspList = listMc,
                        indexList = to
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }
        }
        #endregion
       [HttpPost, ValidateInput(false)]
        public ActionResult GetSalePointById(int id)
        {
            try
            {
                var sp1 = (from sp in Dbcontext.TM_SP where (sp.SP_ID == id) select sp).FirstOrDefault();
                if (sp1 != null)
                {
                    var spId = sp1.SP_ID.ToString();
                    var spName = sp1.SP_NAME;
                    var spTmName = sp1.SP_TM_NAME;
                    var spMerchantName = sp1.SP_MERCHANT_NAME;
                    var tmId = sp1.TM_ID.ToString();
                    var provinceName = sp1.PROVINCE_NAME;
                    var districtName = sp1.DISTRICT_NAME;
                    var wardName = sp1.WARD_NAME;
                    var spAddress = sp1.SP_ADDRESS + "," + sp1.WARD_NAME + "," + sp1.DISTRICT_NAME + "," + sp1.PROVINCE_NAME; //8
                    var spPhone = sp1.SP_PHONE;
                    var spWebsite = sp1.SP_WEBSITE;
                    var spDescription = sp1.SP_DESCRIPTION;
                    var imageUrl = "";
                    if (!string.IsNullOrEmpty(sp1.IMAGE_URL))
                    {
                        imageUrl = sp1.IMAGE_URL;
                    }
                    else
                    {
                        imageUrl = baseImg + "nophoto.png";
                    }
                    var longitude = sp1.LONGITUDE;
                    var latitude = sp1.LATITUDE;
                    var status = sp1.STATUS;
                    var createdBy = sp1.CREATED_BY;
                    var spIcon = "/Resources/website/images/pinblue.svg";
                    const string spCss = "hot";
                    string[] returnView = { spId, spName, spTmName, spMerchantName, tmId, provinceName, districtName, wardName, spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status, createdBy, spIcon, spCss };
                    return Json(
                        new
                        {
                            Success = true,
                            RspList = returnView
                        });
                }
                return Json(null);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        #region SearchType1
      [HttpPost, ValidateInput(false)]
        public ActionResult GetSearchType1(string tradeMartParent, string tradeMartChild, string spTmName, string province, string district, string txtname, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var tmId1 = -1;
                var tradeMartParent1 = 0;
                var tradeMartChild1 = 0;
                if (tradeMartParent!="0")
                {
                    tradeMartParent1 = int.Parse(tradeMartParent);
                }
                if (tradeMartChild!="0")
                {
                    tradeMartChild1 = int.Parse(tradeMartChild);
                }
                if (tradeMartParent1 > 0)
                {
                    tmId1 = tradeMartChild1 > 0 ? tradeMartChild1 : tradeMartParent1;
                }
                if (spTmName=="0")
                {
                    spTmName = null;
                }
                if (province=="0")
                {
                    province = null;
                }
                district = district=="0" ? null : district.Trim();
                if (string.IsNullOrEmpty(txtname))
                {
                    txtname = null;
                }
                var spId = "";
                var spName = "";
                var spTmName1 = "";
                var spMerchantName = "";
                var tmId = "";
                var provinceName = "";
                var districtName = "";
                var wardName = "";
                var spAddress = "";
                var spPhone = "";
                var spWebsite = "";
                var spDescription = "";
                var imageUrl = "";
                var longitude = "";
                var latitude = "";
                var status = "";
                var createdBy = "";
                var i = 0;
                double to = 0;
                double totalPages = 0;
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var listItem = Dbcontext.FRONT_SP_SEARCH_TYPE1(pageSize, pageNumber, totalRow, province, district, txtname, spTmName, tmId1).ToList();
                to = double.Parse(totalRow.Value.ToString());
                totalPages = Convert.ToInt32(Math.Ceiling(to / pageSize));
                var listMc = new Array[listItem.Count];
                i = 0;
                foreach (var item in listItem)
                {
                    spId = item.SP_ID.ToString();
                    spName = item.SP_NAME;
                    spTmName1 = item.SP_TM_NAME;
                    spMerchantName = item.SP_MERCHANT_NAME;
                    tmId = item.TM_ID.ToString();
                    provinceName = item.PROVINCE_NAME;
                    districtName = item.DISTRICT_NAME;
                    wardName = item.WARD_NAME;
                    spAddress = item.SP_ADDRESS + "," + item.WARD_NAME + "," + item.DISTRICT_NAME + "," + item.PROVINCE_NAME; //8
                    spPhone = item.SP_PHONE;
                    spWebsite = item.SP_WEBSITE;
                    spDescription = item.SP_DESCRIPTION;
                    if (!string.IsNullOrEmpty(item.IMAGE_URL))
                    {
                        imageUrl = item.IMAGE_URL;
                    }
                    else
                    {
                        imageUrl = baseImg + "nophoto.png";
                    }
                    longitude = item.LONGITUDE;
                    latitude = item.LATITUDE;
                    status = item.STATUS;
                    createdBy = item.CREATED_BY;
                    var spIcon = baseImg + "pinblue.svg";
                    const string spCss = "hot";
                    string[] returnView = { spId, spName, spTmName1, spMerchantName, tmId, provinceName, districtName, wardName, spAddress, spPhone, spWebsite, spDescription, imageUrl, longitude, latitude, status, createdBy, spIcon, spCss };
                    listMc[i] = returnView;
                    i++;
                }
                return Json(
                   new
                   {
                       Success = true,
                       TotalRow = totalPages,
                       RspList = listMc,
                       indexList = to
                   });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }
        }
        #endregion
    }
}
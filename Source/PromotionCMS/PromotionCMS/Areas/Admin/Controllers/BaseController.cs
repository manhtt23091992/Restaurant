using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using log4net;
using PromotionCMS.Models;
using PromotionCMS.Areas.Admin.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        //thangnh 20/07/2018
        // GET: /Admin/Base/
        public UserModel CurrentUser;
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected PromotionCMSEntities Dbcontext;
        public JavaScriptSerializer ser = new JavaScriptSerializer();
        public BaseController()
        {
            Dbcontext = new PromotionCMSEntities();
            ViewBag.Title = "Trang chủ";
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            CurrentUser = (UserModel)Session[CommonConstants.USER_SESSION];
            if (CurrentUser == null)
            {
                filterContext.Result = new RedirectResult(Url.Action("Login", "Users"));
            }
            base.OnActionExecuting(filterContext);
        }
        public string MyIp()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
        //protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
        //    };
        //}
        public static GeocoderLocation Locate(string query,string key)
        {
            try
            {
                Thread.Sleep(250);
                if (!string.IsNullOrEmpty(query))
                {
                    var q = HttpUtility.UrlEncode(query);
                    var url = $"https://maps.googleapis.com/maps/api/geocode/xml?address={q}&key={key}";//
                    var request = WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            var document = XDocument.Load(new StreamReader(stream));

                            var longitudeElement = document.Descendants("lng").FirstOrDefault();
                            var latitudeElement = document.Descendants("lat").FirstOrDefault();

                            if (longitudeElement != null && latitudeElement != null)
                            {
                                return new GeocoderLocation
                                {
                                    Longitude = longitudeElement.Value,
                                    Latitude = latitudeElement.Value
                                };
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return new GeocoderLocation
                {
                    Longitude = null,
                    Latitude = null
                };
            }
            return new GeocoderLocation
            {
                Longitude = null,
                Latitude = null
            };
        }
        public static GeocoderLocation GetLocateByAddress(string query)
        {
            try
            {
                if (!string.IsNullOrEmpty(query))
                {
                    var request = WebRequest
                        .Create("http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address="
                                + HttpUtility.UrlEncode(query));

                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            var document = XDocument.Load(new StreamReader(stream));

                            var longitudeElement = document.Descendants("lng").FirstOrDefault();
                            var latitudeElement = document.Descendants("lat").FirstOrDefault();

                            if (longitudeElement != null && latitudeElement != null)
                            {
                                return new GeocoderLocation
                                {
                                    Longitude = longitudeElement.Value,
                                    Latitude = latitudeElement.Value
                                };
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return new GeocoderLocation
                {
                    Longitude = null,
                    Latitude = null
                };
            }
            return new GeocoderLocation
            {
                Longitude = null,
                Latitude = null
            };
        }

    }
}
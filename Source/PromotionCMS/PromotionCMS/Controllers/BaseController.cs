using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using log4net;
using PromotionCMS.Models;

namespace PromotionCMS.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected PromotionCMSEntities Dbcontext;
        public BaseController()
        {
            Dbcontext = new PromotionCMSEntities();
            ViewBag.Title = "Trang chủ";
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
                                    Latitude =latitudeElement.Value
                                };
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
        public static GeocoderLocation GetLocateByLngLat(string latitude, string longitude)
        {
            try
            {
                var request = WebRequest
                    .Create(string.Format("http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false", latitude, longitude));
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var document = XDocument.Load(new StreamReader(stream));

                        var address = document.Descendants("result").Take(1);
                        var persons = address.Descendants("address_component").Select(x => new GeocoderAddress { LongName = x.Element("long_name").Value, ShortName = x.Element("short_name").Value, Type = x.Element("type").Value }).ToList();
                        var itemAddress = new GeocoderLocation();
                        foreach (var item in persons)
                        {
                            if (item.Type == "premise")
                            {
                                itemAddress.Address = item.LongName;
                            }
                            if (item.Type == "neighborhood")
                            {
                                itemAddress.Address = item.LongName;
                            }
                            if (item.Type == "sublocality")
                            {
                                itemAddress.Address = item.LongName;
                            }
                            if (item.Type == "route")
                            {
                                itemAddress.Address = item.LongName;
                            }
                            if (item.Type == "administrative_area_level_3")
                            {
                                itemAddress.Ward = item.LongName;
                            }
                            if (item.Type == "administrative_area_level_2")
                            {
                                itemAddress.District = item.LongName;
                            }
                            if (item.Type == "administrative_area_level_1")
                            {
                                itemAddress.Province = item.LongName;
                            }
                        }
                        return itemAddress;
                    }
                }

            }
            catch (Exception e)
            {
                return null;
            }
            //URL do distancematrix - adicionando endereco de origem e destino
            return null;
        }
    }
}
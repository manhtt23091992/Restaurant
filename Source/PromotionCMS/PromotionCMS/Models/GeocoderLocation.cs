using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Models
{
    public class GeocoderLocation
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Status { get; set; }
    }
    public class GeocoderAddress
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
    }
    public class Select
    {
        public string value;
        public string text;
        public bool selected;
    }
}
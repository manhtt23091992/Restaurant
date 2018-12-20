using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class SettingsModel
    {
        public int SetID { get; set; }
        public string SetCode { get; set; }
        public string SetKey { get; set; }
        public string SetValue { get; set; }
        public string SetNote { get; set; }
        public string SetStatus { get; set; }
    }
}
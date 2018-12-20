using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class BankModel
    {
        public int Bannerid { get; set; }
        public Guid BannerGuid { get; set; }
        public string BannerTitle { get; set; }
        public string BannerDescription { get; set; }
        public string BannerKeyword { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string LinkAndroid { get; set; }
        public string LinkIos { get; set; }
        public string Status { get; set; }
        public int Ord { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Prioritize { get; set; }
    }

    public class BankPriority
    {
        public int Id { get; set; }
        public int Ord { get; set; }
    }

}
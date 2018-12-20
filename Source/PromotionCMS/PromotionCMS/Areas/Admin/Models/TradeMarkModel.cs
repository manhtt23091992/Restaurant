using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class TradeMarkModel
    {
        public int TmId { get; set; }
        public int TmIdc { get; set; }
        public string TmName { get; set; }

        public string ImageUrl { get; set; }

        public int ParentId { get; set; }

        public string TmStatus { get; set; }

        public string Status { get; set; }

        public int Ord { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateBy { get; set; }


    }

    public class TradeMarkPriority
    {
        public int Id { get; set; }
        public int Ord { get; set; }
    }
   
   
}
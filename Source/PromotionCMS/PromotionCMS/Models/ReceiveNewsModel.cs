using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Models
{
    public class ReceiveNewsModel
    {
        public int RnId { get; set; }
        public string RnName { get; set; }
        public string RnEmail { get; set; }
        public string RnPhone { get; set; }
        public DateTime RnCreateDate { get; set; }
    }
}
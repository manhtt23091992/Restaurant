using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class CategoryModel
    {
        public int Cateid { get; set; }
        public Guid CateGuid { get; set; }
        public string CateName { get; set; }
        public string CateAlias { get; set; }
        public int ParentId { get; set; }
        public string CateDescription { get; set; }
        public string CateKeyword { get; set; }
        public string CatePermalink { get; set; }
        public string CateThumbnail { get; set; }
        public int Ord { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string CateStatus { get; set; }
    }
}
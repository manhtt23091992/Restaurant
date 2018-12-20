using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class PostModel
    {
        public int PostId { get; set; }
        public Guid PostGuid { get; set; }  
        public string PostTitle { get; set; }
        public string PostAlias { get; set; }
        public string PostDescription { get; set; }
        public string PostContent { get; set; }
        public string PostKeyword { get; set; }
        public string PostThumbnail { get; set; }
        public string PostPriority { get; set; }
        public string PostStatus { get; set; }
        public int ViewCount { get; set; }
        public int Ord { get; set; }
        public string CreatedBy { get; set; }

        public string CreatedDate { get; set; }
        public string CreatedPost { get; set; }
        public string CreatedLook { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
    public class PostPriority
    {
        public int Id { get; set; }
        public int Ord { get; set; }
    }
}
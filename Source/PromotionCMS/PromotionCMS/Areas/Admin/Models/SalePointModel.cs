using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{

    public class SalePointModel
    {
        public int SpId { get; set; }

        public string SpMerchantName { get; set; }

        public string SpTmName { get; set; }

        public int TmId { get; set; }

        public string SpName { get; set; }

        public string ProvinceName { get; set; }

        public string DistrictName { get; set; }

        public string WardName { get; set; }

        public string SpAddress { get; set; }

        public string SpPhone { get; set; }

        public string SpWebsite { get; set; }

        public string SpDescription { get; set; }

        public string ImageUrl { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public Guid SpGuid { get; set; }

        public string Status { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class ProgramModel
    {
        public string PgId { get; set; }

        public string PgName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid PgGuid { get; set; }

        public string Status { get; set; }
    }
}
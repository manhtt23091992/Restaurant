using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class Credential
    {
        [StringLength(20)]
        public string FunctionName { set; get; }

        [StringLength(50)]
        public int RoleID { set; get; }
    }
}
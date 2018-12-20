using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class RolesModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleKey { get; set; }
        public string RoleNote { get; set; }
        public int Ord { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string Status { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
    public class RolesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ReturnUrl { get; set; }
        public string Show { get; set; }
    } 
}
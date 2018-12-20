using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class UsersManagerModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Fullname { get; set; }

        public string Password { get; set; }

        public DateTime Lastlogin { get; set; }

        public int RoleId { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
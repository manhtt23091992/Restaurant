using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    [Serializable]
    public class UserModel
    {
        public string UserName { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public int? RoleId { get; set; }

        public string UserIp { get; set; }

        public DateTime LoginDate { get; set; }
    }
}
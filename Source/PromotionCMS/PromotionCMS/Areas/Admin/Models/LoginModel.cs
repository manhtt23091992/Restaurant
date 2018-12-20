using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class LoginModel
    {
        [Key]
        [Required(ErrorMessage = "Bạn phải nhập tài khoản")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Bạn phải nhập mật khẩu")]
        public string Password { set; get; }

    }
}
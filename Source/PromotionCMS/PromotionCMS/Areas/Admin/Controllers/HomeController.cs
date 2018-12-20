using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PromotionCMS.Areas.Admin.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        // thangnh 25/07/2018
        // GET: /Admin/Home/
   
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePass(FormCollection collection, UsersManagerModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentUser.UserName))
                {
                    var userid = (from u in Dbcontext.CMS_USERS where (u.USERNAME == CurrentUser.UserName) select u)
                        .FirstOrDefault();
                    if (userid != null)
                    {
                        var encryPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                        var rs = Dbcontext.PROC_CMS_USERS_UPDATE_PASS(userid.USER_ID, encryPassword);
                        if (rs < 1)
                        {
                            TempData["RspCode"] = "99";
                            return RedirectToAction("ChangePass");
                        }
                    }
                } 
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("ChangePass");
            }
            return RedirectToAction("Index","Home");
        }
    }
}

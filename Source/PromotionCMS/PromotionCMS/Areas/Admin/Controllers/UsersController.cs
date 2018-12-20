using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        readonly PromotionCMSEntities _dbcontext = new PromotionCMSEntities();

        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //thangnh 20/07/2018
        // GET: /Admin/Users/
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                var encryPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "md5");
                var result = _dbcontext.PROC_CMS_USER_LOGIN(model.UserName, encryPassword).FirstOrDefault();
                
                if (result != null)
                {
                    if (result.STATUS == "1" && result.ROLE_STATUS=="1") { 
                    var user = _dbcontext.CMS_USERS.SingleOrDefault(x => x.USERNAME == model.UserName);
                    var userSession = new UserModel();
                    if (user != null)
                    {
                        userSession.UserName = user.USERNAME;
                        userSession.UserId = user.USER_ID;
                        userSession.RoleId = user.ROLE_ID;
                    }
                    var listCredentials = GetListCredential(model.UserName);

                    Session.Add(CommonConstants.SESSION_CREDENTIALS, listCredentials);
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    _dbcontext.PROC_CMS_USER_LOG_INSERT(model.UserName, "User", "", "", MyIp(), "Login", DateTime.Now);
                    return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["RspCode"] = "99";
                        TempData["Message"] = "Tài khoản của bạn đang bị khóa";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return RedirectToAction("Login");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["Message"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                return RedirectToAction("Login");
            }
        }
        public ActionResult Logout()
        {
            var user = (UserModel)Session[CommonConstants.USER_SESSION];
            _dbcontext.PROC_CMS_USER_LOG_INSERT(user.UserName, "User", "", "", MyIp(), "Logout", DateTime.Now);
            Session[CommonConstants.USER_SESSION] = null;
            Session.Abandon();
            return RedirectToAction("Login","Users");
        }
        public List<string> GetListCredential(string userName)
        {
            var user = _dbcontext.CMS_USERS.Single(x => x.USERNAME == userName);
            var data = (from a in _dbcontext.CMS_FUNCT_ROLE
                        join b in _dbcontext.CMS_ROLES on a.ROLE_ID equals b.ROLE_ID
                        join c in _dbcontext.CMS_FUNCTIONS on a.FUN_ID equals c.FUN_ID
                where b.ROLE_ID == user.ROLE_ID
                select new
                {
                    FunctionName = c.FUN_KEY,
                    RoleID = a.ROLE_ID
                }).AsEnumerable().Select(x => new Credential()
            {
                FunctionName = x.FunctionName,
                RoleID = x.RoleID
            });
            return data.Select(x => x.FunctionName).ToList();
        }
        public string MyIp()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

    }
}
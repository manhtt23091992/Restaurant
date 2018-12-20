using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PromotionCMS.Areas.Admin.Models;
using log4net;
using PromotionCMS.Models;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class ReceiveNewsController : BaseController
    {
        private static readonly ILog log =
        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Admin/ReceiveNews
        [HasCredential(FunctionName = "VIEW_RECEIVENEWS")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ReceiveNews", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }

        [Route("receivenews/get-receivenews")]
        [HasCredential(FunctionName = "VIEW_RECEIVENEWS")]
        public JsonResult Search(string RnName, int pageNo, string table_list_length)
        {
            int list_length;
            if (string.IsNullOrEmpty(RnName))
            {
                RnName = null;
            }
            if (String.IsNullOrEmpty(table_list_length))
            {
                list_length = 10;
            }
            else
            {
                list_length = int.Parse(table_list_length);
            }
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(Int32));
                var model = Dbcontext.PROC_RECEIVE_NEWS_SEARCH(list_length, pageNo, totalRow, RnName, null);
                var rspList = model.ToList();
                return Json(
                  new
                  {
                      Success = true,
                      TotalRow = totalRow.Value,
                      ListData = rspList
                  });
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception: {0}", ex);
                return Json(
                new
                {
                    Success = false
                });
            }
        }
        #region SendMail
        //thangnh 10/08/2018
        // GET: Admin/SendMail

        public ActionResult LoadTabEmail()
        {
            try
            {
                var listTm = GeSpDataList();
                return Json(
                    new
                    {
                        Success = true,
                        RspList = listTm
                    });
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(null);
            }

        }
        public List<JsTreeModel> GeSpDataList()
        {
            try
            {
                var listData = new List<JsTreeModel>();
                var listR = (from e in Dbcontext.RECEIVE_NEWS
                             orderby e.CREATE_DATE ascending
                             select e).ToList();
                if (listR.Count <= 0) return listData;
                listData.AddRange(listR.Select(item => new JsTreeModel { text = item.RN_NAME, id = item.RN_EMAIL }));
                return listData;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HasCredential(FunctionName = "EMAIL_RECEIVENEWS")]
        public ActionResult SendMail()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EMAIL_RECEIVENEWS")]
        public ActionResult SendMail(FormCollection collection, IEnumerable<string> cemail, string subject, string content)
        {
            Log.InfoFormat("Begin Send email to: {0}", cemail);
            try
            {
                if (content == "<html>\n<head>\n\t<title></title>\n</head>\n<body>&nbsp;</body>\n</html>\n")
                {
                    return Json(new RolesResult
                    {
                        Success = false,
                        Message = "Nội dung bắt buộc nhập",
                        Show = "1"
                    }, JsonRequestBehavior.AllowGet);
                }
                if (cemail == null)
                {
                    return Json(new RolesResult
                    {
                        Success = false,
                        Message = "Bạn chưa chọn người nhận",
                        Show = "0"
                    }, JsonRequestBehavior.AllowGet);
                }
                foreach (var item in cemail)
                {
                    var mail = new MailMessage();
                    var smtpServer = new SmtpClient("smtp.gmail.com");
                    var from = ConfigurationManager.AppSettings["FROM_MAIL"];
                    mail.From = new MailAddress(from);
                    mail.To.Add(item);
                    mail.Subject = subject;
                    mail.Body = content;
                    mail.IsBodyHtml = true;
                    smtpServer.Port = 587;
                    smtpServer.Credentials = new System.Net.NetworkCredential("no-reply@vnpay.vn",
                        ConfigurationManager.AppSettings["FROM_PASS"]);
                    smtpServer.EnableSsl = true;
                    smtpServer.Send(mail);
                    Dbcontext.PROC_EMAIL_LOG_INSERT(item, subject, content, "1");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Send email Thread exception: {0} {1}", ex.Message, ex);
                return Json(new RolesResult
                {
                    Success = false,
                    Message = "Xảy ra lỗi trong quá trình gửi email:" + ex.Message,
                    ReturnUrl = Url.Action("SendMail")
                }, JsonRequestBehavior.AllowGet);
            }
            var dataLog = ser.Serialize(cemail);
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ReceiveNews", "", dataLog, MyIp(), "Gửi Mail", DateTime.Now);
            return Json(new RolesResult
            {
                Success = true,
                ReturnUrl = Url.Action("Index")
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [Route("receivenews/showpopup")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Details(int id)
        {
            var model = (from rn in Dbcontext.EMAIL_LOG where (rn.EMAIL_LOG_ID == id) select rn).FirstOrDefault();
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ReceiveNews", "", id.ToString(), MyIp(), "Xem chi tiết", DateTime.Now);
            return PartialView("~/Areas/Admin/Views/ReceiveNews/_Details.cshtml", model);
        }
        [HasCredential(FunctionName = "VIEW_LOG_EMAIL")]
        public ActionResult ListSendMail()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "ReceiveNews", "", "", MyIp(), "List Log Email", DateTime.Now);
            return View();
        }
        [Route("receivenews/get-logemail")]
        [HasCredential(FunctionName = "VIEW_LOG_EMAIL")]
        public JsonResult SearchListEmail(string toEmail, string emailStatus, int pageNo, string table_list_length)
        {
            if (string.IsNullOrEmpty(toEmail))
            {
                toEmail = null;
            }
            if (string.IsNullOrEmpty(emailStatus))
            {
                emailStatus = null;
            }
            var listLength = String.IsNullOrEmpty(table_list_length) ? 10 : int.Parse(table_list_length);
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(Int32));
                var model = Dbcontext.PROC_EMAIL_LOG_SEARCH(toEmail, emailStatus, listLength, pageNo, totalRow);
                var rspList = model.ToList();
                return Json(
                  new
                  {
                      Success = true,
                      TotalRow = totalRow.Value,
                      ListData1 = rspList
                  });
            }
            catch (Exception ex)
            {
                log.InfoFormat("Exception: {0}", ex);
                return Json(
                new
                {
                    Success = false
                });
            }
        }

        public ActionResult ExportToExcel()
        {
            var products = new System.Data.DataTable("RECEIVE_NEWS");
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Email", typeof(string));
            products.Columns.Add("Phone", typeof(string));
            products.Columns.Add("Information source", typeof(string));
            products.Columns.Add("Create date", typeof(string));

            var list = (from rn in Dbcontext.RECEIVE_NEWS select rn).ToList();
            foreach (var item in list)
            {
                products.Rows.Add(item.RN_NAME, item.RN_EMAIL, item.RN_PHONE, item.INFOMATION_SOURCE, $"{item.CREATE_DATE:dd-MM-yyyy HH:mm:ss}");
            }

            var grid = new GridView {DataSource = products};
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=RECEIVE_NEWS.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index", "ReceiveNews");
        }
    }
}

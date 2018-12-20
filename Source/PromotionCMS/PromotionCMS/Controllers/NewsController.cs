using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Controllers;

namespace PromotionCMS.Controllers
{
    public class NewsController : BaseController
    {
        public string BaseNews = ConfigurationManager.AppSettings["baseNews"];
        // thangnh 
        // GET: /NewPromotion/
        public ActionResult Promotion()
        {

            return View();
        }
        [Route("get-post")]
        public ActionResult Listpost(int pageNo)
        {
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(Int32));
                var model = Dbcontext.FRONT_POST_ALL("1", 8, pageNo, totalRow);
                var rspList = model != null ? model.ToList() : null;
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
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(
                    new
                    {
                        Success = false
                    });
            }
        }
        //
        // GET: /NewPromotion/Details/5
        public ActionResult Details(string postAlias)
        {
            var alias = postAlias;
            var details = (from news in Dbcontext.POSTs where (news.POST_ALIAS == alias) && (news.POST_STATUS == "1") select news).FirstOrDefault();
            var newHot = (from news in Dbcontext.POSTs where (news.POST_ALIAS != alias) && (news.POST_STATUS == "1") orderby news.POST_PRIOTITY descending, news.ORD ascending select news).Take(5).ToList();
            ViewBag.detailsNews = details;
            ViewBag.newHot = newHot;
            ViewBag.baseUrl = BaseNews;
            return View();
        }


    }
}

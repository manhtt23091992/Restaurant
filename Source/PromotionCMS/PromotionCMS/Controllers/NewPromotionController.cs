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
    public class NewPromotionController : BaseController
    {
        //
        // GET: /NewPromotion/
        public ActionResult Index()
        {
            
            return View();
        }
        [Route("get-post")]
        public ActionResult Listpost(int pageNo)
        {
            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(Int32));
                var model = Dbcontext.FRONT_POST_ALL("1",8,pageNo,totalRow);
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
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /NewPromotion/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NewPromotion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NewPromotion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /NewPromotion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /NewPromotion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /NewPromotion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

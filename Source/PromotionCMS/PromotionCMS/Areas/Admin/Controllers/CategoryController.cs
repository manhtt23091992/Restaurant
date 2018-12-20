using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Library;
using PromotionCMS.Models;
using System.Data.Entity.Core.Objects;
using PromotionCMS;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class CategoryController : BaseController
    {
        //Thangnh 29/06/2018
        // GET: /Admin/Category/
        private int _level = -1;
        StringBuilder _sb = new StringBuilder();
        readonly List<SelectListItem> _items = new List<SelectListItem>();
        [HasCredential(FunctionName = "VIEW_CATEGORY")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "CATEGORY", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }
        [Route("category/get-cate-list")]
        [HasCredential(FunctionName = "VIEW_CATEGORY")]
        public JsonResult Search(string CateName, string CateStatus, int pageNo, string table_list_length)
        {
            int list_length;
            if (String.IsNullOrEmpty(CateName))
            {
                CateName = null;
            }
            if (String.IsNullOrEmpty(CateStatus))
            {
                CateStatus = null;
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
                var model = Dbcontext.PROC_CATEGORY_SEARCH(list_length, pageNo, totalRow, CateName, CateStatus);
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
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return Json(
                  new
                  {
                      Success = false
                  });
            }
        }

        #region DropdownListParent
        public JsonResult Listparent()
        {
            var lstItem1 =
                (from item in Dbcontext.CATEGORies where (item.CATE_STATUS == "1") select item)
                .ToList();

            _items.Add(new SelectListItem { Text = "-- All --", Value = "", Selected = true });
            RecursiveFillTree(lstItem1, 0);
            IEnumerable<SelectListItem> ls = _items;
            return Json(new SelectList(ls, "Value", "Text"));
        }
        private void RecursiveFillTree(IEnumerable<CATEGORY> lstItem, int parentId)
        {
            _level += 1;
            var appender = new StringBuilder();

            for (var j = 0; j <= _level - 1; j++)
            {
                appender.Append("&nbsp;&nbsp;&nbsp;");
            }
            if (_level > 0)
            {
                appender.Append("|----");
            }
            var systemCategorys = lstItem as CATEGORY[] ?? lstItem.ToArray();
            var lstChild = systemCategorys.Where(p => p.PARENT_ID == parentId).ToList();
            if (lstChild.Count > 0)
            {

                for (var i = 0; i <= lstChild.Count - 1; i++)
                {
                    _items.Add(new SelectListItem { Text = appender + lstChild[i].CATE_NAME, Value = lstChild[i].CATE_ID.ToString() });
                    RecursiveFillTree(systemCategorys, lstChild[i].CATE_ID);
                }
            }
            _level -= 1;
        }
        #endregion

        //Thangnh 29/06/2018
        // GET: /Admin/Category/Create
        [HasCredential(FunctionName = "ADD_CATEGORY")]
        public ActionResult Create()
        {
            return View();
        }

        //Thangnh 29/06/2018
        // POST: /Admin/Category/Create
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_CATEGORY")]
        public ActionResult Create(FormCollection collection, CategoryModel model)
        {
            try
            {
                var alias = Utils.GetAlias(model.CateName);
                Dbcontext.PROC_CATEGORY_INSERT(model.CateName, alias, model.ParentId, model.CateDescription,
                    model.CateKeyword, model.CatePermalink, model.CateThumbnail, Byte.Parse(model.Ord.ToString()),
                    CurrentUser.UserName, model.CateStatus);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Thêm mới danh mục thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "CATEGORY", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Create");
            }
        }

        //
        // GET: /Admin/Category/Edit/5
        [HasCredential(FunctionName = "EDIT_CATEGORY")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = Dbcontext.PROC_CATEGORY_SELECTBYID(id).FirstOrDefault();
                if (model != null && model.CATE_ID > 0)
                {
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Index");
            }

        }

        //
        // POST: /Admin/Category/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_CATEGORY")]
        public ActionResult Edit(FormCollection collection, CategoryModel model)
        {
            try
            {
                Dbcontext.PROC_CATEGORY_UPDATE(model.Cateid, model.CateName, model.CateAlias, model.ParentId, model.CateDescription, model.CateKeyword, model.CatePermalink, model.CateThumbnail, Byte.Parse(model.Ord.ToString()), CurrentUser.UserName, model.CateStatus);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Cập nhật danh mục thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "CATEGORY", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return RedirectToAction("Edit", new { id = model.Cateid });
            }

        }
        [Route("category/update-status")]
        [HasCredential(FunctionName = "EDIT_CATEGORY")]
        public JsonResult UpdateStatus(int cateId, string status)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = status == "1" ? "0" : "1";
                var rs = Dbcontext.PROC_CATEGORY_UPDATE_STATUS(cateId, statusUpdate);
                  var dataLog = ser.Serialize(status);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "CATEGORY", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Danh mục cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho danh mục thành công";
                }
                else
                {
                    message = "Đã xảy ra lỗi trong quá trình cập nhật. Vui lòng thực hiện lại sau";
                }
                return Json(
                  new
                  {
                      Success = true,
                      RspCode = rspCode,
                      Message = message
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


    }
}

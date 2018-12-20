using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PromotionCMS.Areas.Admin.Models;
using PromotionCMS.Library;

namespace PromotionCMS.Areas.Admin.Controllers
{
    public class PostController : BaseController
    {
        //thangnh 10/07/2018
        [Route("post/get-post-list")]
        [HasCredential(FunctionName = "VIEW_POST")]
        public JsonResult Search(string PostTitle, string PostStatus, string txtFromDate, string txtToDate, int pageNo, string tableListLength)
        {
            if (string.IsNullOrEmpty(PostTitle))
            {
                PostTitle = null;
            }
            if (string.IsNullOrEmpty(PostStatus))
            {
                PostStatus = null;
            }
            var fTo = !string.IsNullOrEmpty(txtFromDate)
                ? DateTime.ParseExact(txtFromDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00", "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            var cTo = !string.IsNullOrEmpty(txtToDate)
                ? DateTime.ParseExact(txtToDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture)
                : DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59", "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
            var listLength = String.IsNullOrEmpty(tableListLength) ? 10 : int.Parse(tableListLength);

            try
            {
                var totalRow = new ObjectParameter("P_TOTALROW", typeof(int));
                var model = Dbcontext.PROC_POST_SEARCH(PostTitle, PostStatus, fTo, cTo, listLength, pageNo,
                    totalRow);
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
                        TotalRow = totalRow.Value,
                        RspList = rspList
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
        // GET: /Admin/Post/
        [HasCredential(FunctionName = "VIEW_POST")]
        public ActionResult Index()
        {
            Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", "", MyIp(), "Tìm Kiếm", DateTime.Now);
            return View();
        }

        //
        // GET: /Admin/Post/Details/5
        [HasCredential(FunctionName = "DETAILS_POST")]
        public ActionResult Details(int id)
        {
            try
            {
                var model = (from p in Dbcontext.POSTs where p.POST_ID == id select p).FirstOrDefault();
                if (model != null)
                {
                    Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", "", MyIp(), "Xem chi tiết", DateTime.Now);
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Admin/Post/Create
        [HasCredential(FunctionName = "ADD_POST")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Admin/Post/Create
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "ADD_POST")]
        public ActionResult Create(FormCollection collection, PostModel model)
        {
            try
            {
                var getDate = DateTime.Now;
                var status = 0;
                DateTime? fTo;
                DateTime? cTo;
                if (!string.IsNullOrEmpty(model.CreatedPost))
                {
                    fTo = DateTime.ParseExact(model.CreatedPost, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    fTo = null;
                }
                if (!string.IsNullOrEmpty(model.CreatedLook))
                {
                    cTo = DateTime.ParseExact(model.CreatedLook, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    cTo = null;
                }
                if (fTo != null && cTo != null)
                {
                    var from = DateTime.ParseExact(model.CreatedPost, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    var to = DateTime.ParseExact(model.CreatedLook, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    var compare = DateTime.Compare(getDate, from);
                    var compare1 = DateTime.Compare(getDate, to);
                    if (compare == 0 || compare > 0)
                    {
                        status = 1;
                    }
                    if (compare1 == 0 || compare1 > 0)
                    {
                        status = 0;
                    }
                    if (compare < 0)
                    {
                        status = 2;
                    }
                }
                else
                {
                    status = 1;
                }
                var alias = Utils.GetAlias(model.PostTitle);
               // model.PostThumbnail = System.Net.WebUtility.UrlDecode(model.PostThumbnail).ToString();
                Dbcontext.PROC_POST_INSERT(model.PostTitle, alias, model.PostDescription, model.PostContent, model.PostKeyword, model.PostThumbnail, status.ToString(), 100, model.Ord, CurrentUser.UserName, fTo, cTo);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Thêm mới tin tức khuyến mại thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Thêm mới", DateTime.Now);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Create");
            }
        }

        [Route("post/update-status")]
        [HasCredential(FunctionName = "EDIT_POST")]
        public JsonResult UpdateStatus(string postId, string postStatus)
        {
            try
            {
                var rspCode = "";
                var statusUpdate = "";
                if (postStatus == "2")
                {
                    statusUpdate = "1";
                }
                else
                {
                    statusUpdate = postStatus == "1" ? "0" : "1";
                }
                var rs = Dbcontext.PROC_POST_UPDATE_STATUS(postId, statusUpdate);
                var dataLog = ser.Serialize(postStatus);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Update Status", DateTime.Now);
                rspCode = rs < 1 ? "99" : "00";
                var message = "";
                if (rspCode == "00")
                {
                    message = statusUpdate == "1" ? "Tin tức khuyến mại đã cập nhật trạng thái hoạt động thành công" : "Cập nhật trạng thái tạm khóa cho tin tức khuyến mại thành công";
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
        //
        // GET: /Admin/Post/Edit/5
        [HasCredential(FunctionName = "EDIT_POST")]
        public ActionResult Edit(int id)
        {
            try
            {
                var model = (from p in Dbcontext.POSTs where p.POST_ID == id select p).FirstOrDefault();
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    TempData["RspCode"] = "99";
                    TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Admin/Post/Edit/5
        [HttpPost, ValidateInput(false)]
        [HasCredential(FunctionName = "EDIT_POST")]
        public ActionResult Edit(FormCollection collection, PostModel model)
        {
            try
            {
                var getDate = DateTime.Now;
                var status = 0;
                DateTime? fTo;
                DateTime? cTo;
                var createTo = DateTime.ParseExact(model.CreatedDate, "dd-MM-yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(model.CreatedPost))
                {
                    fTo = DateTime.ParseExact(model.CreatedPost, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    fTo = null;
                }
                if (!string.IsNullOrEmpty(model.CreatedLook))
                {
                    cTo = DateTime.ParseExact(model.CreatedLook, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                }
                else
                {
                    cTo = null;
                }
                if (fTo != null && cTo != null)
                {
                    var from = DateTime.ParseExact(model.CreatedPost, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);
                    var to = DateTime.ParseExact(model.CreatedLook, "dd-MM-yyyy HH:mm:ss",
                        CultureInfo.InvariantCulture);

                    var compare = DateTime.Compare(getDate, from);
                    var compare1 = DateTime.Compare(getDate, to);
                    if (compare == 0 || compare > 0)
                    {
                        status = 1;
                    }
                    if (compare1 == 0 || compare1 > 0)
                    {
                        status = 0;
                    }
                    if (compare < 0)
                    {
                        status = 2;
                    }
                }
                else
                {
                    status = 1;
                }
                var alias = Utils.GetAlias(model.PostTitle);
                Dbcontext.PROC_POST_UPDATE(model.PostId, model.PostTitle, alias, model.PostDescription, model.PostContent, model.PostKeyword, model.PostThumbnail, createTo, fTo, cTo, status.ToString(), CurrentUser.UserName);
                TempData["RspCode"] = "00";
                TempData["Message"] = "Cập nhật thông tin khuyến mại thành công";
                var dataLog = ser.Serialize(model);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Chỉnh sửa", DateTime.Now);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                TempData["Message"] = "Kết nối bị gián đoạn. Vui lòng thử lại";
                return RedirectToAction("Edit");
            }
        }

        [HasCredential(FunctionName = "PRIORITY_POST")]
        public ActionResult Priority()
        {
            return View();
        }
        [Route("post/update-priority-ord")]
        [HasCredential(FunctionName = "PRIORITY_POST")]
        public JsonResult PriorityUpdateOrd(IEnumerable<PostPriority> postPriority)
        {
            try
            {
                if (postPriority == null)
                    return Json(new { success = true, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
                var enumerable = postPriority.ToList();
                foreach (var itemid in enumerable)
                {
                    var i = int.Parse(itemid.Id.ToString());
                    var v = int.Parse(itemid.Ord.ToString());
                    var checkitem =
                        (from newpm in Dbcontext.POSTs
                         where (newpm.POST_ID == i)
                         select newpm).FirstOrDefault();
                    if (checkitem != null && checkitem.POST_ID > 0)
                    {
                        Dbcontext.PROC_POST_UPDATEORDBYID(i, v);
                    }
                }
                var dataLog = ser.Serialize(postPriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);

                return Json(new { success = true, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                TempData["RspCode"] = "99";
                return Json(new { success = false, responseText = Url.Action("Index") }, JsonRequestBehavior.AllowGet);
            }

        }

        [Route("post/update-priority")]
        [HasCredential(FunctionName = "PRIORITY_POST")]
        public ActionResult UpdatePriority(IEnumerable<string> postpriority)
        {
            try
            {
                var success1 = false;
                foreach (var item in postpriority)
                {
                    var i = int.Parse(item);
                    var o = 0;
                    var checkitem = (from p in Dbcontext.POSTs where (p.POST_PRIOTITY == "0") && (p.POST_ID == i) select p).FirstOrDefault();
                    if (checkitem != null && checkitem.POST_ID > 0)
                    {
                        var orl = (from p1 in Dbcontext.POSTs orderby p1.ORD descending select p1.ORD).Take(1).FirstOrDefault();
                        if (!string.IsNullOrEmpty(orl.ToString()))
                        {
                            o = int.Parse(orl.ToString()) + 1;
                        }
                        Dbcontext.PROC_POST_UPDATEBYID(i, "1", o);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }

                }
                var dataLog = ser.Serialize(postpriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [Route("post/update-nopriority")]
        [HasCredential(FunctionName = "PRIORITY_POST")]
        public ActionResult UpdateNoPriority(IEnumerable<string> postnopriority)
        {
            try
            {
                var success1 = false;
                foreach (var item in postnopriority)
                {
                    var i = int.Parse(item);
                    var checkitemno = (from post in Dbcontext.POSTs where (post.POST_PRIOTITY == "1") && (post.POST_ID == i) select post).FirstOrDefault();
                    if (checkitemno != null && checkitemno.POST_ID > 0)
                    {
                        Dbcontext.PROC_POST_UPDATEBYID(i, "0", 0);
                        success1 = true;
                    }
                    else
                    {
                        success1 = false;
                    }
                }
                var dataLog = ser.Serialize(postnopriority);
                Dbcontext.PROC_CMS_USER_LOG_INSERT(CurrentUser.UserName, "POST", "", dataLog, MyIp(), "Phân quyền ưu tiên", DateTime.Now);
                return Json(new { success = success1, responseText = "Bạn đang ở quyền ko ưu tiên" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error: {0}, stack: {1}", ex.Message, ex.StackTrace);
                return RedirectToAction("Priority");
            }

        }
        [Route("post/get-NoPriotity")]
        [HasCredential(FunctionName = "PRIORITY_POST")]
        public JsonResult GetNoPriotity()
        {
            try
            {
                var model = (from pri in Dbcontext.POSTs where (pri.POST_PRIOTITY == "0") orderby pri.POST_TITLE select pri).ToList();
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
                        ListData1 = rspList
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
        [Route("post/get-Priotity")]
        [HasCredential(FunctionName = "PRIORITY_POST")]
        public JsonResult GetPriotity()
        {
            try
            {
                var model = (from pri in Dbcontext.POSTs where (pri.POST_PRIOTITY == "1") orderby pri.ORD select pri).ToList();
                var rspList = model.ToList();
                return Json(
                    new
                    {
                        Success = true,
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
    }
}

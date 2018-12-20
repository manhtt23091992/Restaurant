using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PromotionCMS.Areas.Admin.Models
{
    public class DropdownModel
    {

        public SelectList BindStatus(string status)
        {
            var t = new SelectList(new[]
            {
                new{ID="",Name="-- All --"},
                new{ID="1",Name="Hoạt động"},
                new{ID="0",Name="Tạm khóa"}
            },
                "ID", "Name", status);
            return t;
        }
    }
    public class Select2
    {
        public string id;
        public string text;
        public bool selected;
        public List<Select2> children;
    }
}
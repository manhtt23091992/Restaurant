using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PromotionCMS.Areas.Admin.Models
{
    public class JsTreeModel
    {
        public string id;
        public string text;
        public string icon;
        public JsTreeAttribute state;
        public JsTreeAttr a_attr;
        public List<JsTreeModel> children;
    }
    public class JsTreeAttribute
    {
        public bool opened;
        public bool selected;
        public bool disabled;
        public bool undetermined;
    }
     public class JsTreeAttr
    {
        public string @class;
    }

   
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PromotionCMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class POST
    {
        public int POST_ID { get; set; }
        public System.Guid POST_GUID { get; set; }
        public string POST_TITLE { get; set; }
        public string POST_ALIAS { get; set; }
        public string POST_DESCRIPTION { get; set; }
        public string POST_CONTENT { get; set; }
        public string POST_KEYWORD { get; set; }
        public string POST_THUMBNAIL { get; set; }
        public string POST_PRIOTITY { get; set; }
        public string POST_STATUS { get; set; }
        public Nullable<int> VIEW_COUNT { get; set; }
        public Nullable<int> ORD { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public Nullable<System.DateTime> CREATE_POST { get; set; }
        public Nullable<System.DateTime> CREATE_LOOK { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
    }
}

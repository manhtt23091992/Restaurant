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
    
    public partial class PROC_CATEGORY_SEARCH_Result
    {
        public int CATE_ID { get; set; }
        public System.Guid CATE_GUID { get; set; }
        public string CATE_NAME { get; set; }
        public string CATE_ALIAS { get; set; }
        public Nullable<int> PARENT_ID { get; set; }
        public string CATE_DESCRIPTION { get; set; }
        public string CATE_KEYWORD { get; set; }
        public string CATE_PERMALINK { get; set; }
        public string CATE_THUMBNAIL { get; set; }
        public Nullable<byte> ORD { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public string CATE_STATUS { get; set; }
        public string PARENT_NAME { get; set; }
        public Nullable<long> ROWNUMBER { get; set; }
    }
}

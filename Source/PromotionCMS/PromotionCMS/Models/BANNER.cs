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
    
    public partial class BANNER
    {
        public int BANNER_ID { get; set; }
        public System.Guid BANNER_GUID { get; set; }
        public string BANNER_TITLE { get; set; }
        public string BANNER_DESCRIPTION { get; set; }
        public string BANNER_KEYWORD { get; set; }
        public string IMAGE_URL { get; set; }
        public string LINK { get; set; }
        public string LINK_ANDROID { get; set; }
        public string LINK_IOS { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public Nullable<int> ORD { get; set; }
        public string PRIORITIZE { get; set; }
    }
}

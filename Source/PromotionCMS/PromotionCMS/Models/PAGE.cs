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
    
    public partial class PAGE
    {
        public int PAGE_ID { get; set; }
        public System.Guid PAGE_GUID { get; set; }
        public string PAGE_TITLE { get; set; }
        public string PAGE_DESCRIPTION { get; set; }
        public Nullable<int> PAGE_GROUP { get; set; }
        public Nullable<int> PAGE_PARENTID { get; set; }
        public string PAGE_LINK { get; set; }
        public Nullable<int> ORD { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public string PAGE_ALIAS { get; set; }
    }
}

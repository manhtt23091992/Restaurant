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
    
    public partial class RELATION
    {
        public int RELATION_ID { get; set; }
        public string RELATION_TYPE { get; set; }
        public System.Guid RELATION_DATA_ID { get; set; }
        public System.Guid RELATION_RELATED_ID { get; set; }
        public string STATUS { get; set; }
    }
}
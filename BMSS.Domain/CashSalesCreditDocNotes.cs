//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMSS.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class CashSalesCreditDocNotes
    {
        public long DocEntry { get; set; }
        public int LineNum { get; set; }
        public string Note { get; set; }
        public string TStamp { get; set; }
    
        public virtual CashSalesCreditDocH CashSalesCreditDocH { get; set; }
    }
}

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
    
    public partial class SQDocH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SQDocH()
        {
            this.SQDocNotes = new HashSet<SQDocNotes>();
            this.SQDocLs = new HashSet<SQDocLs>();
        }
    
        public long DocEntry { get; set; }
        public string DocNum { get; set; }
        public System.DateTime DocDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public short Status { get; set; }
        public short PrintedStatus { get; set; }
        public string CopiedDO { get; set; }
        public string CustomerRef { get; set; }
        public string DeliveryTime { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public short PaymentTerm { get; set; }
        public string PaymentTermName { get; set; }
        public string Currency { get; set; }
        public decimal ExRate { get; set; }
        public string OfficeTelNo { get; set; }
        public string Fax { get; set; }
        public Nullable<int> SlpCode { get; set; }
        public string SlpName { get; set; }
        public string HeaderRemarks1 { get; set; }
        public string HeaderRemarks2 { get; set; }
        public string HeaderRemarks3 { get; set; }
        public string HeaderRemarks4 { get; set; }
        public string FooterRemarks1 { get; set; }
        public string FooterRemarks2 { get; set; }
        public string FooterRemarks3 { get; set; }
        public string FooterRemarks4 { get; set; }
        public string ShipTo { get; set; }
        public string ShipToAddress1 { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToAddress3 { get; set; }
        public string ShipToAddress4 { get; set; }
        public string ShipToAddress5 { get; set; }
        public string BillTo { get; set; }
        public string BillToAddress1 { get; set; }
        public string BillToAddress2 { get; set; }
        public string BillToAddress3 { get; set; }
        public string BillToAddress4 { get; set; }
        public string BillToAddress5 { get; set; }
        public bool SelfCollect { get; set; }
        public string SelfCollectRemarks1 { get; set; }
        public string SelfCollectRemarks2 { get; set; }
        public string SelfCollectRemarks3 { get; set; }
        public string SelfCollectRemarks4 { get; set; }
        public string SelfCollectRemarks5 { get; set; }
        public bool DiscByPercent { get; set; }
        public decimal DiscPercent { get; set; }
        public decimal DiscAmount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal GstTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<System.DateTime> PrintedOn { get; set; }
        public string PrintedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SQDocNotes> SQDocNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SQDocLs> SQDocLs { get; set; }
    }
}

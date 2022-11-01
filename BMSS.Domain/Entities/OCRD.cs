using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMSS.Domain.Entities
{
    [Table("OCRD")]
    public class OCRD
    {
        public OCRD()
        {
            this.ARInvoices = new HashSet<OINV>();
            this.ARMemos = new HashSet<ORIN>();
            this.APInvoices = new HashSet<OPCH>();
            this.APMemos = new HashSet<ORPC>();
            this.Transactions = new HashSet<OINM>();
            this.CustomerAddress = new HashSet<CRD1>();
        }
        [Key]
        [Display(Name = "Customer Code")]
        public string CardCode { get; set; }
        [Display(Name = "Customer Name")]
        public string CardName { get; set; }
        public string CardType { get; set; }
        public Int16 GroupCode { get; set; }
        [Display(Name = "Credit Limit")]
        public decimal CreditLine { get; set; }
        public Int32 SlpCode { get; set; }
        public string frozenFor { get; set; }
        public Nullable<System.DateTime> validTo { get; set; }
        public string Phone1 { get; set; }
        public string CntctPrsn { get; set; }
        public string Currency { get; set; }
        public Int16 GroupNum { get; set; }   
        public string BillToDef { get; set; }
        public string ShipToDef { get; set; }
        public string ECVatGroup { get; set; }
        public decimal Balance { get; set; }
        public string StreetNo { get; set; }
        public string Block { get; set; }
        public string City { get; set; }
        public string County { get; set; }        
        public string E_Mail { get; set; }
        public string phone1 { get; set; }
        public string Fax { get; set; }
        public string cellular { get; set; }
        public string U_paidupcap { get; set; }
        public Int16 ListNum { get; set; }
         
        public virtual OCRG CustomerGroup { get; set; }
        public virtual OSLP SalesPerson { get; set; }
        public virtual OCTG PaymentTerm { get; set; }
        public virtual ICollection<OPOR> PurchaseOrders { get; set; }
        public virtual ICollection<OINV> ARInvoices { get; set; }
        public virtual ICollection<ORIN> ARMemos { get; set; }
        public virtual ICollection<OPCH> APInvoices { get; set; }
        public virtual ICollection<ORPC> APMemos { get; set; }
        public virtual ICollection<OINM> Transactions { get; set; }
        public virtual ICollection<CRD1> CustomerAddress { get; set; }
        public virtual OPLN Pricelist { get; set; }
    }
}

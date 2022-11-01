using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.PQViewModels
{
    public class PQViewModel
    {
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }
        [JsonProperty(PropertyName = "currentUserIsNotInRoleNotes")]
        public bool CurrentUserIsNotInRoleNotes { get; set; } = true;
        public long DocEntry { get; set; }

        [JsonProperty(PropertyName = "docNum")]
        [Display(Name = "Doc No")]
        [MaxLength(50)]
        public string DocNum { get; set; }
        [ValidSupplierCode(ErrorMessage = "field is invalid", ErrorMessageResourceName = "Customer Code")]
        [JsonProperty(PropertyName = "cardCode")]
        [Required]
        [Display(Name = "Supplier Code")]
        public string CardCode { get; set; }

        [JsonProperty(PropertyName = "cardName")]
        [Required]
        [Display(Name = "Company Name")]
        public string CardName { get; set; }

        [JsonProperty(PropertyName = "docDate")]
        [Display(Name = "Doc Date")]
        [Required]
        [ValidDateFormat(ErrorMessageResourceName = "Doc Date", DateFormat = "dd/MM/yyyy", ErrorMessage = "field is invalid", ShouldGTEToday = false)]
        public string DocDate { get; set; }

        //[JsonProperty(PropertyName = "dueDate")]
        //[Display(Name = "Due Date")]
        //[Required]
        //[ValidDateFormat(ErrorMessageResourceName = "Due Date", DateFormat = "dd/MM/yyyy", ErrorMessage = "field is invalid", ShouldGTEToday = false)]
        //public string DueDate { get; set; }

        [JsonProperty(PropertyName = "deliveryDate")]
        [Display(Name = "Delivery Date")]        
        [ValidDateFormat(ErrorMessageResourceName = "Delivery Date", DateFormat = "dd/MM/yyyy", ErrorMessage = "field is invalid", ShouldGTEToday = true)]
        public string DeliveryDate { get; set; }
        [MaxLength(20)]
        [JsonProperty(PropertyName = "officeTelNo")]
        [Display(Name = "Office Tel No")]
        public string OfficeTelNo { get; set; }
        [MaxLength(20)]
        [JsonProperty(PropertyName = "fax")]
        [Display(Name = "Office Fax No")]
        public string Fax { get; set; }
        [MaxLength(20)]
        [JsonProperty(PropertyName = "deliveryTime")]
        [Display(Name = "Delivery Time")]
        public string DeliveryTime { get; set; }

        [JsonProperty(PropertyName = "status")]
        [Display(Name = "Status")]
        public short Status { get; set; }
        [JsonProperty(PropertyName = "printedStatus")]
        [Display(Name = "Printed Status")]
         
        public short PrintedStatus { get; set; }
        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
        [JsonProperty(PropertyName = "exRate")]
        [Required]
        [Display(Name = "Exchange Rate")]
        [RegularExpression(@"^\d+(\.\d{1,6})?$", ErrorMessage = "Exchange Rate field is invalid")]
        [Range(0, 9999999999999999.99, ErrorMessage = "Exchange Rate field is invalid")]
        public decimal ExRate { get; set; }
        //[JsonProperty(PropertyName = "paymentTerm")]
        //[Display(Name = "Payment Terms")]

        //public short PaymentTerm { get; set; }
        //[Display(Name = "Payment Terms")]
        //[JsonProperty(PropertyName = "paymentTermName")]
        //public string PaymentTermName { get; set; }

        [JsonProperty(PropertyName = "slpCode")]
        [Display(Name = "Sales Person")]

        public Int32 SlpCode { get; set; }
        [Display(Name = "Supplier Sales Person")]
        [JsonProperty(PropertyName = "slpName")]
        [MaxLength(155)]
        public string SlpName { get; set; }

        [JsonProperty(PropertyName = "createdOn")]
        [Display(Name = "Created On")]
        [ReadOnly(true)]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        [Display(Name = "Last Updated By")]
        public string UpdatedBy { get; set; }

        [JsonProperty(PropertyName = "updatedOn")]
        [Display(Name = "Last Updated On")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "copiedPO")]
        [Display(Name = "Copied to PO")]
        [ReadOnly(true)]
        public string CopiedPO { get; set; }

        [JsonProperty(PropertyName = "ref")]
        [Display(Name = "Ref")]
        [MaxLength(20)]
        public string Ref { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "headerRemarks1")]
        public string HeaderRemarks1 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "headerRemarks2")]
        public string HeaderRemarks2 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "headerRemarks3")]
        public string HeaderRemarks3 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "headerRemarks4")]
        public string HeaderRemarks4 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "footerRemarks1")]
        public string FooterRemarks1 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "footerRemarks2")]
        public string FooterRemarks2 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "footerRemarks3")]
        public string FooterRemarks3 { get; set; }
        [MaxLength(100)]
        [JsonProperty(PropertyName = "footerRemarks4")]
        public string FooterRemarks4 { get; set; }
        [JsonProperty(PropertyName = "netTotal")]
        [Display(Name = "Net Total")]
        public decimal NetTotal { get; set; }
        [JsonProperty(PropertyName = "shipTo")]
        [MaxLength(50)]
        [Required]
        [Display(Name = "Ship To")]
        public string ShipTo { get; set; }
        [JsonProperty(PropertyName = "shipToAddress1")]
        [MaxLength(34)]
        [Display(Name = "Address Line 1")]
        public string ShipToAddress1 { get; set; }
        [JsonProperty(PropertyName = "shipToAddress2")]
        [MaxLength(34)]
        [Display(Name = "Address Line 2")]
        public string ShipToAddress2 { get; set; }
        [JsonProperty(PropertyName = "shipToAddress3")]
        [MaxLength(34)]
        [Display(Name = "Address Line 3")]
        public string ShipToAddress3 { get; set; }
        [JsonProperty(PropertyName = "shipToAddress4")]
        [MaxLength(34)]
        [Display(Name = "Address Line 4")]
        public string ShipToAddress4 { get; set; }
        [JsonProperty(PropertyName = "shipToAddress5")]
        [MaxLength(34)]
        [Display(Name = "Address Line 5")]
        public string ShipToAddress5 { get; set; }
        [JsonProperty(PropertyName = "billTo")]
        [MaxLength(50)]
        [Display(Name = "Bill To")]
        public string BillTo { get; set; }
        [JsonProperty(PropertyName = "billToAddress1")]
        [MaxLength(50)]
        [Display(Name = "Address Line 1")]
        public string BillToAddress1 { get; set; }
        [JsonProperty(PropertyName = "billToAddress2")]
        [MaxLength(50)]
        [Display(Name = "Address Line 2")]
        public string BillToAddress2 { get; set; }
        [JsonProperty(PropertyName = "billToAddress3")]
        [MaxLength(50)]
        [Display(Name = "Address Line 3")]
        public string BillToAddress3 { get; set; }
        [JsonProperty(PropertyName = "billToAddress4")]
        [MaxLength(50)]
        [Display(Name = "Address Line 4")]
        public string BillToAddress4 { get; set; }
        [JsonProperty(PropertyName = "billToAddress5")]
        [MaxLength(50)]
        [Display(Name = "Address Line 5")]
        public string BillToAddress5 { get; set; }
        [JsonProperty(PropertyName = "selfCollect")]
        [Display(Name = "Self Collect")]
        public bool SelfCollect { get; set; }
        [JsonProperty(PropertyName = "selfCollectRemarks1")]
        [MaxLength(34)]
        [Display(Name = "Self Collect Remarks 1")]
        public string SelfCollectRemarks1 { get; set; }
        [JsonProperty(PropertyName = "selfCollectRemarks2")]
        [MaxLength(34)]
        [Display(Name = "Self Collect Remarks 2")]
        public string SelfCollectRemarks2 { get; set; }
        [JsonProperty(PropertyName = "selfCollectRemarks3")]
        [MaxLength(34)]
        [Display(Name = "Self Collect Remarks 3")]
        public string SelfCollectRemarks3 { get; set; }
        [JsonProperty(PropertyName = "selfCollectRemarks4")]
        [MaxLength(34)]
        [Display(Name = "Self Collect Remarks 4")]
        public string SelfCollectRemarks4 { get; set; }
        [JsonProperty(PropertyName = "selfCollectRemarks5")]
        [MaxLength(34)]
        [Display(Name = "Self Collect Remarks 5")]
        public string SelfCollectRemarks5 { get; set; }
        [JsonProperty(PropertyName = "discByPercent")]
        [Display(Name = "Discount By")]
        public string DiscByPercent { get; set; }
        [JsonProperty(PropertyName = "discPercent")]
        [Display(Name = "Discount %")]
        public decimal DiscPercent { get; set; }
        [JsonProperty(PropertyName = "discAmount")]
        [Display(Name = "Disc Amount")]
        public decimal DiscAmount { get; set; }
        [JsonProperty(PropertyName = "gstTotal")]
        [Display(Name = "GST Total")]
        public decimal GstTotal { get; set; }
        [JsonProperty(PropertyName = "grandTotal")]
        [Display(Name = "Total")]
        [Range(0.01, 100000000000, ErrorMessage = "Total should be greater than zero")]
        public decimal GrandTotal { get; set; }
        //[JsonProperty(PropertyName = "rounding")]
        //[Display(Name = "Rounding")]
        //public decimal Rounding { get; set; }
        //[JsonProperty(PropertyName = "grandTotalAftRounding")]
        //[Display(Name = "Total After Rounding")]
        //public decimal GrandTotalAftRounding { get; set; }
        [JsonProperty(PropertyName = "lines")]
        public List<PQLineViewModel> Lines { get; set; }
        [JsonProperty(PropertyName = "noteLines")]
        public List<PQNoteViewModel> NoteLines { get; set; }
    }
}
using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.PaymentViewModels
{
    public class PaymentViewModel
    {
        public long DocEntry { get; set; }
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }

        [JsonProperty(PropertyName = "docNum")]
        [Display(Name = "Doc No")]
        [MaxLength(50)]
        public string DocNum { get; set; }

        [JsonProperty(PropertyName = "docDate")]
        [Display(Name = "Doc Date")]
        [Required]
        [ValidDateFormat(ErrorMessageResourceName = "Doc Date",DateFormat = "dd'/'MM'/'yyyy",ErrorMessage ="field is invalid", ShouldGTEToday = true)]
        public string DocDate { get; set; }
        [ValidCustomerCode(ErrorMessage = "field is invalid", ErrorMessageResourceName = "Customer Code")]
        [JsonProperty(PropertyName = "cardCode")]
        [Required]
        [Display(Name = "Customer Code")]
        public string CardCode { get; set; }

        [JsonProperty(PropertyName = "cardName")]
        [Required]
        [Display(Name = "Company Name")]
        public string CardName { get; set; }
        [JsonProperty(PropertyName = "status")]
        [Display(Name = "Status")]
        public short Status { get; set; }
        [JsonProperty(PropertyName = "printedStatus")]
        [Display(Name = "Printed Status")]
      
        public short PrintedStatus { get; set; }
        [JsonProperty(PropertyName = "syncStatus")]
        [Display(Name = "Sync Status")]
        [ReadOnly(true)]
        public short SyncStatus { get; set; }
        [JsonProperty(PropertyName = "ref")]
        [Display(Name = "Ref")]
        [MaxLength(100)]
        public string Ref { get; set; }
        [JsonProperty(PropertyName = "createdOn")]
        [Display(Name = "Created On")]
        [ReadOnly(true)]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        [Display(Name = "Created By")]
        [ReadOnly(true)]
        public string CreatedBy { get; set; }

        [Required]
        [MaxLength(15)]
        [JsonProperty(PropertyName = "gLCode")]
        [Display(Name = "GL Code")]
        public string GLCode { get; set; }
        [Required]
        [MaxLength(100)]
        [JsonProperty(PropertyName = "paymentType")]
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        [Required]
        [JsonProperty(PropertyName = "paidAmount")]
        [Display(Name = "Paid Amount")]
        [Range(0.01, 100000000000, ErrorMessage = "Paid Amount should be greater than zero")]
        public decimal PaidAmount { get; set; } = 0;
        [Required]
        [JsonProperty(PropertyName = "totalDiscountAmount")]
        [Display(Name = "Total Discount")]       
        public decimal TotalDiscountAmount { get; set; } = 0;

        [MaxLength(100)]
        [JsonProperty(PropertyName = "paymentRemarks")]
        [Display(Name = "Payment Remarks")]
        public string PaymentRemarks { get; set; }
        [Required]
        [Display(Name = "Payment Location")]
        [JsonProperty(PropertyName = "paymentLocation")]
        public string PaymentLocation { get; set; }

        [MaxLength(100)]
        [JsonProperty(PropertyName = "chequeNoReference")]
        [Display(Name = "Cheque No / Reference No")]
        public string ChequeNoReference { get; set; }

        [JsonProperty(PropertyName = "submittedToSAP")]
        public bool SubmittedToSAP { get; set; }
        [JsonProperty(PropertyName = "syncedToSAP")]
        public bool SyncedToSAP { get; set; }
        [JsonProperty(PropertyName = "lines")]
        public List<PaymentLineViewModel> Lines { get; set; }
        [JsonProperty(PropertyName = "noteLines")]
        public List<PaymentNoteViewModel> NoteLines { get; set; }
    }
}
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.CashSalesViewModels
{
    public class CashSalesPayViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]
        public int LineNum { get; set; }
        [JsonProperty(PropertyName = "gLCode")]
        [Required]
        [MaxLength(15)]
        [Display(Name = "GL Code")]
        public string GLCode { get; set; }
        [Display(Name = "GL Name")]
        [JsonProperty(PropertyName = "gLName")]
        [Required]
        [MaxLength(100)]
        public string GLName { get; set; }
        [Display(Name = "Payment Type")]
        [JsonProperty(PropertyName = "paymentType")]
        [Required]
        [MaxLength(100)]
        public string PaymentType { get; set; }
        [JsonProperty(PropertyName = "paidAmount")]
        [Required]
        [Display(Name = "Paid Amount")]
        public decimal PaidAmount { get; set; }
        [JsonProperty(PropertyName = "paymentRemarks")]
        [Display(Name = "Payment Remarks")]
        public string PaymentRemarks { get; set; }
        [JsonProperty(PropertyName = "chequeNoReference")]
        [Display(Name = "Cheque No Reference")]
        public string ChequeNoReference { get; set; }
        [Required]
        [Display(Name = "Payment Location")]
        [JsonProperty(PropertyName = "payLocation")]
        public string PayLocation { get; set; }
    }
}
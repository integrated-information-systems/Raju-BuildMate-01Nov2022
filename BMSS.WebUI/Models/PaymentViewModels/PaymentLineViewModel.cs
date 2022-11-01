using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.PaymentViewModels
{
    public class PaymentLineViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]
        public int LineNum { get; set; }        
        [JsonProperty(PropertyName = "referenceDocNum")]
        [Required]
        [MaxLength(50)]
        public string ReferenceDocNum { get; set; }         
        [JsonProperty(PropertyName = "referenceDocType")]
        [Required]
        [MaxLength(50)]
        public string ReferenceDocType { get; set; }
        [MaxLength(20)]
        [JsonProperty(PropertyName = "customerRef")]
        [Display(Name = "Customer Ref")]
        public string CustomerRef { get; set; }
        [Display(Name = "Payment Amount")]
        [JsonProperty(PropertyName = "paymentAmount")]
        [Required]
        //[Range(0.01, 100000000000, ErrorMessage = "Payment Amount should be greater than zero")]
        public decimal PaymentAmount { get; set; }
        [Display(Name = "Discount Amount")]
        [Required]
        [JsonProperty(PropertyName = "discountAmount")]        
        public decimal DiscountAmount { get; set; }
        [Display(Name = "Doc Total")]
        [JsonProperty(PropertyName = "docTotal")]
        [Range(0.01, 100000000000, ErrorMessage = "Doc Total should be greater than zero")]
        public decimal DocTotal { get; set; }
        [Display(Name = "Balance Due")]
        [JsonProperty(PropertyName = "balanceDue")]
        public decimal BalanceDue { get; set; }
        [ValidDateFormat(ErrorMessageResourceName = "Doc Date", DateFormat = "dd/MM/yyyy", ErrorMessage = "field is invalid", ShouldGTEToday = true)]
        [Display(Name = "Doc Date")]
        [JsonProperty(PropertyName = "docDate")]
        public string DocDate { get; set; }
        [JsonProperty(PropertyName = "paid")]
        public bool Paid { get; set; } = false;
    }
}
using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.ChangeCRLimitViewModels
{
    public class ChangeCRLimitViewModel
    {
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }

        [JsonProperty(PropertyName = "cardCode")]
        [ValidCustomerCode(ErrorMessage = "field is invalid", ErrorMessageResourceName = "Customer Code")]
        [Display(Name = "Customer Code")]
        [MaxLength(20)]
        [Required]
        public string CardCode { get; set; }
        [JsonProperty(PropertyName = "cardName")]
        [Display(Name = "Company Name")]
        [MaxLength(100)]
        [Required]
        public string CardName { get; set; }        
        [JsonProperty(PropertyName = "balance")]
        [Display(Name = "Balance")]               
        public decimal Balance { get; set; }
        [JsonProperty(PropertyName = "currentLimit")]
        [Display(Name = "Current Limit")]       
        public decimal CreditLine { get; set; }
        [JsonProperty(PropertyName = "newLimit")]
        [Display(Name = "New Limit")]
        [Required]
        public decimal NewLimit { get; set; }
    }
}
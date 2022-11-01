using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.Customer
{
    public class CashCustomerViewModel
    {
        [JsonProperty(PropertyName = "docEntry")]
        public long DocEntry { get; set; }
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }

        [JsonProperty(PropertyName = "customerID")]
        [Display(Name = "Contact No")]
        [MaxLength(20)]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be numeric")]
        public string CustomerID { get; set; }
        [JsonProperty(PropertyName = "customerName")]
        [Display(Name = "Company Name / Personal Name")]
        [MaxLength(100)]
        [Required]
        public string CustomerName { get; set; }
        [JsonProperty(PropertyName = "addressLine1")]
        [Display(Name = "Main Address Line 1")]
        public string AddressLine1 { get; set; }
        [JsonProperty(PropertyName = "addressLine2")]
        [Display(Name = "Line 2")]
        public string AddressLine2 { get; set; }
        [JsonProperty(PropertyName = "addressLine3")]
        [Display(Name = "Line 3")]
        public string AddressLine3 { get; set; }
        [JsonProperty(PropertyName = "addressLine4")]
        [Display(Name = "Line 4")]
        public string AddressLine4 { get; set; }
        [JsonProperty(PropertyName = "country")]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "postalCode")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [JsonProperty(PropertyName = "slpCode")]
        [Display(Name = "Sales Person")]
        [Required]
        [ValidSlpCode(ErrorMessage = "field is invalid", ErrorMessageResourceName = "Sales Person")]
        public Int32 SlpCode { get; set; }
        [Display(Name = "Sales Person")]
        [JsonProperty(PropertyName = "slpName")]
        public string SlpName { get; set; }
    }
}
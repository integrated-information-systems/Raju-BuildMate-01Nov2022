using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMSS.WebUI.Models.DOViewModels
{
    public class DOPlannerRowViewModel
    {
        public DOPlannerRowViewModel(List<SelectListItem> warhouses)
        {
            this.Warehouses = warhouses;
        }
        public DOPlannerRowViewModel()
        {

        }
        public List<SelectListItem> Warehouses { get; set; }
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }


        [JsonProperty(PropertyName = "docType")]
        public string DocType { get; set; }
        [JsonProperty(PropertyName = "creatorLocation")]
        public string CreatorLocation { get; set; }
        [Display(Name = "Reference No")]
        [JsonProperty(PropertyName = "referenceNo")]
        [Required]
        public string ReferenceNo { get; set; }
        [Display(Name = "Delivery Location")]
        [JsonProperty(PropertyName = "deliveryLocation")]
        [Required]
        public string DeliveryLocation { get; set; }
        [Required]
        [Display(Name = "Delivery Date")]
        [JsonProperty(PropertyName = "deliveryDate")]
        public string DeliveryDate { get; set; }
        [Display(Name = "Delivery Time")]
        [JsonProperty(PropertyName = "deliveryTime")]
        public string DeliveryTime { get; set; }
        [JsonProperty(PropertyName = "remarks")]
        public string Remarks { get; set; }
        public int SentCount { get; set; } = 0;

    }
}
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.CashSalesViewModels
{
    public class CashSalesLineViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]
        public int LineNum { get; set; }
        [Display(Name = "Stock Code")]
        [JsonProperty(PropertyName = "itemCode")]
        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; }
        [Display(Name = "Stock Description")]
        [JsonProperty(PropertyName = "description")]
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "overwriteDescription")]
        [Display(Name = "Overwrite Description")]
        [MaxLength(100)]
        public string OverwriteDescription { get; set; }
        [JsonProperty(PropertyName = "description2")]
        [Display(Name = "Description2")]
        [MaxLength(100)]
        public string Description2 { get; set; }
        [JsonProperty(PropertyName = "description3")]
        [Display(Name = "Description3")]
        [MaxLength(100)]
        public string Description3 { get; set; }
        [JsonProperty(PropertyName = "description4")]
        [Display(Name = "Description4")]
        [MaxLength(100)]
        public string Description4 { get; set; }
        [JsonProperty(PropertyName = "description5")]
        [Display(Name = "Description5")]
        [MaxLength(100)]
        public string Description5 { get; set; }
        [JsonProperty(PropertyName = "description6")]
        [Display(Name = "Description6")]
        [MaxLength(100)]
        public string Description6 { get; set; }
        [JsonProperty(PropertyName = "serialNumber")]
        [Display(Name = "Serial Number")]
        [MaxLength(36)]
        public string SerialNumber { get; set; }
        [Required]
        [JsonProperty(PropertyName = "qty")]
        [Display(Name = "Quantity")]
        public decimal Qty { get; set; }
        [Required]
        [Display(Name = "Location")]
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
        [Required]
        [Display(Name = "Location Text")]
        [JsonProperty(PropertyName = "locationText")]
        public string LocationText { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        [JsonProperty(PropertyName = "unitPrice")]
        [Range(0.0001, 10000000, ErrorMessage = "Unit Price should be greater than zero")]
        public decimal UnitPrice { get; set; }
        //[Required]
        [Display(Name = "Unit Cost")]
        [JsonProperty(PropertyName = "unitCost")]
        //[Range(0.0001, 10000000, ErrorMessage = "Unit Cost should be greater than zero")]
        public decimal UnitCost { get; set; }
        [Required]
        [Display(Name = "GST")]
        [JsonProperty(PropertyName = "gst")]
        public decimal Gst { get; set; }
        [Display(Name = "GST")]
        [Required]
        [JsonProperty(PropertyName = "gstName")]
        public string GstName { get; set; }
        [Display(Name = "Line Total")]
        [JsonProperty(PropertyName = "lineTotal")]
        public decimal LineTotal { get; set; }
    }
}
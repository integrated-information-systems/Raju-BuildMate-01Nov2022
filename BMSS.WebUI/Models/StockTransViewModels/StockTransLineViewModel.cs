using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.StockTransViewModels
{
    public class StockTransLineViewModel
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
        [Display(Name = "From Location")]
        [JsonProperty(PropertyName = "fromLocation")]
        public string FromLocation { get; set; }
        [Required]
        [Display(Name = "From Location Text")]
        [JsonProperty(PropertyName = "fromLocationText")]
        public string FromLocationText { get; set; }
        [Required]
        [Display(Name = "To Location")]
        [JsonProperty(PropertyName = "toLocation")]
        public string ToLocation { get; set; }
        [Required]
        [Display(Name = "To Location Text")]
        [JsonProperty(PropertyName = "toLocationText")]
        public string ToLocationText { get; set; }
 
    }
}
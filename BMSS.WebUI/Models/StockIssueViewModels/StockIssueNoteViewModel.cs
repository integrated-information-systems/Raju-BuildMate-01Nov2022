using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.StockIssueViewModels
{
    public class StockIssueNoteViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]
        public int LineNum { get; set; }
        [JsonProperty(PropertyName = "note")]
        [Required]
        [MaxLength(500)]
        public string Note { get; set; }
    }
}
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.SQViewModels
{
    public class SQNoteViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]        
        public int LineNum { get; set; }
        [JsonProperty(PropertyName = "note")]
        [Required]
        [MaxLength(500)]
        public string Note { get; set; }
        
    }
}
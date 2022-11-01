using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BMSS.WebUI.Models.PQViewModels
{
    public class PQNoteViewModel
    {
        [JsonProperty(PropertyName = "lineNum")]
        public int LineNum { get; set; }
        [JsonProperty(PropertyName = "note")]
        [Required]
        [MaxLength(500)]
        public string Note { get; set; }
    }
}
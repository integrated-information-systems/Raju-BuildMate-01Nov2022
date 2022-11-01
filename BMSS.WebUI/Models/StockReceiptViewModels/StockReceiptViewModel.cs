using BMSS.WebUI.Helpers.Attributes.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.StockReceiptViewModels
{
    public class StockReceiptViewModel
    {
        public long DocEntry { get; set; }
        public bool IsModelValid { get; set; } = true;
        public List<string> ModelErrList { get; set; }

        [JsonProperty(PropertyName = "docNum")]
        [Display(Name = "Doc No")]
        [MaxLength(50)]
        public string DocNum { get; set; }

        [JsonProperty(PropertyName = "docDate")]
        [Display(Name = "Doc Date")]
        [Required]
        [ValidDateFormat(ErrorMessageResourceName = "Doc Date",DateFormat = "dd'/'MM'/'yyyy",ErrorMessage ="field is invalid", ShouldGTEToday = true)]
        public string DocDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        [Display(Name = "Status")]
        public short Status { get; set; }
        [JsonProperty(PropertyName = "printedStatus")]
        [Display(Name = "Printed Status")]
       
        public short PrintedStatus { get; set; }
        [JsonProperty(PropertyName = "syncStatus")]
        [Display(Name = "Sync Status")]
        [ReadOnly(true)]
        public short SyncStatus { get; set; }
        [JsonProperty(PropertyName = "ref")]
        [Display(Name = "Ref")]
        [MaxLength(100)]
        public string Ref { get; set; }
        [JsonProperty(PropertyName = "createdOn")]
        [Display(Name = "Created On")]
        [ReadOnly(true)]
        public DateTime CreatedOn { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        [Display(Name = "Created By")]
        [ReadOnly(true)]
        public string CreatedBy { get; set; }

        [JsonProperty(PropertyName = "updatedBy")]
        [Display(Name = "Last Updated By")]
        [ReadOnly(true)]
        public string UpdatedBy { get; set; }
        [ReadOnly(true)]
        [JsonProperty(PropertyName = "updatedOn")]
        [Display(Name = "Last Updated On")]
        public DateTime UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "submittedToSAP")]
        public bool SubmittedToSAP { get; set; }
        [JsonProperty(PropertyName = "syncedToSAP")]
        public bool SyncedToSAP { get; set; }
        [JsonProperty(PropertyName = "lines")]
        public List<StockReceiptLineViewModel> Lines { get; set; }
        [JsonProperty(PropertyName = "noteLines")]
        public List<StockReceiptNoteViewModel> NoteLines { get; set; }

    }
}
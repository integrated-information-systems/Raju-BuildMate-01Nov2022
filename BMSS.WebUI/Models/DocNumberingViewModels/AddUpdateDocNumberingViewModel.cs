using BMSS.WebUI.Models.General;
using System;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.DocNumberingViewModels
{
    public class AddUpdateDocNumberingViewModel
    {

        public Int32 NumberingID { get; set; }
        [Required]
        [MaxLength(8)]
        [Display(Name = "Series Name")]
        public string SeriesName { get; set; }
        [Display(Name = "First No")]
        public Int32 FirstNo { get; set; }
        [Display(Name = "Last No")]
        public Int32 LastNo { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }
        [Required]
        [Display(Name = "Is Default?")]
        public bool IsDefault { get; set; }        
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
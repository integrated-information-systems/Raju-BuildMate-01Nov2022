using System;

namespace BMSS.WebUI.Models.DocNumberingViewModels
{
    public class DocNumberingViewModel
    {
        public int NumberingID { get; set; }
        public string SeriesName { get; set; }
        public int FirstNo { get; set; }
        public int NextNo { get; set; }
        public int LastNo { get; set; }
        public string Prefix { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLocked { get; set; }
        public bool IsUsed { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
 
    }
}
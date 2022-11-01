using System;

namespace BMSS.WebUI.Models.NotesViewModels
{
    public class ItemNoteViewModel
    {
        public int NoteID { get; set; }
        public string ItemCode { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
    }
}
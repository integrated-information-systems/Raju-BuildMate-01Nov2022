using BMSS.WebUI.Models.General;
using System;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.NotesViewModels
{
    public class AddUpdateNoteViewModel
    {
       
        public int NoteID { get; set; }
        [Required]
        public string CardCode { get; set; }
        [Required]
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
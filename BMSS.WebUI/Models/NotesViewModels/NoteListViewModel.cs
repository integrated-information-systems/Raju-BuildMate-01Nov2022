
using BMSS.WebUI.Models.General;
using System.Collections.Generic;


namespace BMSS.WebUI.Models.NotesViewModels
{
    public class NoteListViewModel
    {
        public IEnumerable<NoteViewModel> NotesList { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }

        
        public bool CanEditNote { get; set; }
    }
}
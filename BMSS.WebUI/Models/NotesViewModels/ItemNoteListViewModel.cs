using BMSS.WebUI.Models.General;
using System.Collections.Generic;

namespace BMSS.WebUI.Models.NotesViewModels
{
    public class ItemNoteListViewModel
    {
        public IEnumerable<ItemNoteViewModel> NotesList { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
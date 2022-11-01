using BMSS.WebUI.Models.General;
using System.Collections.Generic;

namespace BMSS.WebUI.Models.DocNumberingViewModels
{
    public class DocNumberingListViewModel
    {
        public IEnumerable<DocNumberingViewModel> NumberingList { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
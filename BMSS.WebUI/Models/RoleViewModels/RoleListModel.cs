using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.UserIdentity;
using System.Collections.Generic;

namespace BMSS.WebUI.Models.RoleViewModels
{
    public class RoleListModel
    {
        public IEnumerable<AppRole> RoleList { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
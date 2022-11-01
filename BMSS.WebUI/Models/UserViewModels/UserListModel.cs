using BMSS.WebUI.Models.General;
using BMSS.WebUI.Models.UserIdentity;
using System.Collections.Generic;

namespace BMSS.WebUI.Models.UserViewModels
{
    public class UserListModel
    {
        public IEnumerable<AppUser> UserList { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
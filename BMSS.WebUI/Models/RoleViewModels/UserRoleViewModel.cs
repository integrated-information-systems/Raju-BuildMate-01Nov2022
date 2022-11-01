using System.Collections.Generic;

namespace BMSS.WebUI.Models.RoleViewModels
{
    public class UserRoleViewModel
    {
        public string Username { get; set; }
        public List<string> RolesAssigned { get; set; }
    }
}
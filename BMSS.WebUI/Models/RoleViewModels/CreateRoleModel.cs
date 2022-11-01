using BMSS.WebUI.Models.General;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.RoleViewModels
{
    public class CreateRoleModel
    {
        [Required]
        public string Name { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
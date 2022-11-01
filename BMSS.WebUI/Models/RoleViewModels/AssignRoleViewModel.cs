using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.RoleViewModels
{
    public class AssignRoleViewModel
    {
        [Required]
        public string[] IdsToAdd { get; set; }
    }
}
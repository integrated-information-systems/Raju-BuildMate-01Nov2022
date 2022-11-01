using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
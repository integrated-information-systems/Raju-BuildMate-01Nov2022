using BMSS.WebUI.Models.General;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.UserViewModels
{
    public class CreateUserModel
    {              
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string PreferredLocation { get; set; }
        [Required]
        [Display(Name = "Cash Cusomter")]
        public string PreferredCashCusomter { get; set; }
        [Required]
        [Display(Name = "Sales Person")]
        public string PreferredSalesPerson { get; set; }
        [Required]
        public string Password { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }


    }
}
using BMSS.WebUI.Models.General;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Models.UserViewModels
{
    public class UpdateUserModel
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string UpdatedEmail { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string PreferredLocation { get; set; }
        [Required]
        [Display(Name = "Cash Cusomter")]
        public string PreferredCashCusomter { get; set; }
        [Required]
        [Display(Name = "Sales Person")]
        public string PreferredSalesPerson { get; set; }
        public string Password { get; set; }
        public AjaxFormViewModel AjaxOptions { get; set; }
    }
}
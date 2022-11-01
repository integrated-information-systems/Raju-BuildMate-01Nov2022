using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{

    public class ValidPaymentTermCode : ValidationAttribute
    {  
        //Repositories
        private I_OCTG_Repository i_OCTG_Repository;
        public ValidPaymentTermCode()
        {
            this.i_OCTG_Repository = new EF_OCTG_Repository();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {           
            if (i_OCTG_Repository.IsValidCode((short)value))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessageResourceName + " " + this.ErrorMessage);
            }

        }
    }
}
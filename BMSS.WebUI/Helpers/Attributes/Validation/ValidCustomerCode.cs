using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{
    public class ValidCustomerCode : ValidationAttribute
    {
        //Repositories
        private I_OCRD_Repository i_OCRD_Repository;

        public ValidCustomerCode()
        {
            this.i_OCRD_Repository = new EF_OCRD_Repository();
        }
    
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (i_OCRD_Repository.IsValidCustomerCode((string)value))
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
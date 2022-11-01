using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{
    public class ValidSupplierCode : ValidationAttribute
    {
        //Repositories
        private I_OCRD_Repository i_OCRD_Repository;

        public ValidSupplierCode()
        {
            this.i_OCRD_Repository = new EF_OCRD_Repository();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (i_OCRD_Repository.IsValidSupplierCode((string)value))
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
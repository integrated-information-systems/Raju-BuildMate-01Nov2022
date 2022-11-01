using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using System;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{
    public class ValidSlpCode : ValidationAttribute
    {
        //Repositories
        private I_OSLP_Repository i_OSLP_Repository;
        public ValidSlpCode()
        {
            this.i_OSLP_Repository = new EF_OSLP_Repository();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (i_OSLP_Repository.IsValidCode((Int32)value))
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
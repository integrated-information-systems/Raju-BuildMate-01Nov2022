using BMSS.Domain.Abstract.SAP;
using BMSS.Domain.Concrete.SAP;
using System;
using System.ComponentModel.DataAnnotations;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{
    public class ValidContactId : ValidationAttribute
    {
        //Repositories
        private I_OCPR_Repository i_OCPR_Repository;

        public ValidContactId()
        {
            this.i_OCPR_Repository = new EF_OCPR_Repository();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
          
            if (value == null) {
                return ValidationResult.Success;
            }
            else if (i_OCPR_Repository.IsValidCode((Int32)value))
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
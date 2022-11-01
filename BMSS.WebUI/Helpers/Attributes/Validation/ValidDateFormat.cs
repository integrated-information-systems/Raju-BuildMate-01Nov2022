using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BMSS.WebUI.Helpers.Attributes.Validation
{
    public class ValidDateFormat : ValidationAttribute
    {
        public string DateFormat { get; set; }
        public bool ShouldGTToday { get; set; }
        public bool ShouldGTEToday { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime ValidDate = new DateTime();
            string input = (string)value;
            if(input == null)
            {
                return ValidationResult.Success;
            }
            else if (DateTime.TryParseExact(input, DateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None,out ValidDate))
            {
                //if (ShouldGTToday)
                //{                     
                //    if (ValidDate> DateTime.Now)
                //    {
                //        return ValidationResult.Success;
                //    }
                //    else
                //    {
                //        return new ValidationResult(ErrorMessageResourceName + " Should be greater than Today's Date");
                //    }                
                //}
                //else if(ShouldGTEToday) {
                //    if (ValidDate >= DateTime.Today)
                //    {
                //        return ValidationResult.Success;
                //    }
                //    else
                //    {
                //        return new ValidationResult(ErrorMessageResourceName + " Should be greater than or equal to Today's Date");
                //    }
                //}
                //else
                //{
                //    return ValidationResult.Success;
                //}
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessageResourceName + " " + this.ErrorMessage);
            }
        }

    }
}
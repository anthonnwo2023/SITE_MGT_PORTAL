using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.V1.Web.Validators
{
    public class CustomMultipleSelectListValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            IList<string> values = value as List<string>;
            if (values.Count > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Please select at least one item in the list.",
                new[] { validationContext.MemberName });
        }
    }
}

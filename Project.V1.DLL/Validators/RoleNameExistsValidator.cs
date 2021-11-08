using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.V1.DLL.Validators
{
    public class RoleNameExistsValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RoleManager<IdentityRole> role = (RoleManager<IdentityRole>)validationContext
                         .GetService(typeof(RoleManager<IdentityRole>));

            List<IdentityRole> roles = role.Roles.ToList();
            bool roleExists = roles.Any(x => x.Name == value.ToString());

            if (!roleExists)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Role name already used.",
                new[] { validationContext.MemberName });
        }
    }
}

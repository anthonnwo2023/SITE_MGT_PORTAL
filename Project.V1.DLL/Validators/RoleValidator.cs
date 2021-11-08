using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Project.V1.DLL.Validators
{
    public class RoleValidator : AbstractValidator<IdentityRole>
    {
        public readonly RoleManager<IdentityRole> _role;
        public readonly IServiceProvider SP;

        public RoleValidator(RoleManager<IdentityRole> role, IServiceProvider sp)
        {
            _role = role;
            this.SP = sp;

            RuleFor(Q => Q.Name).NotEmpty()
                .Must(BeUniqueName).WithMessage("Vendor name already exists.");
        }

        private bool BeUniqueName(string roleName)
        {
            bool exist = (_role.Roles.ToList()).Where(Q => Q.Name == roleName).Any();
            return (exist == false);
        }
    }
}

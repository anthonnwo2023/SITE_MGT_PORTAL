using FluentValidation;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.V1.DLL.Validators
{
    public class VendorValidator : AbstractValidator<VendorModel>
    {
        public readonly IVendor _vendor;
        public readonly IServiceProvider SP;

        public VendorValidator(IVendor vendor, IServiceProvider sp)
        {
            _vendor = vendor;
            this.SP = sp;

            RuleFor(Q => Q.Name).NotEmpty()
                .MustAsync(BeUniqueName).WithMessage("Vendor name already exists.");
        }


        private async Task<bool> BeUniqueName(string vendorName, CancellationToken ct)
        {
            bool exist = (await _vendor.Get()).Where(Q => Q.Name == vendorName).Any();
            return (exist == false);
        }
    }
}

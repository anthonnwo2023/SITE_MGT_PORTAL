using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ESDWorkflow.V1.DLL.Validators
{
    public class VendorNameExistsValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IVendor vendor = (IVendor)validationContext
                         .GetService(typeof(IVendor));

            //IServiceProvider a = InstanceFactory.Instance;
            //a = InstanceFactory.Instance2;

            //IServiceProvider foo = a.GetRequiredService<IServiceProvider>();
            //var vendor = (IVendor)validationContext.GetService(typeof(IVendor));

            //InstanceFactory instFactory = new(a);
            //IVendor vendorInterface = instFactory.CreateVendor();

            List<VendorModel> vendors = vendor.Get().GetAwaiter().GetResult();
            bool vendorExists = vendors.Any(x => x.Name == value.ToString());

            if (!vendorExists)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Vendor name already used.",
                new[] { validationContext.MemberName });
        }
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project.V1.DLL.Validators;
using Project.V1.Models;

namespace Project.V1.Web.Middlewares
{
    public static class FormValidatorHandler
    {
        public static void ConfigureValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<VendorModel>, VendorValidator>();
            services.AddTransient<IValidator<IdentityRole>, RoleValidator>();
        }
    }
}

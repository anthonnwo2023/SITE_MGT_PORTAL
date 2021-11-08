using Microsoft.Extensions.DependencyInjection;
using System;

namespace Project.V1.Web.Middlewares
{
    public static class ClaimsPolicyExtension
    {
        public static void ConfigureClaimsAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //Role Policies
                options.AddPolicy("VendorAccessPolicy", policy => policy.RequireRole("Vendor"));
                options.AddPolicy("EngineerAccessPolicy", policy => policy.RequireRole("Engineer"));
                options.AddPolicy("ManagerAccessPolicy", policy => policy.RequireRole("Manager"));
                options.AddPolicy("AdminAccessPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdminAccessPolicy", policy => policy.RequireRole("SuperAdmin"));

                //Project Policies
                options.AddPolicy("SiteAcceptancePolicy", policy => policy.RequireClaim("Site Acceptance"));

                //Report Policies
                options.AddPolicy("Can:ViewReport", policy => policy.RequireClaim("Can View Report"));

                //Request Policies
                options.AddPolicy("Can:CreateRequest", policy => policy.RequireClaim("Can Create Request"));
                options.AddPolicy("Can:ViewRequest", policy => policy.RequireClaim("Can View Request"));
                options.AddPolicy("Can:ViewRequestSummary", policy => policy.RequireClaim("Can View Request Summary"));
                options.AddPolicy("Can:UpdateRequest", policy => policy.RequireClaim("Can Update Request"));

                //Account Policies
                options.AddPolicy("Can:AddUser", policy => policy.RequireClaim("Can Add User"));
                options.AddPolicy("Can:UpdateUser", policy => policy.RequireClaim("Can Update User"));
                options.AddPolicy("Can:DeleteUser", policy => policy.RequireClaim("Can Delete User"));
                options.AddPolicy("Can:ViewUser", policy => policy.RequireClaim("Can View User"));
                options.AddPolicy("Can:ManageAccess", policy => policy.RequireClaim("Can Manage Access"));

                //Role Policies
                options.AddPolicy("Can:AddRole", policy => policy.RequireClaim("Can Add Role"));
                options.AddPolicy("Can:UpdateRole", policy => policy.RequireClaim("Can Update Role"));
                options.AddPolicy("Can:DeleteRole", policy => policy.RequireClaim("Can Delete Role"));

                //Vendor Policies
                options.AddPolicy("Can:AddVendor", policy => policy.RequireClaim("Can Add Vendor"));
                options.AddPolicy("Can:UpdateVendor", policy => policy.RequireClaim("Can Update Vendor"));
                options.AddPolicy("Can:DeleteVendor", policy => policy.RequireClaim("Can Delete Vendor"));
            });
        }
    }
}

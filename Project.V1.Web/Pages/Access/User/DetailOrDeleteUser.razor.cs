using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.User
{
    public partial class DetailOrDeleteUser
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected RoleManager<IdentityRole> Role { get; set; }
        [Inject] protected IUser User { get; set; }
        [Inject] protected IVendor Vendor { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        public ApplicationUser ApplicationUserModel { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public string PageText { get; set; } = "Detail of";

        public InputModel Input { get; set; } = new InputModel();

        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();

        public List<VendorModel> Vendors { get; set; } = new List<VendorModel>();

        public List<PathInfo> Paths { get; set; }

        public bool ShowDeleteButton { get; set; }

        public class InputModel
        {
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            public string Department { get; set; }

            [Required]
            [Display(Name = "Vendor")]
            public string VendorId { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string RoleId { get; set; }

            [Required]
            public string Fullname { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} User", Link = "access/user/detail" },
                new PathInfo { Name = "Manage Access", Link = "access" },
                new PathInfo { Name = "Settings", Link = "access" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (Id.StartsWith("del-"))
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:DeleteUser"))
                        {
                            NavMan.NavigateTo("access-denied");
                        }

                        PageText = "Delete";
                        ShowDeleteButton = true;
                        Id = Id.Replace("del-", "");
                    }

                    if (!await UserAuth.IsAutorizedForAsync("Can:ViewUser"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    Vendors = await Vendor.Get();
                    Roles = await Role.Roles.ToListAsync();
                    ApplicationUserModel = await User.GetUserById(Id);

                    Logger.LogInformation("Loading User", new { });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading User", new { }, ex);
                }
            }
        }
        protected async Task HandleDeleteUser()
        {
            try
            {
                if (Id != null)
                {
                    ApplicationUserModel = await User.GetUserById(Id);

                    if (ApplicationUserModel != null)
                    {
                        await User.DeleteUser(ApplicationUserModel);
                    }
                }

                NavMan.NavigateTo("access");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting User", new { }, ex);
            }
        }
    }
}

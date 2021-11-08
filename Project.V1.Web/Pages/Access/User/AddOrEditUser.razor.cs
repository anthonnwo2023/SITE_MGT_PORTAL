using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Project.V1.Web.Validators;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.User
{
    public partial class AddOrEditUser
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] protected UserManager<ApplicationUser> User { get; set; }
        [Inject] protected RoleManager<IdentityRole> Role { get; set; }
        [Inject] protected IVendor Vendor { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected List<string> SelectedIds { get; set; } = new();
        public ApplicationUser ApplicationUserModel { get; set; }
        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create User";
        public InputModel Input { get; set; } = new();
        //public List<string> UserRoles { get; set; }
        public List<IdentityRole> Roles { get; set; } = new();
        public List<VendorModel> Vendors { get; set; } = new();
        public List<PathInfo> Paths { get; set; }
        public List<ClaimListManager> UserClaims { get; set; }
        public string OutPutValue { get; set; }
        public bool DisableCreateButton { get; set; } = false;
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";
        private SfToast ToastObj;

        public class InputModel
        {
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            public string Department { get; set; }

            [Required]
            [Display(Name = "Vendor")]
            public string VendorId { get; set; }

            //[Required]
            //[CustomMultipleSelectListValidator]
            //[Display(Name = "Role")]
            //public List<string> Roles { get; set; } = new List<string>();

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

            public bool ShouldRequirePassword { get; set; } = false;

            [RequiredWhen("ShouldRequirePassword", true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [RequiredWhen("ShouldRequirePassword", true, AllowEmptyStrings = false, ErrorMessage = "Password is required for vendor accounts.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string[] SelectedRoles { get; set; }
        }

        private async Task ShowOnClick()
        {
            await this.ToastObj.Show();
        }

        protected void ShowSelectedValues()
        {
            OutPutValue = string.Join(",", SelectedIds.ToArray());
            StateHasChanged();
        }

        protected void ClaimSelectionChanged(bool IsSelected)
        {
            if (IsSelected)
            {
                //UserClaims.Claims.Where();
            }
            else
            {

            }
        }

        protected void ToggleRoleClaimSelection(IReadOnlyList<string> selectedRoles)
        {
            //Input.Roles = selectedRoles.ToList();
        }

        protected void TogglePasswordValidation(ChangeEventArgs e)
        {
            string vendorValue = (string)e.Value;
            if (vendorValue == Vendors.FirstOrDefault(x => x.Name == "MTN Nigeria").Id && Id == null)
            {
                Input.ShouldRequirePassword = false;
            }
            else
            {
                Input.ShouldRequirePassword = true;
            }

            Input.VendorId = vendorValue;
        }

        protected override async Task OnInitializedAsync()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} User", Link = "access/user/create" },
                new PathInfo { Name = "Manage Access", Link = "access" },
            };

            await Task.CompletedTask;
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    ApplicationUserModel = new();
                    Vendors = await Vendor.Get();
                    UserClaims = new();
                    Roles = Role.Roles.ToList();

                    ClaimsPrincipal user = (await AuthenticationStateTask).User;
                    ApplicationUser userData = await IUser.GetUserByUsername(user.Identity.Name);

                    Logger.LogInformation("Loading User", new { });

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:UpdateUser"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        PageText = "Edit";
                        BtnText = "Update User";
                        ApplicationUserModel = await IUser.GetUserById(Id);

                        Input.SelectedRoles = (await IUser.GetUserRolesId(ApplicationUserModel)).ToArray();

                        UserClaims = (await Claim.Get(x => x.IsActive)).Where(x => x.Category.Name == "Project").GroupBy(x => x.Category.Name).Select(x => new ClaimListManager
                        {
                            Category = x.Key,
                            Claims = x.ToList().FormatClaimSelection(ApplicationUserModel)
                        }).ToList();

                        //Input.ShouldRequirePassword = (ApplicationUserModel.Vendor.Name == "MTN Nigeria");
                        if (ApplicationUserModel != null)
                        {
                            ApplicationUser2InputMapping();
                        }

                        return;
                    }

                    UserClaims = (await Claim.Get(x => x.IsActive)).Where(x => x.Category.Name == "Project").GroupBy(x => x.Category.Name).Select(x => new ClaimListManager
                    {
                        Category = x.Key,
                        Claims = x.ToList().FormatClaimSelection()
                    }).ToList();

                    if (!await UserAuth.IsAutorizedForAsync("Can:AddUser"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading User", new { }, ex);
                }
            }
        }

        protected async Task HandleValidSubmit()
        {
            
            DisableCreateButton = true;
            BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";
            StateHasChanged();

            try
            {
                bool isCreated = false;
                Input2ApplicationUserMapping();

                if (Id != null)
                {
                    isCreated = await IUser.UpdateUser(ApplicationUserModel, Input.Password, Input.SelectedRoles.ToList());
                }
                else
                {
                    isCreated = await IUser.CreateUser(ApplicationUserModel, Input.Password, Input.SelectedRoles.ToList());
                }

                if (isCreated)
                {
                    await User.AddUserClaims(ApplicationUserModel, UserClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList());

                    NavMan.NavigateTo("access");

                    return;
                }

                BulkUploadIconCss = "fas fa-paper-plane ml-2";
                DisableCreateButton = false;
            }
            catch (Exception ex)
            {
                BulkUploadIconCss = "fas fa-paper-plane ml-2";
                DisableCreateButton = false;
                ToastTitle = "Error Notification";
                ToastCss = "e-toast-danger";

                ToastContent = $"An error has occurred. {ex.Message}";
                await Task.Delay(100);
                await ShowOnClick();

                Logger.LogError($"Error {PageText}ing User", new { User = AuthenticationStateTask.Result.User.Identity.Name }, ex);
            }

        }

        private void Input2ApplicationUserMapping()
        {
            ApplicationUserModel.Department = Input.Department;
            ApplicationUserModel.UserName = Input.UserName;
            ApplicationUserModel.Fullname = Input.Fullname;
            ApplicationUserModel.PhoneNumber = Input.PhoneNumber;
            ApplicationUserModel.Email = Input.Email;
            ApplicationUserModel.Vendor.Id = Input.VendorId;
        }

        private void ApplicationUser2InputMapping()
        {
            Input.Department = ApplicationUserModel.Department;
            Input.UserName = ApplicationUserModel.UserName;
            Input.Fullname = ApplicationUserModel.Fullname;
            Input.PhoneNumber = ApplicationUserModel.PhoneNumber;
            Input.Email = ApplicationUserModel.Email;
            Input.VendorId = ApplicationUserModel.Vendor.Id;
        }
    }
}

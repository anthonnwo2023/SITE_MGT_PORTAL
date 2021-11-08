using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Helpers;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Validators;
using Project.V1.Models;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Project.V1.Web.Pages.Access.Role.AddOrEditRole;

namespace Project.V1.Web.Pages.Access.Role
{
    public partial class AddOrEditRole
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] public RoleManager<IdentityRole> Role { get; set; }

        public List<PathInfo> Paths { get; set; }
        public List<ClaimListManager> RoleClaims { get; set; }
        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create Role";
        public IdentityRole IdentityRole { get; set; }
        public bool DisableCreateButton { get; set; } = false;
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";
        public List<Claim> UserClaims { get; set; }

        [Required]
        [RoleNameExistsValidator]
        public string VendorName { get; set; }

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";
        private SfToast ToastObj;

        protected override async Task OnInitializedAsync()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} Role", Link = "access/role/create" },
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
                    IdentityRole = new();
                    RoleClaims = new();

                    Logger.LogInformation("Loading Role", new { });

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:UpdateRole"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        PageText = "Edit";
                        BtnText = "Update Role ";
                        IdentityRole = await Role.FindByIdAsync(Id);

                        RoleClaims = (await Claim.Get(x => x.IsActive)).Where(x => x.Category.Name != "Project").GroupBy(x => x.Category.Name).Select(x => new ClaimListManager
                        {
                            Category = x.Key,
                            Claims = x.ToList().FormatClaimSelection(IdentityRole)
                        }).ToList();

                        return;
                    }

                    RoleClaims = (await Claim.Get(x => x.IsActive)).Where(x => x.Category.Name != "Project").GroupBy(x => x.Category.Name).Select(x => new ClaimListManager
                    {
                        Category = x.Key,
                        Claims = x.ToList().FormatClaimSelection()
                    }).ToList();

                    if (!await UserAuth.IsAutorizedForAsync("Can:AddRole"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Role", new { }, ex);
                }
            }
        }

        private async Task ShowOnClick()
        {
            await this.ToastObj.Show();
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

        protected async Task HandleValidSubmit()
        {
            IdentityResult result = new();

            DisableCreateButton = true;
            BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";
            StateHasChanged();

            try
            {
                ClaimsPrincipal user = (await AuthenticationStateTask).User;
                bool IsSuperAdmin = user.IsInRole("SuperAdmin");

                if (Id != null)
                {
                    result = await Role.UpdateAsync(IdentityRole);

                    await Role.RemoveRoleClaims(IdentityRole);

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    DisableCreateButton = false;
                }
                else
                {
                    result = await Role.CreateAsync(IdentityRole);

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    DisableCreateButton = false;
                }

                if (result.Succeeded)
                {
                    await Role.AddRoleClaims(IdentityRole, RoleClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList());
                    NavMan.NavigateTo("access");
                }

                return;
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

                Logger.LogError($"Error {PageText}ing Role", new { User = AuthenticationStateTask.Result.User.Identity.Name }, ex);
            }
        }
    }
}

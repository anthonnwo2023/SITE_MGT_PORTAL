using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
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

namespace Project.V1.Web.Pages.Access.Claims
{
    public partial class AddOrEditClaim
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] public IClaimCategory ClaimCategory { get; set; }

        public List<PathInfo> Paths { get; set; }
        public List<ClaimCategoryModel> ClaimCategories { get; set; }
        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create Claim";
        public ClaimViewModel ClaimModel { get; set; }
        public string SelectedCategory { get; set; }
        public bool DisableCreateButton { get; set; } = false;
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

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
                new PathInfo { Name = $"{PageText} Claim", Link = "access/claim/create" },
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
                    ClaimModel = new ClaimViewModel();
                    ClaimCategories = (await ClaimCategory.Get(x => x.IsActive)).ToList();

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:UpdateClaim"))
                        {
                            NavMan.NavigateTo("access-denied");
                        }

                        PageText = "Edit";
                        BtnText = "Update Claim ";
                        ClaimModel = await Claim.GetById(x => x.Id == Id);
                    }

                    if (!await UserAuth.IsAutorizedForAsync("Can:AddClaim"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    Logger.LogInformation("Loading Claim", new { });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Claim", new { }, ex);
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
            ClaimViewModel result = new();
            DisableCreateButton = true;
            BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";
            StateHasChanged();

            try
            {
                ClaimsPrincipal user = (await AuthenticationStateTask).User;
                bool IsSuperAdmin = user.IsInRole("SuperAdmin");
                ClaimModel.Category = ClaimCategories.FirstOrDefault(x => x.Id == SelectedCategory);

                if (Id != null)
                {
                    result = await Claim.Update(ClaimModel, x => x.Id == Id);

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    DisableCreateButton = false;
                }
                else
                {
                    result = await Claim.Create(ClaimModel);

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    DisableCreateButton = false;
}

                if (result != default)
                {
                    NavMan.NavigateTo("access");
                    return;
                }
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

                Logger.LogError($"Error {PageText}ing Claim", new { User = AuthenticationStateTask.Result.User.Identity.Name }, ex);
            }
        }
    }
}

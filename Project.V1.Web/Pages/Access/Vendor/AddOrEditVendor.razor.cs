using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.Vendor
{
    public partial class AddOrEditVendor
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IVendor Vendor { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        public List<PathInfo> Paths { get; set; }
        public bool DisableCreateButton { get; set; } = false;
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";
        private SfToast ToastObj;

        public string VendorName { get; set; }
        public VendorModel VendorModel { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create Vendor";

        protected override async Task OnInitializedAsync()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} Vendor", Link = "access/vendor/create" },
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
                    VendorModel = new VendorModel();

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:UpdateVendor"))
                        {
                            NavMan.NavigateTo("access-denied");
                        }

                        PageText = "Edit";
                        BtnText = "Update Vendor";
                        VendorModel = await Vendor.GetById(x => x.Id == Id);
                    }

                    if (!await UserAuth.IsAutorizedForAsync("Can:AddVendor"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    Logger.LogInformation("Loading Vendor", new { });
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error loading Vendor", new { }, ex);
                }
            }
        }

        private async Task ShowOnClick()
        {
            await this.ToastObj.Show();
        }

        protected async Task HandleValidSubmit()
        {
            DisableCreateButton = true;
            BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";

            try
            {
                if (Id != null)
                {
                    await Vendor.Update(VendorModel, x => x.Id == VendorModel.Id);
                }
                else
                {
                    await Vendor.Create(VendorModel);
                }

                NavMan.NavigateTo("access");
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

                Logger.LogError($"Error {PageText}ing Vendor", new { }, ex);
            }
        }
    }
}

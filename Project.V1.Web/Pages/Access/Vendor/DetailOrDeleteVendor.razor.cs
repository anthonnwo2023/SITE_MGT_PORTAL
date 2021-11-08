using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.Vendor
{
    public partial class DetailOrDeleteVendor
    {
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IVendor Vendor { get; set; }
        [Inject] protected ICLogger Logger { get; set; }
        public string PageText { get; set; } = "Delete";
        public List<PathInfo> Paths { get; set; }
        public VendorModel VendorModel { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} Role", Link = "access/vendor/delete" },
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
                    if (!await UserAuth.IsAutorizedForAsync("Can:DeleteVendor") && !await UserAuth.IsAutorizedForAsync("Can:ViewVendor"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    VendorModel = await Vendor.GetById(x => x.Id == Id);
                    Logger.LogInformation("Loading Vendor", new { Id, Vendor = JsonConvert.SerializeObject(VendorModel) });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Vendor", new { }, ex);
                }
            }
        }

        protected async Task HandleDeleteVendor()
        {
            try
            {
                if (Id != null)
                {
                    VendorModel = await Vendor.GetById(x => x.Id == Id);

                    if (VendorModel != null)
                    {
                        await Vendor.Delete(VendorModel, x => x.Id == Id);
                    }
                }

                NavMan.NavigateTo("access");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error deleting Vendor", new { }, ex);
            }
        }
    }
}

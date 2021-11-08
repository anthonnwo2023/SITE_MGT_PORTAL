using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.Claims.Categories
{
    public partial class DetailOrDeleteClaimCategory
    { 
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IClaimCategory ClaimCategory { get; set; }
        public List<PathInfo> Paths { get; set; }
        public string PageText { get; set; } = "Delete";
        public ClaimCategoryModel ClaimCategoryModel { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"{PageText} Claim", Link = "access/claim/delete" },
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
                    if (!await UserAuth.IsAutorizedForAsync("Can:DeleteClaimCategory") && !await UserAuth.IsAutorizedForAsync("Can:ViewClaimCategory"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    ClaimCategoryModel = await ClaimCategory.GetById(x => x.Id == Id);

                    Logger.LogInformation("Loading Claim", new { });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Claim", new { }, ex);
                }
            }
        }
        protected async Task HandleDeleteClaim()
        {
            try
            {
                if (Id != null)
                {
                    ClaimCategoryModel = await ClaimCategory.GetById(x => x.Id == Id);

                    if (ClaimCategoryModel != null)
                    {
                        await ClaimCategory.Delete(ClaimCategoryModel, x => x.Id == Id);
                    }
                }

                NavMan.NavigateTo("access");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error Deleting Claim", new { }, ex);
            }
        }
    }
}

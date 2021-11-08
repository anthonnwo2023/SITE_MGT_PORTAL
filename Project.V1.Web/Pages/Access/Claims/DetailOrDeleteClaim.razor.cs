using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Project.V1.Data.Interfaces;
using Project.V1.DLL.Extensions;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Access.Claims
{
    public partial class DetailOrDeleteClaim
    { 
        [Parameter] public string Id { get; set; } = string.Empty;
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IClaimService Claim { get; set; }
        public List<PathInfo> Paths { get; set; }
        public string PageText { get; set; } = "Delete";
        public ClaimViewModel ClaimModel { get; set; }
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
                    if (!await UserAuth.IsAutorizedForAsync("Can:DeleteClaim") && !await UserAuth.IsAutorizedForAsync("Can:ViewClaim"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    ClaimModel = await Claim.GetById(x => x.Id == Id);

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
                    ClaimModel = await Claim.GetById(x => x.Id == Id);

                    if (ClaimModel != null)
                    {
                        await Claim.Delete(ClaimModel, x => x.Id == Id);
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

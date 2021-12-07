using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using Syncfusion.Blazor.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class Worklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }

        List<RequestViewModel> Requests { get; set; } = new();
        List<RegionViewModel> Regions { get; set; }
        List<TechTypeModel> TechTypes { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public ClaimsPrincipal Principal { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected SfGrid<RequestViewModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"My Worklist", Link = "acceptance/worklist" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:ReworkRequest"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    Principal = (await AuthenticationStateTask).User;

                    Requests = (await IRequest.Get(x => x.Requester.Username == Principal.Identity.Name && x.Status == "Rejected")).OrderByDescending(x => x.EngineerAssigned.DateApproved).ToList();
                    TechTypes = await ITechType.Get(x => x.IsActive);
                    Regions = await IRegion.Get(x => x.IsActive);
                    Spectrums = await ISpectrum.Get(x => x.IsActive);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

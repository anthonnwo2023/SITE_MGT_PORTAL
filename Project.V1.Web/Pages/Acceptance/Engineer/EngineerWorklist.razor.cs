using Project.V1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Project.V1.Lib.Interfaces;
using Project.V1.DLL.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Syncfusion.Blazor.Grids;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Services;
using Project.V1.DLL.Services.Interfaces;
using System.Linq;
using Project.V1.Web.Shared;
using Project.V1.Lib.Extensions;

namespace Project.V1.Web.Pages.Acceptance.Engineer
{
    public partial class EngineerWorklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IUser IUser { get; set; }

        public List<RequestViewModel> Requests { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected SfGrid<RequestViewModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Engineer Worklist", Link = "acceptance/engineer/worklist" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }
        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:UpdateRequest"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    Requests = (await IRequest.Get(x => User.Regions.Select(x => x.Id).Contains(x.RegionId) && x.Status != "Rejected" && x.Status != "Accepted")).OrderByDescending(x => x.DateCreated).ToList();
                    TechTypes = await ITechType.Get(x => x.IsActive);
                    Regions = await IRegion.Get(x => x.IsActive);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

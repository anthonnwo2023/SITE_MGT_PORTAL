namespace Project.V1.Web.Pages.SiteHalt.Engineer
{
    public partial class EngineerWorklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUser IUser { get; set; }

        //public List<RequestViewModel> RequestEngWorklists { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        //protected SfGrid<RequestViewModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Engineer Worklist", Link = "halt/engineer/worklist" },
                new PathInfo { Name = $"Site Halt & Unhalt", Link = "halt" },
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
                    var userRegionIds = User.Regions.Select(x => x.Id);

                    //RequestEngWorklists = (await IRequest.Get(x => userRegionIds.Contains(x.RegionId) && (x.Status == "Pending" || x.Status == "Reworked"
                    //                        || x.Status == "Restarted"))).OrderByDescending(x => x.DateCreated).ToList();

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                    StateHasChanged();
                }
            }
        }
    }
}

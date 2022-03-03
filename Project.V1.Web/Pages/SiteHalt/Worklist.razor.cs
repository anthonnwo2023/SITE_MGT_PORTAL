namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Worklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }

        //List<RequestViewModel> Requests { get; set; } = new();

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        //protected SfGrid<RequestViewModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"My Worklist", Link = "halt/worklist" },
                new PathInfo { Name = $"Site Halt & Unhalt", Link = "halt" },
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
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    //Requests = (await IRequest.Get(x => x.Requester.Vendor.Name == User.Vendor.Name && x.Status == "Rejected")).OrderByDescending(x => x.EngineerAssigned.DateActioned).ToList();

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

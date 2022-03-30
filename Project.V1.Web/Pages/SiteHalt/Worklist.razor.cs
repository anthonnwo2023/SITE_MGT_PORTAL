namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Worklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IHUDRequest IHUDRequest { get; set; }

        protected List<SiteHUDRequestModel> HUDMyWorklist { get; set; } = new();

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected SfGrid<SiteHUDRequestModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"My Worklist", Link = "hud/worklist" },
                new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
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

                    HUDMyWorklist = (await IHUDRequest.Get(x => x.Requester.Username == User.UserName && x.Status.Contains("Disapprove"), x => x.OrderByDescending(y => y.DateCreated))).ToList();

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

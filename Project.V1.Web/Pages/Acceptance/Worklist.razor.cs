namespace Project.V1.Web.Pages.Acceptance
{
    public partial class Worklist
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IUser IUser { get; set; }
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
        public ApplicationUser User { get; set; }

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
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    Requests = (await IRequest.Get(x => x.Requester.Vendor.Name == User.Vendor.Name && x.Status == "Rejected", x => x.OrderByDescending(x => x.EngineerAssigned.DateActioned), "EngineerAssigned,Requester.Vendor,AntennaMake,AntennaType")).ToList();
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

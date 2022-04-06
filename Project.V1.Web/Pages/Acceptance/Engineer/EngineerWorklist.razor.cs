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
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IProjects IProject { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected IUser IUser { get; set; }

        public List<RequestViewModel> RequestEngWorklists { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<ProjectModel> Projects { get; set; }
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
                    var userRegionIds = User.Regions.Select(x => x.Id);

                    RequestEngWorklists = (await IRequest.Get(x => userRegionIds.Contains(x.RegionId) && (x.Status == "Pending" || x.Status == "Reworked"
                                            || x.Status == "Restarted"), x => x.OrderByDescending(x => x.DateCreated), "Requester.Vendor")).ToList();
                    TechTypes = await ITechType.Get(x => x.IsActive);
                    Regions = await IRegion.Get(x => x.IsActive);
                    Spectrums = await ISpectrum.Get(x => x.IsActive);
                    ProjectTypes = await IProjectType.Get(x => x.IsActive);
                    Projects = await IProject.Get(x => x.IsActive);

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

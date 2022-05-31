namespace Project.V1.Web.Pages
{
    public partial class Dashboard
    {
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private readonly List<PathInfo> Paths = new();

        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] protected IHttpContextAccessor HttpContext { get; set; }
        [Inject] public IUser IUser { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] public ICLogger Logger { get; set; }

        public List<ClaimListManager> UserClaims { get; set; }
        public List<ClaimViewModel> ProjectClaims { get; set; }

        public string AppDescription { get; set; } = "This is a central portal for managing site acceptance and other related site activities";
        public string AppName { get; set; } = "{AppName}";
        public string AppButtonVisible { get; set; } = "none";
        public string AppLink { get; set; } = "#";
        public string AppImage { get; set; } = "images/app-placeholder.png";

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    //if (!await UserAuth.IsAutorizedForAsync("Can:ManageAccess"))
                    //{
                    //    NavMan.NavigateTo("acceptance");
                    //}

                    ClaimsPrincipal user = (await AuthenticationStateTask).User;
                    ApplicationUser userData = await IUser.GetUserByUsername(user.Identity.Name);

                    UserClaims = (await Claim.Get(y => y.IsActive && y.Category.Name == "Project", null, "Category")).GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                    {
                        Category = u.Key,
                        Claims = u.ToList().FormatClaimSelection(userData)
                    }).ToList();

                    ProjectClaims = UserClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading Access data", new { }, ex);
                }
            }
        }

        protected void LoadLinkData(string app)
        {
            AppDescription = GetAppDescription(app);
            AppName = HelperFunctions.GetTypeName(app);
            AppLink = HelperFunctions.GetAppLink(app);
            AppImage = GetAppImage(app);
            AppButtonVisible = "block";
        }

        private static string GetAppDescription(string app)
        {
            return app switch
            {
                "SA" => "This application is used to manage an end-2-end acceptance of sites. It ensures request status tracking, aging and helps to identify bottlenecks.",
                "HS" => "This application is used to plan site implementations at the beginning of the year.",
                "LS" => "This application is used to manage live sites that are already active on the network.",
                "EM" => "This application is used to manage equipment matching, identifying all equipments available at network sites.",
                "EO" => "This application is used to manage inventory of equipment ordered for specific sites.",
                "H|U|D" => "This application is used to manage both the halting and unhalting process",
                _ => "This is possibly a bug. Please notify TSS"
            };
        }

        protected static string GetAppImage(string app)
        {
            return app switch
            {
                "SA" => "images/site-accept.jpg",
                "HS" => "images/holistic.jpg",
                "LS" => "images/live.jpg",
                "EM" => "images/eq-matching.jpg",
                "EO" => "images/eq-ordering.jpg",
                "H|U|D" => "images/hud.jpg",
                _ => "images/app-placeholder.png"
            };
        }
    }
}

namespace Project.V1.Web.Shared
{
    public partial class LoginDisplay
    {
        [Inject] public IUser IUser { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        public List<ClaimListManager> AllProjectClaims { get; set; }
        public List<ClaimViewModel> ProjectClaims { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        public bool CanViewDashboard { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ClaimsPrincipal user = (await AuthenticationStateTask).User;
                ApplicationUser userData = (user.Identity.Name == null) ? throw new Exception("No logged-in user found") : await IUser.GetUserByUsername(user.Identity.Name);

                AllProjectClaims = (await Claim.Get(y => y.IsActive, null, "Category")).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                {
                    Category = u.Key,
                    Claims = u.ToList().FormatClaimSelection(userData)
                }).ToList();

                ProjectClaims = AllProjectClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();
                CanViewDashboard = ProjectClaims.Count > 1;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading Access data", new { }, ex);
            }
        }
    }
}

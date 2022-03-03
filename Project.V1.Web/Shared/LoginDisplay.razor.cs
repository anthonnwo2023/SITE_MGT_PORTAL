namespace Project.V1.Web.Shared
{
    public partial class LoginDisplay
    {
        [Inject] public IUser IUser { get; set; }
        [Inject] public IClaimService Claim { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        public List<ClaimListManager> UserClaims { get; set; }
        public List<ClaimViewModel> ProjectClaims { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        public bool CanManageAccess { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ClaimsPrincipal user = (await AuthenticationStateTask).User;
                ApplicationUser userData = (user.Identity.Name == null) ? throw new Exception("No logged-in user found") : await IUser.GetUserByUsername(user.Identity.Name);

                UserClaims = (await Claim.Get(y => y.IsActive)).Where(z => z.Category.Name == "Project").GroupBy(v => v.Category.Name).Select(u => new ClaimListManager
                {
                    Category = u.Key,
                    Claims = u.ToList().FormatClaimSelection(userData)
                }).ToList();

                ProjectClaims = UserClaims.SelectMany(x => x.Claims).Where(x => x.IsSelected).ToList();
                CanManageAccess = user.HasClaim(x => x.Type == "Can:ManageAccess");

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading Access data", new { }, ex);
            }
        }

        protected static string GetAppLink(string app)
        {
            return app switch
            {
                "SA" => "acceptance",
                "HS" => "holistic",
                "LS" => "live",
                "EM" => "eq-matching",
                "EO" => "eq-ordering",
                "SH&U" => "halt",
                _ => "Buggy Link"
            };
        }

        protected static string GetAppName(string app)
        {
            return app switch
            {
                "SA" => "Site Acceptance",
                "HS" => "Holistic Site",
                "LS" => "Live Site",
                "EM" => "Equipment Matching",
                "EO" => "Equipment Ordering",
                "SH&U" => "Site Halt & Unhalt",
                _ => "Buggy Link"
            };
        }
    }
}

namespace Project.V1.Web.Pages.SiteHalt.Components
{
    public partial class NavMenuHalt
    {
        private bool collapseNavMenu = true;
        private bool expandReportNav;
        private bool expandManageNav;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        [Inject] protected IRequest IRequest { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Principal = (await AuthenticationStateTask).User;

                if (Principal.Identity.Name != null)
                {
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                }
                else
                {
                    string returnUrl = Uri.EscapeDataString(NavMan.ToBaseRelativePath(NavMan.Uri));
                    returnUrl = (returnUrl == "access-denied" || returnUrl.Contains("logout")) ? null : returnUrl;

                    NavMan.NavigateTo($"Identity/Account/Login?returnUrl={returnUrl}", forceLoad: true);

                    return;
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }
    }
}

namespace Project.V1.Web.Pages.SiteHalt.Components
{
    public partial class NavMenuHalt : IDisposable
    {
        private bool collapseNavMenu = true;

        private bool expandHUDRequestSubNav;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        public int HUDRejectedWorklistCount { get; set; }
        [Inject] AppState AppState { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        [Inject] protected IHUDRequest IHUDRequest { get; set; }
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private void HandleReloadClick()
        {
            NavMan.NavigateTo("hud/approver/worklist", true);
        }

        private void CalStateChanged()
        {
            var request = new SiteHUDRequestModel();

            HUDRejectedWorklistCount = (IHUDRequest.Get(x => x.Requester.Username == User.UserName && x.Status.EndsWith("Disapproved")).GetAwaiter().GetResult()).Count();

            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Principal = (await AuthenticationStateTask).User;
                AppState.OnChange += CalStateChanged;

                if (Principal.Identity.Name != null)
                {
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                    AppState.TriggerRequestRecount();
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

        public void Dispose()
        {
            AppState.OnChange -= CalStateChanged;
        }
    }
}

namespace Project.V1.Web.Pages.SiteHalt.Components
{
    public partial class BreadcrumbHalt
    {
        [Parameter] public List<PathInfo> Paths { get; set; }
        [Parameter] public EventCallback<bool> OnAuthenticationCheck { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IHttpContextAccessor HttpContext { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (!await UserAuth.IsAuthenticatedAsync() || !await UserAuth.IsAutorizedForAsync("Site Halt & Unhalt"))
            {
                NavMan.NavigateTo("access-denied");
                return;
            }

            await OnAuthenticationCheck.InvokeAsync(true);
        }
    }
}

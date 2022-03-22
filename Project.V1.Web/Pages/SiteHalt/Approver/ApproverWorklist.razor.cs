namespace Project.V1.Web.Pages.SiteHalt.Approver;

public partial class ApproverWorklist : IDisposable
{
    public List<PathInfo> Paths { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] public IHUDRequest IHUDRequest { get; set; }

    List<SiteHaltRequestModel> HUDApproverRequests { get; set; } = new();

    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    protected SfGrid<SiteHaltRequestModel> Grid_Request { get; set; }

    public List<string> ToolbarItems = new() { "Search" };

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"Approver Worklist", Link = "hud/engineer/worklist" },
            new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
        };
    }

    protected async Task AuthenticationCheck(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            try
            {
                if (!await UserAuth.IsAutorizedForAsync("Can:ApproveHUD") && !await UserAuth.IsAutorizedForAsync("Can:UpdateRequest"))
                {
                    NavMan.NavigateTo("access-denied");
                }

                Principal = (await AuthenticationStateTask).User;
                User = await IUser.GetUserByUsername(Principal.Identity.Name);
                var userRegionIds = User.Regions.Select(x => x.Id);

                HUDApproverRequests = (await IHUDRequest.Get(x => (x.FirstApprover.Username == User.UserName && !x.FirstApprover.IsActioned)
                    || (x.FirstApprover.IsApproved && x.SecondApprover.Username == User.UserName && !x.SecondApprover.IsActioned)
                    || (x.FirstApprover.IsApproved && x.SecondApprover.IsApproved && x.ThirdApprover.Username == User.UserName && !x.ThirdApprover.IsActioned))).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
                StateHasChanged();
            }
        }
    }

    public void Dispose()
    {
        HUDApproverRequests = null;
    }
}

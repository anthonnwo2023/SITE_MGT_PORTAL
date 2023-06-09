﻿namespace Project.V1.Web.Pages.SiteHalt.Approver;

public partial class ApproverWorklist : IDisposable
{
    public List<PathInfo> Paths { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] public IHUDRequest IHUDRequest { get; set; }

    List<SiteHUDRequestModel> HUDApproverRequests { get; set; } = new();

    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    protected SfGrid<SiteHUDRequestModel> Grid_Request { get; set; }

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

                List<string> FAStatus = new() { "Pending", "Restarted" };

                HUDApproverRequests = (await IHUDRequest.Get(x => (x.FirstApprover.Username == User.UserName && FAStatus.Contains(x.Status))
                    || (x.SecondApprover.Username == User.UserName && x.Status.Equals("FAApproved"))
                    || (x.ThirdApprover.Username == User.UserName && x.Status.Equals("SAApproved")), x => x.OrderByDescending(y => y.DateCreated), "Requester.Vendor,FirstApprover,SecondApprover,ThirdApprover,TechTypes")).ToList();
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
        GC.SuppressFinalize(this);
    }
}

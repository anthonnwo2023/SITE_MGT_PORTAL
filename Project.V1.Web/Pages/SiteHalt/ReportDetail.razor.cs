﻿namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class ReportDetail
    {
        [Parameter] public string Id { get; set; }
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IHUDRequest IHUDRequest { get; set; }
        [Inject] protected IUser IUser { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public SiteHaltRequestModel ReportRequest { get; set; }
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
        public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();

        public List<string> ToolbarItems = new() { "Search" };

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Request Detail", Link = $"hud/reports/general/{Id}" },
                new PathInfo { Name = $"Report", Link = "hud/reports/general" },
                new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
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

                    ReportRequest = await IHUDRequest.GetById(x => x.Id == Id);

                    if (ReportRequest.RequestAction != "UnHalt")
                    {
                        ReportRequest.FirstApproverId = ReportRequest.FirstApprover.Id;
                        ReportRequest.SecondApproverId = ReportRequest.SecondApprover.Id;
                        ReportRequest.ThirdApproverId = ReportRequest.ThirdApprover.Id;
                    }

                    ReportRequest.TechTypeIds = ReportRequest.TechTypes.Select(x => x.Id).ToArray();

                    BaseFirstLevelApprovers = (await UserManager.GetUsersInRoleAsync("HUD RF SM")).Select(x => new RequestApproverModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Fullname = x.Fullname,
                        ApproverType = "FA",
                        Email = x.Email,
                        PhoneNo = x.PhoneNumber,
                        Username = x.UserName,
                        Title = x.JobTitle,
                    }).ToList();

                    BaseFirstLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == ReportRequest.FirstApprover?.Username)
                        {
                            x.Id = ReportRequest.FirstApprover.Id;
                        }
                    });

                    BaseSecondLevelApprovers = (await UserManager.GetUsersInRoleAsync("HUD Approver"))
                        .Where(x => !BaseFirstLevelApprovers.Select(y => y.Username).Contains(x.UserName)).Select(x => new RequestApproverModel
                        {
                            Id = Guid.NewGuid().ToString(),
                            Fullname = x.Fullname,
                            ApproverType = "SA",
                            Email = x.Email,
                            PhoneNo = x.PhoneNumber,
                            Username = x.UserName,
                            Title = x.JobTitle,
                        }).ToList();

                    BaseSecondLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == ReportRequest.SecondApprover?.Username)
                        {
                            x.Id = ReportRequest.SecondApprover.Id;
                        }
                    });

                    BaseThirdLevelApprovers = BaseSecondLevelApprovers;

                    BaseThirdLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == ReportRequest.ThirdApprover?.Username)
                        {
                            x.Id = ReportRequest.ThirdApprover.Id;
                        }
                    });
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

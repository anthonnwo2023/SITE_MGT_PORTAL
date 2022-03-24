namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Report : IDisposable
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IHUDRequest IHUDRequest { get; set; }
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

        List<SiteHaltRequestModel> HUDReportRequests { get; set; } = new();
        public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();

        public List<RequestApproverModel> FirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> SecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> ThirdLevelApprovers { get; set; } = new();
        public SiteHaltRequestModel RequestModel { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public string ButtonProcessingIconCss { get; set; } = "fas fa-spin fa-spinner m-1 text-white";
        public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected SfGrid<SiteHaltRequestModel> Grid_Request { get; set; }
        protected bool[] CompleteButtons { get; set; }
        protected bool[] UpdateButtons { get; set; }
        private string RequestUniqueId { get; set; }
        public bool Visibility { get; set; } = true;
        public bool DisableBtn { get; set; } = true;

        public List<string> ToolbarItems = new() { "Search" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
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
                    if (!await UserAuth.IsAutorizedForAsync("Can:ViewReport"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    HUDReportRequests = (await IHUDRequest.Get(null, x => x.OrderByDescending(y => y.DateCreated)));

                    CompleteButtons = new bool[HUDReportRequests.Count];
                    UpdateButtons = new bool[HUDReportRequests.Count];

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }


        public void ChangeNonRFSMApprover(ChangeEventArgs<string, RequestApproverModel> args, string approver)
        {
            if (args.ItemData == null) return;

            SecondLevelApprovers = BaseSecondLevelApprovers.ToList();
            ThirdLevelApprovers = BaseThirdLevelApprovers.ToList();

            if (approver.Equals("approver1"))
            {
                RequestModel.FirstApprover = args.ItemData;
            }

            if (approver.Equals("approver2"))
            {
                var approver3 = (!string.IsNullOrWhiteSpace(RequestModel.ThirdApproverId))
                    ? BaseThirdLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.ThirdApproverId)
                    : new RequestApproverModel();
                ThirdLevelApprovers.Remove(args.ItemData);
                SecondLevelApprovers.Remove(approver3);

                RequestModel.SecondApprover = args.ItemData;
            }
            if (approver.Equals("approver3"))
            {
                var approver2 = (!string.IsNullOrWhiteSpace(RequestModel.SecondApproverId))
                    ? BaseSecondLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.SecondApproverId)
                    : new RequestApproverModel();
                SecondLevelApprovers.Remove(args.ItemData);
                ThirdLevelApprovers.Remove(approver2);

                RequestModel.ThirdApprover = args.ItemData;
            }
            DisableBtn = false;

            StateHasChanged();
        }
        public async void ShowUpdateApproverDialog(MouseEventArgs args, string requestId)
        {
            RequestModel = HUDReportRequests.FirstOrDefault(x => x.Id == requestId);
            RequestModel.TempComment = "Temp";
            RequestModel.TempStatus = "TStatus";
            RequestModel.User = User;

            BaseFirstLevelApprovers = (await UserManager.GetUsersInRoleAsync("HUD RF SM")).Select(x => new RequestApproverModel
            {
                Id = Guid.NewGuid().ToString(),
                Fullname = x.Fullname,
                ApproverType = "FA",
                Email = x.Email,
                PhoneNo = x.PhoneNumber,
                Username = x.UserName,
                Title = x.JobTitle,
                RequestId = RequestModel.Id
            }).ToList();

            if (RequestModel.FirstApprover is not null)
                BaseFirstLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestModel.FirstApprover.Username)
                    {
                        x.Id = RequestModel.FirstApprover.Id;
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
                    RequestId = RequestModel.Id
                }).ToList();

            if (RequestModel.SecondApprover is not null)
                BaseSecondLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestModel.SecondApprover.Username)
                    {
                        x.Id = RequestModel.SecondApprover.Id;
                    }
                });

            BaseThirdLevelApprovers = BaseSecondLevelApprovers;

            if (RequestModel.ThirdApprover is not null)
                BaseThirdLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestModel.ThirdApprover.Username)
                    {
                        x.Id = RequestModel.ThirdApprover.Id;
                    }
                });

            RequestUniqueId = RequestModel?.UniqueId;

            if (RequestModel.FirstApprover is not null)
                RequestModel.FirstApproverId = RequestModel.FirstApprover.Id;

            if (RequestModel.SecondApprover is not null)
                RequestModel.SecondApproverId = RequestModel.SecondApprover.Id;

            if (RequestModel.ThirdApprover is not null)
                RequestModel.ThirdApproverId = RequestModel.ThirdApprover.Id;

            if (RequestModel.ThirdApprover != null)
                SecondLevelApprovers.Remove(SecondLevelApprovers.FirstOrDefault(x => x.Username == RequestModel.ThirdApprover.Username));

            if (RequestModel.SecondApprover != null)
                ThirdLevelApprovers.Remove(ThirdLevelApprovers.FirstOrDefault(x => x.Username == RequestModel.SecondApprover.Username));

            FirstLevelApprovers = BaseFirstLevelApprovers.ToList();
            SecondLevelApprovers = BaseSecondLevelApprovers.ToList();
            ThirdLevelApprovers = BaseThirdLevelApprovers.ToList();
            Visibility = true;
            DisableBtn = true;

            StateHasChanged();
        }

        protected async Task<bool> UpdateApprovers()
        {
            var index = HUDReportRequests.IndexOf(RequestModel);
            UpdateButtons[index] = true;
            try
            {
                await IHUDRequest.SetState(RequestModel, "SiteHalt");

                return (IHUDRequest as dynamic).Update(RequestModel, RequestModel.Variables);
            }
            catch
            {
                return await Task.FromResult(false);
            }
        }

        private void DialogClosed(CloseEventArgs args)
        {
            RequestUniqueId = string.Empty;

            DisableBtn = true;
        }

        public void Dispose()
        {
            HUDReportRequests = null;
        }
    }
}

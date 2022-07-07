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

        List<SiteHUDRequestModel> HUDReportRequests { get; set; } = new();
        public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();

        public List<RequestApproverModel> FirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> SecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> ThirdLevelApprovers { get; set; } = new();
        public SiteHUDRequestModel RequestModel { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public string ButtonProcessingIconCss { get; set; } = "fas fa-spin fa-spinner m-1 text-white";
        public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected SfGrid<SiteHUDRequestModel> Grid_Request { get; set; }
        public bool UserIsAdmin { get; set; }
        protected bool[] CompleteButtons { get; set; }
        protected bool[] UpdateButtons { get; set; }
        private string RequestUniqueId { get; set; }
        public bool Visibility { get; set; } = true;
        public bool DisableBtn { get; set; } = true;

        public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

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

                    UserIsAdmin = await UserManager.IsInRoleAsync(User, "Admin") || await UserManager.IsInRoleAsync(User, "Super Admin");

                    HUDReportRequests = (await IHUDRequest.Get(null, x => x.OrderByDescending(y => y.DateCreated), "Requester.Vendor,FirstApprover,SecondApprover,ThirdApprover,TechTypes")).ToList();

                    CompleteButtons = new bool[HUDReportRequests.Count];
                    UpdateButtons = new bool[HUDReportRequests.Count];

                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "HUDReportTable_excelexport") //Id is combination of Grid's ID and itemname
            {
                ExcelExportProperties ExportProperties = new();
                ExportProperties.FileName = $"General_Report{DateTimeOffset.UtcNow:ddMMyyyy.Hmmss}.xlsx";
                ExportProperties.IncludeHiddenColumn = false;

                await Grid_Request.ExcelExport(ExportProperties);
            }
        }

        public void ChangeNonRFSMApprover(ChangeEventArgs<string, RequestApproverModel> args, string approver)
        {
            if (args.ItemData == null)
            {
                return;
            }

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

            var Approvers = (await UserManager.GetUsersInRoleAsync("HUD Approver"))
                .Select(x => new RequestApproverModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = x.Fullname,
                    ApproverType = "FA",
                    Email = x.Email,
                    PhoneNo = x.PhoneNumber,
                    Username = x.UserName,
                    Title = x.JobTitle,
                    RequestId = RequestModel.Id
                });

            var ApproverList = Approvers.Select(approver =>
            {
                if (RequestModel.FirstApprover is not null)
                    if (approver.Username == RequestModel.FirstApprover.Username)
                    {
                        approver.Id = RequestModel.FirstApprover.Id;
                    }

                if (RequestModel.SecondApprover is not null)
                    if (approver.Username == RequestModel.SecondApprover.Username)
                    {
                        approver.Id = RequestModel.SecondApprover.Id;
                    }

                if (RequestModel.ThirdApprover is not null)
                    if (approver.Username == RequestModel.ThirdApprover.Username)
                    {
                        approver.Id = RequestModel.ThirdApprover.Id;
                    }

                return approver;

            }).ToList();

            BaseFirstLevelApprovers = ApproverList;

            BaseSecondLevelApprovers = ApproverList;

            if (RequestModel.SecondApprover is not null)
            {
                BaseSecondLevelApprovers.ForEach(x =>
                {
                    x.ApproverType = "SA";
                });
            }

            BaseThirdLevelApprovers = ApproverList;

            if (RequestModel.ThirdApprover is not null)
            {
                BaseThirdLevelApprovers.ForEach(x =>
                {
                    x.ApproverType = "TA";
                });

                BaseFirstLevelApprovers = BaseFirstLevelApprovers.Except(BaseFirstLevelApprovers.Where(x => x.Id == RequestModel.ThirdApprover.Id)).ToList();
                BaseSecondLevelApprovers = BaseSecondLevelApprovers.Except(BaseSecondLevelApprovers.Where(x => x.Id == RequestModel.ThirdApprover.Id)).ToList();
            }

            BaseFirstLevelApprovers = BaseFirstLevelApprovers.Except(BaseFirstLevelApprovers.Where(x => x.Id == RequestModel.SecondApprover.Id).ToList()).ToList();
            BaseSecondLevelApprovers = BaseSecondLevelApprovers.Except(BaseSecondLevelApprovers.Where(x => x.Id == RequestModel.FirstApprover.Id)).ToList();
            BaseThirdLevelApprovers = BaseThirdLevelApprovers.Except(BaseThirdLevelApprovers.Where(x => x.Id == RequestModel.FirstApprover.Id || x.Id == RequestModel.SecondApprover.Id)).ToList();

            RequestUniqueId = RequestModel?.UniqueId;

            if (RequestModel.FirstApprover is not null)
            {
                RequestModel.FirstApproverId = RequestModel.FirstApprover.Id;
            }

            if (RequestModel.SecondApprover is not null)
            {
                RequestModel.SecondApproverId = RequestModel.SecondApprover.Id;
            }

            if (RequestModel.ThirdApprover is not null)
            {
                RequestModel.ThirdApproverId = RequestModel.ThirdApprover.Id;
            }

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

                var isUpdated = IHUDRequest.Update(RequestModel, RequestModel.Variables);

                if (isUpdated)
                {
                    Visibility = false;
                    UpdateButtons[index] = false;
                }
                return true;
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
            GC.SuppressFinalize(this);
        }
    }
}

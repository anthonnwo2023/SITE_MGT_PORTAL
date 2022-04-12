namespace Project.V1.Web.Pages.SiteHalt.Engineer;

public partial class EngineerWorklist
{
    public List<PathInfo> Paths { get; set; }
    [Inject] public IHttpContextAccessor Context { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] public IHUDRequest IHUDRequest { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

    List<SiteHUDRequestModel> HUDEngineerRequests { get; set; } = new();
    public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();

    public List<RequestApproverModel> FirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> SecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> ThirdLevelApprovers { get; set; } = new();

    public int CompleteIndex { get; set; } = -1;
    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public bool Visibility { get; set; } = true;
    public bool DisableBtn { get; set; } = true;
    public SiteHUDRequestModel RequestModel { get; set; }
    private string RequestUniqueId { get; set; }
    public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";
    public string ButtonCompleteIconCss { get; set; } = "fas fa-check m-1 text-white";
    public string ButtonProcessingIconCss { get; set; } = "fas fa-spin fa-spinner m-1 text-white";

    private string ToastPosition { get; set; } = "Right";
    public string ToastTitle { get; set; } = "Error Notification";
    public string ToastContent { get; set; }
    public string ToastCss { get; set; } = "e-toast-danger";

    protected SfToast ToastObj { get; set; }

    private readonly List<ToastModel> Toast = new()
    {
        new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons", Timeout = 0 },
        new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons", Timeout = 0 },
        new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons", Timeout = 0 },
        new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons", Timeout = 0 }
    };

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    protected SfGrid<SiteHUDRequestModel> Grid_Request { get; set; }
    protected SfButton UpdateButton { get; set; }
    protected bool[] CompletingButtons { get; set; }
    protected bool[] UpdateButtons { get; set; }

    public List<string> ToolbarItems = new() { "Search" };

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"Engineer Worklist", Link = "hud/engineer/worklist" },
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
                var userRegionIds = User.Regions.Select(x => x.Id);

                HUDEngineerRequests = (await IHUDRequest.Get(x => (x.ThirdApprover.IsApproved || x.RequestAction == "UnHalt") && x.Status != "Completed", x => x.OrderByDescending(y => y.DateCreated), "Requester.Vendor,FirstApprover,SecondApprover,ThirdApprover,TechTypes")).ToList();
                CompletingButtons = new bool[HUDEngineerRequests.Count];
                UpdateButtons = new bool[HUDEngineerRequests.Count];

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
                StateHasChanged();
            }
        }
    }

    public async void ShowUpdateApproverDialog(MouseEventArgs args, string requestId)
    {
        RequestModel = HUDEngineerRequests.FirstOrDefault(x => x.Id == requestId);
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

    private void DialogClosed(CloseEventArgs args)
    {
        RequestUniqueId = string.Empty;

        DisableBtn = true;
    }

    protected async Task<bool> UpdateApprovers()
    {
        var index = HUDEngineerRequests.IndexOf(RequestModel);
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

    private async void SuccessBtnOnClick()
    {
        Toast[1].Content = ToastContent;

        await ToastObj.Show(Toast[1]);
    }

    private async void ErrorBtnOnClick()
    {
        Toast[2].Content = ToastContent;

        await ToastObj.Show(Toast[2]);
    }

    public void OnToastClickHandler(ToastClickEventArgs args)
    {
        args.ClickToClose = true;
    }

    protected async void CompleteRequest(MouseEventArgs args, SiteHUDRequestModel request)
    {
        CompleteIndex = HUDEngineerRequests.IndexOf(request);
        CompletingButtons[CompleteIndex] = true;

        try
        {
            await IHUDRequest.SetState(request, "SiteHalt");

            if (ProcessAction(request, IHUDRequest))
            {
                //HUDEngineerRequests = (await IHUDRequest.Get(x => (x.ThirdApprover.IsApproved || x.RequestAction == "UnHalt") && x.Status != "Completed", x => x.OrderByDescending(y => y.DateCreated), "Requester.Vendor,FirstApprover,SecondApprover,ThirdApprover,TechTypes")).ToList();

                await Grid_Request.DeleteRecordAsync("Id", request);
                await Grid_Request.EndEdit();
                Grid_Request.Refresh();
                ToastContent = $"Request ({request.UniqueId}) completed successfully.";
                CompletingButtons[CompleteIndex] = false;
                SuccessBtnOnClick();
                return;
            }

            ToastContent = "An error occurred, request could not be updated.";
            CompletingButtons[CompleteIndex] = false;

            await Task.Delay(200);

            ErrorBtnOnClick();
        }
        catch (Exception ex)
        {
            CompletingButtons[CompleteIndex] = false;
            Logger.LogError(ex.Message, new { }, ex);
        }
    }

    private bool ProcessAction<T>(T requestObj, dynamic request) where T : SiteHUDRequestModel
    {
        try
        {
            requestObj.User = User;
            return request.Complete(requestObj, requestObj.Variables);
        }
        catch
        {
            return false;
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
}

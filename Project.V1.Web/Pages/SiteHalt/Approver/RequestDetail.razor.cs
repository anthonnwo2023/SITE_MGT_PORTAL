namespace Project.V1.Web.Pages.SiteHalt.Approver;

public partial class RequestDetail
{
    [Parameter] public string Id { get; set; }
    public List<PathInfo> Paths { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] AppState AppState { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] public IHUDRequest IHUDRequest { get; set; }
    [Inject] protected IUser IUser { get; set; }


    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public SiteHUDRequestModel RequestToApprove { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
    public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();
    private string ApproverClass { get; set; } = "FA";
    public bool DisableButton { get; set; } = false;

    public RequestApproverModel FAApprover { get; set; }
    public RequestApproverModel SAApprover { get; set; }
    public RequestApproverModel TAApprover { get; set; }


    public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";

    private string ToastPosition { get; set; } = "Right";
    public string ToastTitle { get; set; } = "Error Notification";
    public string ToastContent { get; set; }
    public string ToastCss { get; set; } = "e-toast-danger";

    protected SfToast ToastObj { get; set; }

    private readonly List<ToastModel> Toast = new()
    {
        new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" },
        new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons" },
        new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons" },
        new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons" }
    };

    public class ProjectStatus
    {
        public string Name { get; set; }

        [Required]
        public string Status { get; set; }
    }

    public List<ProjectStatus> ProjectStatuses { get; set; } = new()
    {
        new ProjectStatus { Name = "Approve", Status = "Approved" },
        new ProjectStatus { Name = "Disapprove", Status = "Disapproved" }
    };

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public List<string> ToolbarItems = new() { "Search" };

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"Action Request", Link = $"hud/approver/worklist/{Id}" },
            new PathInfo { Name = $"Approver Worklist", Link = "hud/approver/worklist" },
            new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
        };
    }

    protected async Task AuthenticationCheck(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            try
            {
                if (!await UserAuth.IsAutorizedForAsync("Can:ApproveHUD"))
                {
                    NavMan.NavigateTo("access-denied");
                }

                Principal = (await AuthenticationStateTask).User;
                User = await IUser.GetUserByUsername(Principal.Identity.Name);

                RequestToApprove = await IHUDRequest.GetById(x => x.Id == Id, null, "Requester.Vendor,FirstApprover,SecondApprover,ThirdApprover,TechTypes");
                RequestToApprove.FirstApproverId = RequestToApprove?.FirstApprover?.Id;
                RequestToApprove.SecondApproverId = RequestToApprove?.SecondApprover?.Id;
                if (RequestToApprove.ThirdApprover != null)
                    RequestToApprove.ThirdApproverId = RequestToApprove?.ThirdApprover?.Id;

                GetApproverClass();

                RequestToApprove.TempComment = " ";
                RequestToApprove.TempStatus = "";

                RequestToApprove.TechTypeIds = RequestToApprove.TechTypes.Select(x => x.Id).ToArray();

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
                        RequestId = RequestToApprove.Id
                    });

                var ApproverList = Approvers.Select(approver =>
                {
                    if (RequestToApprove.FirstApprover is not null)
                        if (approver.Username == RequestToApprove.FirstApprover.Username)
                        {
                            approver.Id = RequestToApprove?.FirstApprover?.Id;
                        }

                    if (RequestToApprove.SecondApprover is not null)
                        if (approver.Username == RequestToApprove.SecondApprover.Username)
                        {
                            approver.Id = RequestToApprove?.SecondApprover?.Id;
                        }

                    if (RequestToApprove.ThirdApprover is not null)
                        if (approver.Username == RequestToApprove.ThirdApprover.Username)
                        {
                            approver.Id = RequestToApprove?.ThirdApprover?.Id;
                        }

                    return approver;

                }).ToList();

                BaseFirstLevelApprovers = ApproverList;

                BaseSecondLevelApprovers = ApproverList;

                BaseThirdLevelApprovers = ApproverList;

                if (RequestToApprove.SecondApprover is not null)
                {
                    BaseSecondLevelApprovers.ForEach(x =>
                    {
                        x.ApproverType = "SA";
                    });
                }

                if (RequestToApprove.ThirdApprover is not null)
                {
                    BaseThirdLevelApprovers.ForEach(x =>
                    {
                        x.ApproverType = "TA";
                    });

                    BaseFirstLevelApprovers = BaseFirstLevelApprovers.Except(BaseFirstLevelApprovers.Where(x => x.Id == RequestToApprove?.ThirdApprover?.Id)).ToList();
                    BaseSecondLevelApprovers = BaseSecondLevelApprovers.Except(BaseSecondLevelApprovers.Where(x => x.Id == RequestToApprove?.ThirdApprover?.Id)).ToList();
                }

                BaseFirstLevelApprovers = BaseFirstLevelApprovers.Except(BaseFirstLevelApprovers.Where(x => x.Id == RequestToApprove?.SecondApprover?.Id).ToList()).ToList();
                BaseSecondLevelApprovers = BaseSecondLevelApprovers.Except(BaseSecondLevelApprovers.Where(x => x.Id == RequestToApprove?.FirstApprover?.Id)).ToList();
                BaseThirdLevelApprovers = BaseThirdLevelApprovers.Except(BaseThirdLevelApprovers.Where(x => x.Id == RequestToApprove?.FirstApprover?.Id || x.Id == RequestToApprove?.SecondApprover?.Id)).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
            }
        }
    }

    private void GetApproverClass()
    {
        ApproverClass = RequestToApprove.FirstApprover.Username == User.UserName
            ? "FA"
            : RequestToApprove.SecondApprover.Username == User.UserName
                ? "SA"
                : "TA";
    }

    private async void ErrorBtnOnClick()
    {
        Toast[2].Content = ToastContent;

        await this.ToastObj.Show(Toast[2]);
    }

    private void EnableDisableActionButton(bool IsSERRUType)
    {
        return;
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            ButtonIconCss = "fas fa-spin fa-spinner ml-2";
            DisableButton = true;

            await IHUDRequest.SetState(RequestToApprove, "SiteHalt");

            if (ProcessAction(RequestToApprove, IHUDRequest))
            {
                AppState.TriggerRequestRecount();
                StateHasChanged();
                NavMan.NavigateTo("hud/approver/worklist");

                return;
            }

            ToastContent = "An error occurred, request could not be updated.";
            DisableButton = false;

            await Task.Delay(200);

            ErrorBtnOnClick();
        }
        catch (Exception ex)
        {
            ButtonIconCss = "fas fa-paper-plane ml-2";

            string msg = ex.InnerException?.Message ?? ex.Message;

            ToastContent = (msg.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";
            await Task.Delay(200);
            ErrorBtnOnClick();

            Logger.LogError("Error creating request. ", new { }, ex);
        }
    }

    private bool ProcessAction<T>(T requestObj, dynamic request) where T : SiteHUDRequestModel
    {
        try
        {
            requestObj.Status = $"{ApproverClass}{RequestToApprove.TempStatus}";

            if (ApproverClass == "FA")
            {
                requestObj.FirstApprover.IsActioned = true;
                requestObj.FirstApprover.IsApproved = (RequestToApprove.TempStatus == "Approved");
                requestObj.FirstApprover.ApproverComment = RequestToApprove.TempComment;
                requestObj.FirstApprover.DateActioned = DateTimeOffset.UtcNow.DateTime;
                requestObj.FirstApprover.DateApproved = (requestObj.FirstApprover.IsApproved)
                    ? DateTimeOffset.UtcNow.DateTime
                    : DateTime.MinValue;

                if (requestObj.FirstApprover.IsApproved)
                {
                    return request.Approve(requestObj, RequestToApprove.Variables);
                }

                requestObj.FirstApprover.DateApproved = DateTime.MinValue;

                return request.Disapprove(requestObj, RequestToApprove.Variables, requestObj.FirstApprover);
            }
            if (ApproverClass == "SA")
            {
                requestObj.SecondApprover.IsActioned = true;
                requestObj.SecondApprover.IsApproved = (RequestToApprove.TempStatus == "Approved");
                requestObj.SecondApprover.ApproverComment = RequestToApprove.TempComment;
                requestObj.SecondApprover.DateActioned = DateTimeOffset.UtcNow.DateTime;
                requestObj.SecondApprover.DateApproved = (requestObj.SecondApprover.IsApproved)
                    ? DateTimeOffset.UtcNow.DateTime
                    : DateTime.MinValue;

                if (requestObj.SecondApprover.IsApproved)
                {
                    return request.Approve(requestObj, RequestToApprove.Variables);
                }

                requestObj.SecondApprover.DateApproved = DateTime.MinValue;

                return request.Disapprove(requestObj, RequestToApprove.Variables, requestObj.SecondApprover);
            }
            if (ApproverClass == "TA")
            {
                requestObj.ThirdApprover.IsActioned = true;
                requestObj.ThirdApprover.IsApproved = (RequestToApprove.TempStatus == "Approved");
                requestObj.ThirdApprover.ApproverComment = RequestToApprove.TempComment;
                requestObj.ThirdApprover.DateActioned = DateTimeOffset.UtcNow.DateTime;
                requestObj.ThirdApprover.DateApproved = (requestObj.ThirdApprover.IsApproved)
                    ? DateTimeOffset.UtcNow.DateTime
                    : DateTime.MinValue;

                if (requestObj.ThirdApprover.IsApproved)
                {
                    return request.Approve(requestObj, RequestToApprove.Variables);
                }

                requestObj.ThirdApprover.DateApproved = DateTime.MinValue;

                return request.Disapprove(requestObj, RequestToApprove.Variables, requestObj.ThirdApprover);
            }


            return false;

        }
        catch
        {
            return false;
        }
    }
}

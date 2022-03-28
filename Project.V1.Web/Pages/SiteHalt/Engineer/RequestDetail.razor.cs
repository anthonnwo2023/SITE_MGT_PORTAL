namespace Project.V1.Web.Pages.SiteHalt.Engineer;

public partial class RequestDetail : IDisposable
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
    public SiteHUDRequestModel RequestToAction { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
    public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();
    public UploadFiles UploadFiles { get; set; }
    public FileStream Filestream { get; set; }
    public string UploadPath { get; set; }
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
            new PathInfo { Name = $"Action Request", Link = $"hud/engineer/worklist/{Id}" },
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

                RequestToAction = (await IHUDRequest.GetById(x => x.Id == Id));
                RequestToAction.TempComment = " ";
                RequestToAction.TempStatus = RequestToAction.Status;

                RequestToAction.FirstApproverId = RequestToAction.FirstApprover.Id;
                RequestToAction.SecondApproverId = RequestToAction.SecondApprover.Id;
                RequestToAction.ThirdApproverId = RequestToAction.ThirdApprover.Id;

                RequestToAction.TechTypeIds = RequestToAction.TechTypes.Select(x => x.Id).ToArray();

                BaseFirstLevelApprovers = (await UserManager.GetUsersInRoleAsync("HUD RF SM")).Select(x => new RequestApproverModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Fullname = x.Fullname,
                    ApproverType = "FA",
                    Email = x.Email,
                    PhoneNo = x.PhoneNumber,
                    Username = x.UserName,
                    Title = x.JobTitle,
                    RequestId = RequestToAction.Id
                }).ToList();

                BaseFirstLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestToAction.FirstApprover.Username)
                    {
                        x.Id = RequestToAction.FirstApprover.Id;
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
                        RequestId = RequestToAction.Id
                    }).ToList();

                BaseSecondLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestToAction.SecondApprover.Username)
                    {
                        x.Id = RequestToAction.SecondApprover.Id;
                    }
                });

                BaseThirdLevelApprovers = BaseSecondLevelApprovers;

                BaseThirdLevelApprovers.ForEach(x =>
                {
                    if (x.Username == RequestToAction.ThirdApprover.Username)
                    {
                        x.Id = RequestToAction.ThirdApprover.Id;
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

    private void ResetUpload()
    {
        if (File.Exists(UploadPath))
        {
            Filestream.Close();
            UploadFiles.Stream.Close();

            File.Delete(UploadPath);
        }
    }

    private void OnClear(ClearingEventArgs args)
    {
        if (args.FilesData.Count > 0)
            ResetUpload();
    }

    private void OnRemove(RemovingEventArgs args)
    {
        if (args.FilesData.Count > 0)
            ResetUpload();
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
        var variables = new Dictionary<string, object> { { "User", RequestToAction.Requester.Username }, { "App", "acceptance" } };

        try
        {
            ButtonIconCss = "fas fa-spin fa-spinner ml-2";

            await IHUDRequest.SetState(RequestToAction, "SiteHalt");

            if (ProcessAction(RequestToAction, variables, IHUDRequest))
            {
                StateHasChanged();
                NavMan.NavigateTo("hud/engineer/worklist");

                return;
            }

            ToastContent = "An error occurred, request could not be updated.";

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

    private bool ProcessAction<T>(T requestObj, Dictionary<string, object> variables, dynamic request) where T : SiteHUDRequestModel
    {
        try
        {
            return request.Complete(requestObj, variables, string.Empty);
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        RequestToAction.Dispose();
    }
}

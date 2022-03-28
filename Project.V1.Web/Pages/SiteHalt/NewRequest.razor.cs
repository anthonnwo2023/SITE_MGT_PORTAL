﻿using Project.V1.Web.Request;

namespace Project.V1.Web.Pages.SiteHalt;

public partial class NewRequest : IDisposable
{
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IRequestListObject IRequestList { get; set; }
    [Inject] protected IHUDRequest IHUDRequest { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public string RequestSiteIds { get; set; }
    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public List<PathInfo> Paths { get; set; }
    public string UploadFileName { get; set; }
    public string UploadPath { get; set; }
    public List<FilesManager> UploadedRequestFiles { get; set; } = new();
    public SiteHUDRequestModel RequestModel { get; set; }
    public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();

    private string ToastPosition { get; set; } = "Right";
    public string ToastTitle { get; set; } = "Error Notification";
    public string ToastContent { get; set; }
    public string ToastCss { get; set; } = "e-toast-danger";

    public bool DisableButton { get; set; } = false;
    public string UploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

    protected SfToast ToastObj { get; set; }

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"New Request", Link = "hud/request" },
            new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
        };
    }

    protected async Task AuthenticationCheck(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            try
            {
                Logger.LogInformation("Loading create site halt & unhult request page", new { });

                if (!await UserAuth.IsAutorizedForAsync("Can:AddRequest"))
                {
                    NavMan.NavigateTo("access-denied");
                }

                await InitializeForm();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error creating request", new { }, ex);
            }
        }
    }

    private readonly List<ToastModel> Toast = new()
    {
        new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons", Timeout = 0 },
        new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons", Timeout = 0 },
        new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons", Timeout = 0 },
        new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons", Timeout = 0 }
    };

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

    private void EnableDisableActionButton(bool shouldEnable)
    {
        DisableButton = shouldEnable;

        StateHasChanged();
    }

    private static void OnClear(ClearingEventArgs args)
    {
        return;
    }

    private static void OnRemove(RemovingEventArgs args)
    {
        return;
    }

    private async Task<bool> OnFileUploadChange(UploadChangeEventArgs args, string type)
    {
        UploadFiles UploadFile = args.Files.FirstOrDefault();

        string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\{type}\\");

        if (!Directory.Exists(pathBuilt))
        {
            Directory.CreateDirectory(pathBuilt);
        }

        string ext = Path.GetExtension(UploadFile.FileInfo.Name);
        var filenameWOSpecialXters = Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters();
        UploadFileName = "HUD_FN_" + filenameWOSpecialXters + "_" + DateTime.Now.AddHours(1).ToString() + ext;
        UploadPath = Path.Combine(pathBuilt, UploadFileName.RemoveSpecialCharacters());

        var fileExists = UploadedRequestFiles.Where(x => x.Filename != null).Any(c => c.Filename.Contains(Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters()));

        if (!fileExists)
        {
            UploadedRequestFiles.Add(GetFileStreamObject(UploadFile, type, fileExists));
        }
        else
        {
            GetFileStreamObject(UploadFile, type, fileExists);
        }

        return await Task.Run(() => true);
    }

    private FilesManager GetFileStreamObject(UploadFiles UFile, string type, bool fileExists = true)
    {
        if (fileExists == false)
        {
            return new FilesManager
            {
                Filestream = new(UploadPath, FileMode.Create, FileAccess.Write),
                Index = UploadedRequestFiles.Count,
                Filename = Path.GetFileName(UploadPath),
                UploadFile = UFile,
                UploadPath = UploadPath,
                UploadType = type,
            };
        }

        var file = UploadedRequestFiles.FirstOrDefault(x => x.Filename.ToUpper() == Path.GetFileName(UploadPath).ToUpper());

        file.Filestream = new(UploadPath, FileMode.Create, FileAccess.Write);
        file.Filename = Path.GetFileName(UploadPath);
        file.UploadFile = UFile;
        file.UploadPath = UploadPath;
        file.UploadType = type;

        return null;
    }

    private async void SaveLargeSiteIDToFile()
    {
        var fileName = $"{HelperFunctions.GenerateIDUnique("HUD-SID")}.txt";

        var (isWritten, message) = TextFileExtension.Initialize("HUD_SiteID", fileName).GetStream().WriteToFile(RequestModel.SiteIds);

        //var (isWritten, message) = await TextFileExtension.Initialize("HUD_SiteID", fileName).GetStream().WriteToFile(RequestModel.SiteIds);

        if (isWritten)
        {
            RequestModel.HasLargeSiteIdCount = true;
            RequestModel.SiteIds = fileName;
        }
        else
        {
            await ThrowError(message);
            UploadIconCss = "fas fa-paper-plane ml-2";
            return;
        }
    }

    private async Task SaveSupportingDocument()
    {
        FilesManager file = UploadedRequestFiles.OrderByDescending(x => x.Index).FirstOrDefault(x => x.UploadType == "HUD");

        if (file?.UploadFile != null)
        {
            string ext = Path.GetExtension(file.UploadFile.FileInfo.Name);

            await FileUploader.StartUpload(true, file, true)
                .ContinueWith(async (response) =>
                {
                    RequestModel.SupportingDocument = Path.GetFileName((await response).filePath);

                    if ((await response).uploadResp.Length != 0 || (await response).uploadError.Length != 0)
                    {
                        if ((await response).uploadError.Length > 0) await ThrowError((await response).uploadError);
                        UploadIconCss = "fas fa-paper-plane ml-2";
                        return;
                    }
                });
        }
    }

    protected async Task HandleValidSubmit()
    {
        try
        {
            RequestSiteIds = RequestModel.SiteIds;

            if (RequestModel.SiteIds.Length > 1999)
            {
                SaveLargeSiteIDToFile();
            }

            await SaveSupportingDocument()
                .ContinueWith((result) =>
                {
                    ProcessRequest().Wait();
                });
        }
        catch (Exception ex)
        {
            RequestModel.SiteIds = RequestSiteIds;
            DisableButton = false;
            UploadIconCss = "fas fa-paper-plane ml-2";
            Logger.LogError("Error creating request. ", new { }, ex);
        }
    }

    private async Task ThrowSuccess(string message)
    {
        ToastContent = message;
        await Task.Delay(200);
        SuccessBtnOnClick();
    }

    private async Task ThrowError(string message)
    {
        ToastContent = message;
        await Task.Delay(200);
        ErrorBtnOnClick();
    }

    protected void ChangeRequestType(SiteHUDRequestModel requestModel)
    {
        RequestModel = RequestModel.CopyTo(new HUDRequest(IHUDRequest, IRequestList.User));
        RequestModel.User = IRequestList.User;
    }

    private async Task InitializeForm()
    {
        UploadIconCss = "fas fa-paper-plane ml-2";
        Principal = (await AuthenticationStateTask).User;

        await IRequestList.Initialize(Principal, "HUDObject");

        RequestModel = new();
        RequestModel.TempStatus = "Pending";
        RequestModel.TempComment = " ";

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

        BaseThirdLevelApprovers = BaseSecondLevelApprovers;
    }

    private async Task ProcessRequest()
    {
        DisableButton = true;
        UploadIconCss = "fas fa-spin fa-spinner ml-2";

        RequestModel.Id = Guid.NewGuid().ToString();

        if (RequestModel.RequestAction != "UnHalt")
        {
            RequestModel.FirstApprover.RequestId = RequestModel.Id;
            RequestModel.SecondApprover.RequestId = RequestModel.Id;
            RequestModel.ThirdApprover.RequestId = RequestModel.Id;
        }

        RequestModel.TechTypes = IRequestList.TechTypes.Where(x => RequestModel.TechTypeIds.Contains(x.Id)).ToList();

        var (Saved, Message) = await RequestModel.Create();

        if (!Saved)
        {
            if (Message.Length > 0) await ThrowError(Message);
            UploadIconCss = "fas fa-paper-plane ml-2";
            return;
        }

        var siteIds = RequestModel.SiteIds;
        RequestModel.SiteIds = RequestSiteIds;

        await RequestModel.SetCreateState(null);
        RequestModel.SiteIds = siteIds;

        DisableButton = false;
        await ThrowSuccess(Message);
        await InitializeForm();
    }

    public void Dispose()
    {
        RequestModel.Dispose();
    }
}
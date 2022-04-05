using Project.V1.Web.Request;

namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class RequestDetail : IDisposable
    {
        [Parameter] public string Id { get; set; }
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IHUDRequest IHUDRequest { get; set; }
        [Inject] protected IRequestListObject IRequestList { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] AppState AppState { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public SiteHUDRequestModel RequestToUpdate { get; set; }
        public string RequestSiteIds { get; set; }
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
        public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
        public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();
        public List<FilesManager> UploadedRequestFiles { get; set; } = new();
        public string UploadFileName { get; set; }
        public string UploadPath { get; set; }

        public bool DisableButton { get; set; } = false;
        public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";

        public RequestApproverModel FAApprover { get; set; }
        public RequestApproverModel SAApprover { get; set; }
        public RequestApproverModel TAApprover { get; set; }

        public List<string> ToolbarItems = new() { "Search" };

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected SfToast ToastObj { get; set; }

        private readonly List<ToastModel> Toast = new()
        {
            new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" },
            new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons" },
            new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons" },
            new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons" }
        };

        private async void ErrorBtnOnClick()
        {
            Toast[2].Content = ToastContent;

            await ToastObj.Show(Toast[2]);
        }
        private async Task ThrowError(string message)
        {
            ToastContent = message;
            await Task.Delay(200);
            ErrorBtnOnClick();
        }


        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Action Request", Link = $"hud/engineer/worklist/{Id}" },
                new PathInfo { Name = $"Engineer Worklist", Link = "hud/engineer/worklist" },
                new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
            };
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

        private async void SaveLargeSiteIDToFile()
        {
            var fileName = $"{HelperFunctions.GenerateIDUnique("HUD-SID")}.txt";
            RequestSiteIds = RequestToUpdate.SiteIds;
            var (isWritten, message) = TextFileExtension.Initialize("HUD_SiteID", fileName).GetStream().WriteToFile(RequestToUpdate.SiteIds);

            if (isWritten)
            {
                RequestToUpdate.HasLargeSiteIdCount = true;
                RequestToUpdate.SiteIds = fileName;
            }
            else
            {
                await ThrowError(message);
                ButtonIconCss = "fas fa-paper-plane ml-2";
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
                        RequestToUpdate.SupportingDocument = Path.GetFileName((await response).filePath);

                        if ((await response).uploadResp.Length != 0 || (await response).uploadError.Length != 0)
                        {
                            if ((await response).uploadError.Length > 0) await ThrowError((await response).uploadError);
                            return;
                        }
                    });
            }
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:ReworkRequest"))
                    {
                        NavMan.NavigateTo("access-denied");
                    }

                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);

                    RequestToUpdate = await IHUDRequest.GetById(x => x.Id == Id, null, "FirstApprover,SecondApprover,ThirdApprover,TechTypes");
                    RequestToUpdate.TempComment = " ";
                    RequestToUpdate.TempStatus = RequestToUpdate.Status;

                    RequestToUpdate.FirstApproverId = RequestToUpdate.FirstApprover.Id;
                    RequestToUpdate.SecondApproverId = RequestToUpdate.SecondApprover.Id;
                    RequestToUpdate.ThirdApproverId = RequestToUpdate.ThirdApprover.Id;

                    RequestToUpdate.TechTypeIds = RequestToUpdate.TechTypes.Select(x => x.Id).ToArray();

                    BaseFirstLevelApprovers = (await UserManager.GetUsersInRoleAsync("HUD RF SM")).Select(x => new RequestApproverModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Fullname = x.Fullname,
                        ApproverType = "FA",
                        Email = x.Email,
                        PhoneNo = x.PhoneNumber,
                        Username = x.UserName,
                        Title = x.JobTitle,
                        RequestId = RequestToUpdate.Id
                    }).ToList();

                    BaseFirstLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == RequestToUpdate.FirstApprover.Username)
                        {
                            x.Id = RequestToUpdate.FirstApprover.Id;
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
                            RequestId = RequestToUpdate.Id
                        }).ToList();

                    BaseSecondLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == RequestToUpdate.SecondApprover.Username)
                        {
                            x.Id = RequestToUpdate.SecondApprover.Id;
                        }
                    });

                    BaseThirdLevelApprovers = BaseSecondLevelApprovers;

                    BaseThirdLevelApprovers.ForEach(x =>
                    {
                        if (x.Username == RequestToUpdate.ThirdApprover.Username)
                        {
                            x.Id = RequestToUpdate.ThirdApprover.Id;
                        }
                        x.ApproverType = "TA";
                    });
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                    StateHasChanged();
                }
            }
        }

        protected async Task HandleValidSubmit(EditContext context)
        {
            try
            {
                ButtonIconCss = "fas fa-spin fa-spinner ml-2";
                DisableButton = true;


                await IHUDRequest.SetState(RequestToUpdate, "SiteHalt");

                if (RequestToUpdate.SiteIds.Length > 1999)
                {
                    SaveLargeSiteIDToFile();
                }

                await SaveSupportingDocument()
                    .ContinueWith((result) =>
                    {
                        ProcessAction(RequestToUpdate, IHUDRequest).Wait();
                    });
            }
            catch (Exception ex)
            {
                Logger.LogError("Error creating request. ", new { }, ex);
                await ThrowError($"{ex.Message} {ex.InnerException}");
            }
        }

        private async Task ProcessAction<T>(T requestObj, dynamic request) where T : SiteHUDRequestModel
        {
            try
            {
                requestObj.Status = "Restarted";
                RequestToUpdate.TechTypes = IRequestList.TechTypes.Where(x => RequestToUpdate.TechTypeIds.Contains(x.Id)).ToList();

                requestObj.FirstApprover.IsActioned = false;
                requestObj.SecondApprover.IsActioned = false;
                requestObj.ThirdApprover.IsActioned = false;

                var isUpdated = request.Restart(requestObj, RequestToUpdate.Variables);

                if (isUpdated)
                {
                    AppState.TriggerRequestRecount();
                    NavMan.NavigateTo("hud/worklist");

                    return;
                }

                DisableButton = false;
                RequestToUpdate.SiteIds = RequestSiteIds;
                await ThrowError("An error occurred, request could not be updated.");
                ButtonIconCss = "fas fa-paper-plane ml-2";
            }
            catch
            {
                DisableButton = false;
                RequestToUpdate.SiteIds = RequestSiteIds;
                await ThrowError("An error occurred, request could not be updated.");
                ButtonIconCss = "fas fa-paper-plane ml-2";
            }
        }

        public void Dispose()
        {
            RequestToUpdate.Dispose();
        }
    }
}

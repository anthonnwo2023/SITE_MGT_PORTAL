﻿namespace Project.V1.Web.Pages.Acceptance.Engineer
{
    public partial class ActionRequest : IDisposable
    {
        [Parameter] public string Id { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] AppState AppState { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<ProjectModel> RRUTypes { get; set; }
        public RequestViewModel RequestModel { get; set; }

        public bool EnableSpecial { get; set; }
        public DateTime MinDateTime { get; set; }
        public DateTime MaxDateTime { get; set; }

        public InputModel Input { get; set; } = new();
        public string PageText { get; set; } = "Action";
        public string BtnText { get; set; } = "Action Request";
        public string RequestStatus { get; set; }
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        public FileStream Filestream { get; set; }
        public UploadFiles UploadFiles { get; set; }
        public string UploadPath { get; set; }
        public string FilePath { get; set; }
        public string UploadFileName { get; set; }
        public bool DisableCreateButton { get; set; } = false;

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";

        protected SfUploader SF_Uploader { get; set; }
        protected SfToast ToastObj { get; set; }

        private readonly List<ToastModel> Toast = new()
        {
            new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons" },
            new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons" },
            new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons" },
            new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons" }
        };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Action Request", Link = $"acceptance/engineer/worklist/{Id}" },
                new PathInfo { Name = $"Engineer Worklist", Link = "acceptance/engineer/worklist" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async void SuccessBtnOnClick()
        {
            Toast[1].Content = ToastContent;

            await this.ToastObj.Show(Toast[1]);
        }

        public class ProjectStatus
        {
            public string Name { get; set; }

            [Required]
            public string Status { get; set; }
        }

        public List<ProjectStatus> ProjectStatuses { get; set; } = new()
        {
            new ProjectStatus { Name = "Accept", Status = "Accepted" },
            new ProjectStatus { Name = "Reject", Status = "Rejected" }
        };

        public class NigerianState
        {
            public string Name { get; set; }
        }

        public ActionReason Reason { get; set; } = new()
        {
            Id = Guid.NewGuid().ToString(),
        };

        private static readonly string[] States = new string[]
         {
            "Abia", "Adamawa", "Akwa Ibom", "Anambra", "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta", "Ebonyi", "Edo", "Ekiti",
            "Enugu", "FCT - Abuja", "Gombe", "Imo", "Jigawa", "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara","Lagos", "Nasarawa", "Niger",
            "Ogun", "Ondo", "Osun", "Oyo", "Plateau", "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
         };

        public List<NigerianState> NigerianStates { get; set; } = States.Select(x => new NigerianState { Name = x.ToUpper() }).ToList();

        private async void ErrorBtnOnClick()
        {
            Toast[2].Content = ToastContent;

            await this.ToastObj.Show(Toast[2]);
        }

        private void ResetUpload()
        {
            RequestModel.EngineerRejectReport = "";
            DisableCreateButton = true;

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
            {
                ResetUpload();
            }
        }

        private void OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
            {
                ResetUpload();
            }
        }

        public async Task OnTechChange(List<SpectrumViewModel> spectrums)
        {
            //Spectrums = spectrums;

            await Task.CompletedTask;
        }

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);

            RequestModel = new();
            RequestModel.EngineerAssigned = new();
            Input = new();
            RequestModel.DateSubmitted = DateTime.Now;
            RequestModel.IntegratedDate = DateTime.Now;
        }

        public class InputModel
        {
            public int EngineerRejectReport { get; set; }

            [RequiredWhen(nameof(Status), "Rejected", AllowEmptyStrings = false, ErrorMessage = "The Comment is required.")]
            public string Comment { get; set; }

            [Required]
            public string Status { get; set; }
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    Logger.LogInformation("Loading action request page", new { });

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:UpdateRequest"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        await InitializeForm();

                        RequestModel = await IRequest.GetById(x => x.Id == Id, null, RequestModel.Navigations);
                        RequestModel.EngineerAssigned.DateApproved = DateTime.UtcNow;

                        //DateTime dt = DateTime.Now;
                        ////EnableSpecial = (dt.Day == DateTime.DaysInMonth(dt.Year, dt.Month));                      
                        //int lastDayOfPrevMth = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
                        //EnableSpecial = Enumerable.Range(1, 2).Contains(dt.Day);

                        //MinDateTime = new DateTime(dt.Year, dt.Month - 1, lastDayOfPrevMth);
                        //MaxDateTime = new DateTime(dt.Year, dt.Month, 2);


                        DateTime dt = DateTime.Now;                     
                        EnableSpecial = Enumerable.Range(1, 2).Contains(dt.Day);
                      
                        int lastDayOfPrevMth = 0;
                        if (DateTime.Now.Month == 1 && EnableSpecial == true)
                        {
                            int YearNo = int.Parse(dt.AddYears(-1).ToString("yyyy"));
                            int MonthNo = 12;

                            lastDayOfPrevMth = DateTime.DaysInMonth(YearNo, MonthNo);
                            MinDateTime = new DateTime(YearNo, MonthNo, lastDayOfPrevMth);
                            MaxDateTime = new DateTime(dt.Year, dt.Month, 2);
                        }
                        else
                        { 
                            lastDayOfPrevMth = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
                            EnableSpecial = Enumerable.Range(1, 2).Contains(dt.Day);

                            MinDateTime = new DateTime(dt.Year, dt.Month - 1, lastDayOfPrevMth);
                            MaxDateTime = new DateTime(dt.Year, dt.Month, 2);
                        }

                        return;
                    }

                    //await InitializeForm();

                    NavMan.NavigateTo("access-denied");
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error actioning request", new { }, ex);
                }
            }
        }

        protected async Task HandleValidSubmit()
        {
            var variables = new Dictionary<string, object> { { "User", RequestModel.Requester.Username }, { "App", "acceptance" } };

            try
            {
                DisableCreateButton = true;
                BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";

                if (UploadFiles != null)
                {
                    List<string> imageExts = new() { ".jpeg", ".jpg", ".png" };
                    string ext = Path.GetExtension(UploadFiles.FileInfo.Name);

                    bool allowedExtension = ExcelProcessor.IsAllowedExtReject(ext, false);

                    if (!imageExts.Contains(ext?.ToLower()))
                    {
                        (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension);

                        if (uploadResp.Length != 0 || uploadError.Length != 0)
                        {
                            ToastContent = "Error occured, could not upload file";

                            await Task.Delay(200);
                            ErrorBtnOnClick();

                            return;
                        }
                    }
                    else
                    {
                        (string error, string path) = await UploadFile();
                    }
                }

                //RequestModel.Status = RequestStatus;

                await IRequest.SetState(RequestModel);

                if (ProcessAction(RequestModel, variables, IRequest))
                {
                    AppState.TriggerRequestRecount();

                    StateHasChanged();
                    NavMan.NavigateTo("acceptance/engineer/worklist");

                    return;
                }

                ToastContent = "An error occurred, request could not be updated.";

                await Task.Delay(200);

                ErrorBtnOnClick();
            }
            catch (Exception ex)
            {
                if (UploadFiles != null)
                {
                    await SF_Uploader.ClearAll();
                }

                BulkUploadIconCss = "fas fa-paper-plane ml-2";

                string msg = ex.InnerException?.Message ?? ex.Message;

                ToastContent = (msg.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";
                await Task.Delay(200);
                ErrorBtnOnClick();

                Logger.LogError("Error creating request. ", new { }, ex);
            }
        }

        private bool ProcessAction<T>(T requestObj, Dictionary<string, object> variables, dynamic request) where T : RequestViewModel
        {
            try
            {
                requestObj.EngineerAssigned.IsActioned = true;
                requestObj.EngineerAssigned.Fullname = User.Fullname;
                requestObj.EngineerAssigned.Email = User.Email;
                requestObj.EngineerAssigned.PhoneNo = User.PhoneNumber;
                requestObj.EngineerAssigned.IsApproved = (Input.Status == "Accepted");
                requestObj.EngineerAssigned.ApproverComment = Input.Comment;
                requestObj.EngineerAssigned.DateAssigned = DateTimeOffset.UtcNow.DateTime;
                requestObj.EngineerAssigned.DateActioned = DateTimeOffset.UtcNow.DateTime;
                requestObj.Status = Input.Status;

                if (requestObj.EngineerAssigned.IsApproved)
                {
                    requestObj.EngineerAssigned.DateApproved = (requestObj.EngineerAssigned.DateApproved.Date == DateTime.Now.Date) ? DateTimeOffset.UtcNow.DateTime : requestObj.EngineerAssigned.DateApproved;

                    Log.Information("Approved Block =>" + requestObj + " " + "Variables =>" + variables + " " + "Request =>" + " " + request);

                    return request.Accept(requestObj, variables);
                }

                requestObj.EngineerAssigned.DateApproved = DateTime.MinValue;

                Log.Information("Reject Block =>" + requestObj + " " + "Variables =>" + variables + " " + "Request =>" + " " + request);

                return request.Reject(requestObj, variables, requestObj.EngineerAssigned.ApproverComment);
            }
            catch (Exception ex)
            {
                Log.Error("Ex =>" + ex + " " + "InnerException =>" + ex.InnerException + " " + "StackTrace =>" + " " + ex.StackTrace);
                return false;
            }
        }

        private async Task<(string, string, string)> StartUpload(bool allowedExtension)
        {
            switch (allowedExtension)
            {
                case true:
                    {
                        (string error, string path) = await UploadFile();

                        if (path.Length > 0)
                        {
                            return ("", path, error);
                        }

                        return ($"An Error occurred, could not save specified file", "", error);
                    }

                default:
                    return ("Invalid file type. Upload only excel documents. (.xls and .xlsx only)", "", "");
            }
        }

        private async Task<(string, string)> UploadFile()
        {
            try
            {
                await Task.Run(() => UploadFiles.Stream.WriteTo(Filestream));
                Filestream.Close();
                UploadFiles.Stream.Close();

                return ("", UploadPath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
                return (ex.Message, "");
            }
        }

        private async Task<bool> OnFileUploadChange(Syncfusion.Blazor.Inputs.UploadChangeEventArgs args, string type)
        {
            UploadFiles = args.Files.First();

            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\{type}\\");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            string ext = Path.GetExtension(UploadFiles.FileInfo.Name);
            UploadFileName = "SA_Reject_" + UploadFiles.FileInfo.Name.RemoveSpecialCharacters() + "_" + DateTime.Now.AddHours(1).ToString().Replace("/", "_").Replace(":", "").Replace(" ", "") + ext;
            UploadPath = Path.Combine(pathBuilt, UploadFileName);

            FilePath = Path.GetFileName(UploadPath);
            RequestModel.EngineerRejectReport = FilePath;

            Filestream = new(UploadPath, FileMode.Create, FileAccess.Write);

            return await Task.Run(() => true);
        }

        private void EnableDisableActionButton(bool IsSERRUType)
        {
            DisableCreateButton = false;
        }

        private void IsSEValid(bool SEValid)
        {

        }

        public void Dispose()
        {
            //RequestModel.Status = RequestStatus;

            GC.SuppressFinalize(this);
        }
    }
}

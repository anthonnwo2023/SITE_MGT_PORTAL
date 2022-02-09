using Project.V1.Web.Request;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class NewRequest : IDisposable
    {
        [Parameter] public string Id { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IRequestListObject IRequestList { get; set; }
        [Inject] AppState AppState { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public RequestViewModel RequestModel { get; set; }
        public InputModel Input { get; set; } = new();
        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create Request";
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";
        public string SEUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        private string BulkUploadColumnError { get; set; }
        private string BulkUploadError { get; set; }
        private string SaveError { get; set; }

        public double MaxFileSize { get; set; } = 25000000;
        public List<SfUploader> BulkUploadRRUSSVFiles = new();
        public List<FilesManager> UploadedRequestFiles { get; set; } = new();
        public List<FileStream> Filestreams { get; set; }
        public List<UploadFiles> UploadFiles { get; set; }
        public string UploadPath { get; set; }
        public string FilePath { get; set; }
        public string UploadFileName { get; set; }
        public bool DisableSEButton { get; set; } = true;
        public bool IsRRUType { get; set; } = false;
        public bool DisableBUButton { get; set; } = true;
        public DateTime MaxDateTime { get; set; }
        public DateTime MinDateTime { get; set; }

        public bool SSVUploadSelected { get; set; } = true;
        public bool WaiverUploadSelected { get; set; }
        public bool BulkWaiverUploadSelected { get; set; }

        private string ToastPosition { get; set; } = "Right";
        public string ToastTitle { get; set; } = "Error Notification";
        public string ToastContent { get; set; }
        public string ToastCss { get; set; } = "e-toast-danger";

        protected SfUploader SF_Uploader { get; set; }
        protected SfButtonGroup SingleSelector { get; set; }
        protected ButtonGroupButton SingleSSVSelector { get; set; }
        protected ButtonGroupButton SingleWaiverSelector { get; set; }
        protected SfUploader SF_WaiverUploader { get; set; }
        public string WaiverUpload { get; set; }
        protected SfUploader SFBulkAcceptance_Uploader { get; set; }
        protected SfUploader SFBulkASSV_Uploader { get; set; }
        protected SfToast ToastObj { get; set; }
        protected bool SingleEntrySelected { get; set; }
        protected bool SingleEntryValid { get; set; }
        protected bool ShowInvalidDialog { get; set; }
        protected bool BulkUploadSelected { get; set; }
        public bool ShowOffPeakNotice { get; set; }
        public List<RequestViewModel> BulkRequestRRUData { get; set; }
        public List<RequestViewModel> BulkRequestInvalidData { get; set; } = new();
        public int BulkRequestRRUCount { get; set; } = 0;
        public int BulkOptionalSSVCount { get; set; } = 0;
        public (string error, List<RequestViewModel> requests) BulkUploadData { get; set; }
        public string BulkUploadPath { get; set; }
        public double TotalFileCount { get; set; } = 0;
        public int MaxFileCount { get; set; } = 50;
        public double TotalFileSize { get; set; } = 0;
        public double MaxUploadFileSize { get; set; } = 350;
        public DateTime PeakStart { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 0, 0);
        public DateTime PeakEnd { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 05, 00, 00).AddDays(1);

        public List<BoolDropDown> BoolDrops { get; set; } = new()
        {
            new BoolDropDown { Name = "Yes" },
            new BoolDropDown { Name = "No" }
        };

        private bool IsPeakPeriod()
        {
            return DateTime.Now > PeakStart && DateTime.Now < PeakEnd;
        }

        private static readonly string[] States = new string[]
        {
            "Abia", "Adamawa", "Akwa Ibom", "Anambra", "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta", "Ebonyi", "Edo", "Ekiti",
            "Enugu", "FCT - Abuja", "Gombe", "Imo", "Jigawa", "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara","Lagos", "Nasarawa", "Niger",
            "Ogun", "Ondo", "Osun", "Oyo", "Plateau", "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
        };

        public List<NigerianState> NigerianStates { get; set; } = States.Select(x => new NigerianState { Name = x }).ToList();

        public class BoolDropDown
        {
            public string Name { get; set; }
        }

        public class NigerianState
        {
            public string Name { get; set; }
        }

        private readonly List<ToastModel> Toast = new()
        {
            new ToastModel { Title = "Warning!", Content = "There was a problem with your network connection.", CssClass = "e-toast-warning", Icon = "e-warning toast-icons", Timeout = 0 },
            new ToastModel { Title = "Success!", Content = "Your message has been sent successfully.", CssClass = "e-toast-success", Icon = "e-success toast-icons", Timeout = 0 },
            new ToastModel { Title = "Error!", Content = "A problem has been occurred while submitting your data.", CssClass = "e-toast-danger", Icon = "e-error toast-icons", Timeout = 0 },
            new ToastModel { Title = "Information!", Content = "Please read the comments carefully.", CssClass = "e-toast-info", Icon = "e-info toast-icons", Timeout = 0 }
        };

        private readonly Dictionary<string, object> htmlattributeFileUpload = new()
        {
            { "padding", "5px 2px 5px 2px" },
        };

        private async Task CheckIfSEValid()
        {
            if (RequestModel.SiteId != null && RequestModel.SpectrumId != null)
            {
                SingleEntryValid = await IRequest.GetValidRequest(RequestModel);

                if (!SingleEntryValid)
                {
                    BulkRequestInvalidData.Add(RequestModel);
                }
            }
        }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"New Request", Link = "acceptance/request" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async void SuccessBtnOnClick()
        {
            Toast[1].Content = ToastContent;

            await this.ToastObj.Show(Toast[1]);
        }

        private async void ErrorBtnOnClick()
        {
            Toast[2].Content = ToastContent;

            await this.ToastObj.Show(Toast[2]);
        }

        private void ResetUpload(Syncfusion.Blazor.Inputs.FileInfo file)
        {
            var nameCompare = Path.GetFileNameWithoutExtension(file.Name).RemoveSpecialCharacters();
            var fname = (nameCompare.Length > 3) ? Path.GetFileNameWithoutExtension(file.Name.Split('(')[0]).RemoveSpecialCharacters() : Path.GetFileNameWithoutExtension(file.Name).RemoveSpecialCharacters();
            FilesManager uploaderToClear = UploadedRequestFiles.Where(x => x.Filename != null && x.UploadType == "SSV").FirstOrDefault(x => x.Filename.Contains(fname));

            if (uploaderToClear != null)
            {
                if (File.Exists(uploaderToClear.UploadPath))
                {
                    uploaderToClear.Filestream.Close();
                    uploaderToClear.UploadFile.Stream.Close();

                    File.Delete(uploaderToClear.UploadPath);
                }

                UploadedRequestFiles.Remove(uploaderToClear);

                ResetSSVIndex().Wait();
                EnableDisableActionButton(IsRRUType);
            }
        }

        private void ResetUploadToNull(Syncfusion.Blazor.Inputs.FileInfo file, bool shouldDelete)
        {
            var nameCompare = Path.GetFileNameWithoutExtension(file.Name).RemoveSpecialCharacters();
            var fname = (nameCompare.Length > 3) ? Path.GetFileNameWithoutExtension(file.Name.Split('(')[0].Trim()).RemoveSpecialCharacters() : Path.GetFileNameWithoutExtension(file.Name.Trim()).RemoveSpecialCharacters();

            var a = UploadedRequestFiles.Where(x => x.Filename != null && x.UploadType != "SSV");
            var b = a.FirstOrDefault(x => x.Filename.Contains(fname));


            FilesManager uploaderToClear = UploadedRequestFiles.Where(x => x.Filename != null && x.UploadType != "SSV").FirstOrDefault(x => x.Filename.Contains(fname));

            if (uploaderToClear != null)
            {
                if (File.Exists(uploaderToClear.UploadPath))
                {
                    uploaderToClear.Filestream.Close();
                    uploaderToClear.UploadFile.Stream.Close();

                    if (shouldDelete)
                        File.Delete(uploaderToClear.UploadPath);
                }

                uploaderToClear.Filestream = null;
                uploaderToClear.Filename = null;
                uploaderToClear.UploadFile = null;
                uploaderToClear.UploadPath = null;

                if (uploaderToClear.UploadType == "Bulk")
                {
                    BulkRequestRRUData = null;
                    BulkRequestRRUCount = 0;
                    BulkOptionalSSVCount = 0;

                    for (int i = 0; i < UploadedRequestFiles.Count; i++)
                    {
                        if (UploadedRequestFiles[i].Filename != null)
                        {
                            if (UploadedRequestFiles[i].Filename.Contains("RRU MOD"))
                            {
                                UploadedRequestFiles.Remove(UploadedRequestFiles[i]);
                            }
                        }
                    }
                }

                EnableDisableActionButton(IsRRUType);
            }
        }

        private void CloseDialog()
        {
            ShowOffPeakNotice = false;
            ShowInvalidDialog = false;
            BulkRequestInvalidData = Enumerable.Empty<RequestViewModel>().ToList();

            if (SingleEntrySelected && !SingleEntryValid)
            {
                RequestModel.SpectrumId = null;
            }
        }

        public void OnToastClickHandler(ToastClickEventArgs args)
        {
            args.ClickToClose = true;
        }

        private void OnClearRRU(ClearingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First());
        }

        private void OnRemoveRRU(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First());
        }

        private void OnClear(ClearingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First());

            TotalFileCount--;
            TotalFileSize -= args.FilesData.Sum(x => x.Size) / (1024 * 1024);
        }

        private void OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First());

            TotalFileCount--;
            TotalFileSize -= args.FilesData.Sum(x => x.Size) / (1024 * 1024);
        }

        private void OnRemoveToNull(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUploadToNull(args.FilesData.First(), true);
        }

        private void OnClearToNull(ClearingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUploadToNull(args.FilesData.First(), false);
        }

        private async Task InitializeForm()
        {
            TotalFileCount = 0;
            TotalFileSize = 0;
            SEUploadIconCss = "fas fa-paper-plane ml-2";
            BulkUploadIconCss = "fas fa-paper-plane ml-2";
            Principal = (await AuthenticationStateTask).User;

            BulkWaiverUploadSelected = false;
            ShowInvalidDialog = false;

            await IRequestList.Initialize(Principal);
            FileUploader.Initialize(IRequestList, Logger);
            IRequestList.Spectrums = new();
            ShowOffPeakNotice = IsPeakPeriod();

            RequestModel = new();
            Input = new();
            RequestModel.DateSubmitted = DateTime.Now;
            RequestModel.IntegratedDate = DateTime.Now;

            UploadedRequestFiles = new();
            BulkRequestRRUData = null;
            BulkRequestRRUCount = 0;
            BulkOptionalSSVCount = 0;

            UploadedRequestFiles.AddRange(InitializeUploadFiles());

            EnableDisableActionButton(IsRRUType);
        }

        private void ReInitializeRequest(MouseEventArgs args)
        {
            Input.BUSiteCount = 1;
            TotalFileCount = 0;
            TotalFileSize = 0;
            BulkOptionalSSVCount = 0;
            UploadedRequestFiles = new();
            ShowInvalidDialog = false;
            BulkWaiverUploadSelected = false;
            IRequestList.Spectrums = new();

            if (BulkUploadSelected)
            {
                UploadedRequestFiles.AddRange(InitializeUploadFiles());
                IRequestList.Spectrums = ISpectrum.Get(x => x.IsActive).GetAwaiter().GetResult().OrderBy(x => x.Name).ToList();
            }

            EnableDisableActionButton(IsRRUType);

            StateHasChanged();
        }

        private static List<FilesManager> InitializeUploadFiles()
        {
            List<FilesManager> filesManagers = new();
            List<string> initialUploadTypes = new() { "Bulk", "Waiver" };

            for (int i = 0; i < initialUploadTypes.Count; i++)
            {
                filesManagers.Add(new FilesManager
                {
                    Index = i,
                    UploadType = initialUploadTypes[i],
                });
            }

            return filesManagers;
        }

        private async Task ResetSSVIndex()
        {
            int i = 2;

            try
            {
                UploadedRequestFiles.Where(x => x.UploadType == "SSV").ToList().ForEach((ssv) =>
                {
                    if (File.Exists(ssv.UploadPath))
                    {
                        if (ssv.Filestream.CanSeek)
                        {
                            ssv.Filestream.Close();
                            ssv.UploadFile.Stream.Close();
                        }

                        File.Delete(ssv.UploadPath);
                    }

                    ssv.Index = i;

                    i++;
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
            }

            await Task.CompletedTask;
        }

        private async Task RemoveBulkSSVs()
        {
            try
            {
                await SFBulkASSV_Uploader.ClearAllAsync();

                foreach (var ssv in UploadedRequestFiles.ToList())
                {
                    if (ssv.UploadType == "SSV" && !ssv.Filename.Contains("RRU"))
                    {
                        if (File.Exists(ssv.UploadPath))
                        {
                            if (ssv.Filestream.CanSeek)
                            {
                                ssv.Filestream.Close();
                                ssv.UploadFile.Stream.Close();
                            }

                            File.Delete(ssv.UploadPath);
                        }

                        UploadedRequestFiles.Remove(ssv);
                    }
                }

                ResetBulkRRUUpload();
                await ResetSSVIndex();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
            }

            await Task.CompletedTask;
        }

        public class InputModel
        {
            public int BUSiteCount { get; set; }
            public string BUSSVReport { get; set; }
            public string SSVReport { get; set; }
        }

        protected void HandleInput<T>(T args)
        {
            //int result = ((dynamic)args).Value;

            //int uploadCount = ((dynamic)args).PreviousValue;// UploadedRequestFiles.Count(x => x.UploadType == "SSV");

            //if (result < uploadCount)
            //{
            //    var count = uploadCount - result;
            //    for (var i = 1; i <= count; i++)
            //    {
            //        UploadedRequestFiles.Remove(UploadedRequestFiles.OrderByDescending(x => x.Index).First());
            //        BulkUploadFiles.RemoveAt(BulkUploadFiles.Count - 1);
            //    }
            //}

            //if (result > uploadCount)
            //{
            //    var count = result - uploadCount;

            //    for (var i = 1; i <= count; i++)
            //    {
            //        FilesManager file = new()
            //        {
            //            Index = uploadCount + 1 + i,
            //            UploadType = "SSV",
            //        };

            //        UploadedRequestFiles.Add(file);
            //        BulkUploadFiles.Add(new SfUploader());
            //    }
            //}

            EnableDisableActionButton(IsRRUType);
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    Logger.LogInformation("Loading create request page", new { });

                    DateTime dt = DateTime.Now;
                    MinDateTime = new DateTime(dt.Year, dt.Month, 1);
                    MaxDateTime = new DateTime(dt.Year, dt.Month, dt.Day);

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:AddRequest"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        await InitializeForm();

                        PageText = "Edit";
                        BtnText = "Update Request";
                        RequestModel = await IRequest.GetById(x => x.Id == Id);

                        return;
                    }

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

        private async Task ProcessSingleRequest(AcceptanceRequestObject requestObject)
        {
            DisableSEButton = true;
            SEUploadIconCss = "fas fa-spin fa-spinner ml-2";
            FilesManager ssvReport = UploadedRequestFiles.OrderByDescending(x => x.Index).FirstOrDefault(x => x.UploadType == "SSV");

            requestObject.IsWaiver = RequestModel.SSVReportIsWaiver;

            SingleRequest singleRequest = new(RequestModel, ssvReport, IRequest);
            var (Saved, Message) = await singleRequest.Save(requestObject, toClose: true);

            if (!Saved)
            {
                if (Message.Length > 0) await ThrowError(Message);
                SEUploadIconCss = "fas fa-paper-plane ml-2";
                return;
            }

            await singleRequest.SetCreateState();

            await ThrowSuccess(Message);
            await InitializeForm();
        }

        private async Task ProcessBulkRequest(AcceptanceRequestObject requestObject)
        {
            if (BulkUploadData.error.Length > 0)
            {
                await ThrowError(BulkUploadData.error);
                return;
            }

            if (await ValidateSSVUploads(BulkUploadData.requests))
            {
                DisableBUButton = true;
                BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";

                requestObject.IsWaiver = BulkWaiverUploadSelected;

                BulkRequest bulkRequest = new(BulkUploadData.requests, UploadedRequestFiles, IRequest);
                var (SaveStatus, Messages, ValidRequests) = await bulkRequest.Save(requestObject);

                if (ValidRequests.Count > 0)
                    await bulkRequest.SetCreateState();

                await ProcessNotifications(Messages);

                await SFBulkAcceptance_Uploader.ClearAllAsync();

                await InitializeForm();
            }
            else
            {
                if (requestObject.IsWaiver)
                    await ClearBulkWaiverUpload();
                else
                    await RemoveBulkSSVs();

                BulkUploadIconCss = "fas fa-paper-plane ml-2";
                return;
            }
        }

        protected async Task HandleValidSubmit(EditContext context)
        {
            //bool isValid = context.Validate();
            AcceptanceRequestObject requestObject = new()
            {
                ProjectTypes = IRequestList.ProjectTypes,
                Spectrums = IRequestList.Spectrums,
                TechTypes = IRequestList.TechTypes,
                User = IRequestList.User,
                BulkUploadPath = BulkUploadPath
            };

            try
            {
                if (SingleEntrySelected)
                {
                    await ProcessSingleRequest(requestObject);
                }

                if (BulkUploadSelected)
                {
                    await ProcessBulkRequest(requestObject);
                }

                AppState.TriggerRequestRecount();

                IsRRUType = false;
            }
            catch (Exception ex)
            {
                SEUploadIconCss = "fas fa-paper-plane ml-2";
                BulkUploadIconCss = "fas fa-paper-plane ml-2";

                await ClearBulkUploads();

                string msg = (ex.InnerException?.Message ?? ex.Message).Contains("unique")
                    ? "Duplicate entry found"
                    : $"An error has occurred. {ex.InnerException?.Message ?? ex.Message}";

                await ThrowError(msg);

                Logger.LogError("Error creating request. ", new { }, ex);
            }
            EnableDisableActionButton(IsRRUType);
        }

        private async Task ClearBulkUploads()
        {
            if (BulkUploadSelected)
            {
                await SFBulkAcceptance_Uploader.ClearAllAsync();
                if (BulkWaiverUploadSelected)
                    await ClearBulkWaiverUpload();
                else
                    await RemoveBulkSSVs();

                StateHasChanged();

                await InitializeForm();
            }
        }

        private async Task ClearBulkWaiverUpload()
        {
            await SF_WaiverUploader.ClearAllAsync();
            ResetBulkRRUUpload();
        }

        private void ResetBulkRRUUpload()
        {
            if (BulkRequestRRUCount > 0)
            {
                BulkUploadRRUSSVFiles.ForEach(async file =>
                {
                    await file.ClearAllAsync();
                });
            }
        }

        private async Task ProcessNotifications(List<string> errors)
        {
            foreach (var error in errors)
            {
                ToastContent = error;
                if (error.StartsWith("Request Submitted successfully")) SuccessBtnOnClick();
                else await ThrowError(error);
            }
        }

        private async Task<bool> ValidateSSVUploads(List<RequestViewModel> requests)
        {
            var isValidUploads = true;

            if (Input.BUSiteCount != BulkUploadData.requests.Count)
            {
                await SFBulkAcceptance_Uploader.ClearAll();

                await RemoveBulkSSVs();
                await InitializeForm();

                await ThrowError($"{BulkUploadError} - No of Site Count does not match number of records uploaded.");

                return false;
            }

            foreach (RequestViewModel request in requests)
            {
                var spectrumName = IRequestList.Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId).Name;
                var techTypeName = IRequestList.TechTypes.FirstOrDefault(x => x.Id == request.TechTypeId).Name;

                var checkName = (spectrumName.Contains("RRU")) ? $"{request.SiteId.ToUpper()}" : $"{request.SiteId.ToUpper()}_{spectrumName.ToUpper().RemoveSpecialCharacters()}";

                if (!BulkWaiverUploadSelected)
                {
                    var uploadFile = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "SSV" && x.Filename.Contains(checkName));

                    if (uploadFile?.Filename == null && Helpers.ShouldRequireSSV(IRequestList.ProjectTypes, spectrumName, techTypeName, request.ProjectTypeId))
                    {
                        ToastContent = $"No SSV document uploaded to match for {checkName}. Please try again.";
                        await Task.Delay(200);

                        ErrorBtnOnClick();

                        isValidUploads = false;
                    }
                }
            }

            return isValidUploads;
        }

        private async Task<bool> OnFileUploadChange(UploadChangeEventArgs args, string type, int uploadIndex)
        {
            try
            {
                UploadFiles UploadFile = args.Files.First();
                IRequestList.Spectrums = ISpectrum.Get(x => x.IsActive).GetAwaiter().GetResult().OrderBy(x => x.Name).ToList();
                FileUploader.Initialize(IRequestList, Logger);

                string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\{type}\\");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                string ext = Path.GetExtension(UploadFile.FileInfo.Name);
                UploadFileName = "SA_" + Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters() + "_" + DateTime.Now.AddHours(1).ToString().Replace("/", "_").Replace(":", "").Replace(" ", "") + ext;
                UploadPath = Path.Combine(pathBuilt, UploadFileName);

                FilePath = Path.GetFileName(UploadPath);
                RequestModel.SSVReport = FilePath;

                if (UploadedRequestFiles.Exists(x => x.Index == uploadIndex))
                {
                    FilesManager file = UploadedRequestFiles.FirstOrDefault(x => x.Index == uploadIndex);

                    if (file != null)
                    {
                        file.Filestream = new(UploadPath, FileMode.Create, FileAccess.Write);
                        file.Index = uploadIndex;
                        file.Filename = UploadFileName;
                        file.UploadFile = UploadFile;
                        file.UploadPath = UploadPath;
                        file.UploadType = type;
                        file.UploadIsSSV = type == "SSV";
                        file.UploadIsWaiver = type == "Waiver";
                    }

                    if (type == "Bulk")
                    {
                        bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, false);
                        IRequestList.Spectrums = await ISpectrum.Get(x => x.IsActive);

                        (string uploadResp, string filePath, string uploadError) = await FileUploader.StartUpload(allowedExtension, file, true);

                        if (uploadResp.Length == 0 && uploadError.Length == 0)
                        {
                            BulkUploadPath = filePath;
                            BulkUploadData = await FileUploader.ProcessUpload(file.UploadPath);

                            if (BulkUploadData.requests.Count == 0)
                            {
                                await SFBulkAcceptance_Uploader.ClearAllAsync();

                                EnableDisableActionButton(IsRRUType);
                                await ThrowError($"No Valid Request found! {BulkUploadData.error} - {BulkUploadError}");

                                return false;
                            }

                            var (Valid, Invalid) = await IRequest.GetValidRequests(BulkUploadData.requests);
                            BulkUploadData = ("", Valid.ToList());
                            BulkRequestInvalidData = Invalid.ToList();
                            Input.BUSiteCount = BulkUploadData.requests.Count;

                            if (Invalid.Any())
                            {
                                //EnableDisableActionButton(IsRRUType);

                                //return false;
                                ShowInvalidDialog = true;
                                StateHasChanged();
                            }

                            var RRUSpectrums = IRequestList.Spectrums.Where(y => y.Name.Contains("RRU")).Select(x => x.Id).ToList();

                            BulkRequestRRUData = BulkUploadData.requests.Where(x => RRUSpectrums.Contains(x.SpectrumId) && !BulkRequestInvalidData.Contains(x)).ToList();
                            BulkRequestRRUCount = BulkRequestRRUData.Count;
                            BulkOptionalSSVCount = Valid.Count(x => !Helpers.ShouldRequireSSV(IRequestList.ProjectTypes,
                                IRequestList.Spectrums.FirstOrDefault(y => y.Id == x.SpectrumId).Name,
                                IRequestList.TechTypes.FirstOrDefault(y => y.Id == x.TechTypeId).Name, x.ProjectTypeId));

                            foreach (var item in BulkRequestRRUData)
                            {
                                BulkUploadRRUSSVFiles.Add(new SfUploader());
                            }
                        }
                        else
                        {
                            await SFBulkAcceptance_Uploader.ClearAllAsync();
                            BulkUploadIconCss = "fas fa-paper-plane ml-2";
                            ToastContent = $"{uploadResp} {uploadError}";
                            await Task.Delay(200);

                            ErrorBtnOnClick();
                        }

                    }
                }

                EnableDisableActionButton(IsRRUType);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);

                return false;
            }
        }

        private async Task<bool> OnFileSSVUploadChange(UploadChangeEventArgs args, string type)
        {
            List<UploadFiles> UploadFiles = args.Files;

            if (!SingleEntrySelected)
            {
                var s = await SFBulkASSV_Uploader.GetFilesDataAsync();
                int a = (s == null) ? 0 : s.Count;

                var b = args.Files.Sum(x => x.FileInfo.Size) / (1024 * 1024);

                if (b > MaxUploadFileSize || a > MaxFileCount)
                {
                    await SFBulkASSV_Uploader.CancelAsync();

                    return false;
                }

                TotalFileCount = args.Files.Count;
                TotalFileSize = args.Files.Sum(x => x.FileInfo.Size) / (1024 * 1024);
                StateHasChanged();
            }

            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\{type}\\");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            foreach (var UploadFile in UploadFiles)
            {
                string ext = Path.GetExtension(UploadFile.FileInfo.Name);
                var filenameWOSpecialXters = Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters();
                UploadFileName = "SA_" + filenameWOSpecialXters + "_" + DateTime.Now.AddHours(1).ToString() + ext;
                UploadPath = Path.Combine(pathBuilt, UploadFileName.RemoveSpecialCharacters());

                var ssvExists = UploadedRequestFiles.Where(x => x.Filename != null).Any(c => c.Filename.Contains(Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters()));

                if (!ssvExists)
                {
                    UploadedRequestFiles.Add(new FilesManager
                    {
                        Filestream = new(UploadPath, FileMode.Create, FileAccess.Write),
                        Index = UploadedRequestFiles.Count,
                        Filename = Path.GetFileName(UploadPath),
                        UploadFile = UploadFile,
                        UploadPath = UploadPath,
                        UploadType = type,
                        UploadIsSSV = type == "SSV",
                        UploadIsWaiver = type == "Waiver",
                    });
                }
            }

            EnableDisableActionButton(IsRRUType);

            return await Task.Run(() => true);
        }

        private async Task<bool> OnFileSSVRRUUploadChange(UploadChangeEventArgs args, string type)
        {
            UploadFiles UploadFile = args.Files.First();

            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), $"Documents\\{type}\\");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            string ext = Path.GetExtension(UploadFile.FileInfo.Name);
            var filenameWOSpecialXters = Path.GetFileNameWithoutExtension(UploadFile.FileInfo.Name).RemoveSpecialCharacters();
            UploadFileName = "SA_" + filenameWOSpecialXters + "_" + DateTime.Now.AddHours(1).ToString() + ext;
            UploadPath = Path.Combine(pathBuilt, UploadFileName.RemoveSpecialCharacters());

            UploadedRequestFiles.Add(new FilesManager
            {
                Filestream = new(UploadPath, FileMode.Create, FileAccess.Write),
                Index = UploadedRequestFiles.Count,
                Filename = Path.GetFileName(UploadPath),
                UploadFile = UploadFile,
                UploadPath = UploadPath,
                UploadType = type,
                UploadIsSSV = type == "SSV",
                UploadIsWaiver = type == "Waiver",
            });

            EnableDisableActionButton(IsRRUType);

            return await Task.Run(() => true);
        }

        private void TriggerActionButton(ChangeEventArgs args)
        {
            EnableDisableActionButton(IsRRUType);
        }

        private void IsSEValid(bool SEValid)
        {
            if (RequestModel.SiteId != null && RequestModel.SpectrumId != null)
            {
                SingleEntryValid = SEValid;
                ShowInvalidDialog = !SEValid;

                if (SEValid == false)
                {
                    BulkRequestInvalidData.Add(RequestModel);
                }
            }
        }

        private void EnableDisableActionButton(bool IsSERRUType)
        {
            IsRRUType = IsSERRUType;
            var ssvUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "SSV");
            var spectrumName = (!string.IsNullOrWhiteSpace(RequestModel?.SpectrumId))
                ? IRequestList.Spectrums.FirstOrDefault(x => x.Id == RequestModel?.SpectrumId)?.Name : "~";
            var TechTypeName = (!string.IsNullOrWhiteSpace(RequestModel?.TechTypeId))
                ? IRequestList.TechTypes.FirstOrDefault(x => x.Id == RequestModel?.TechTypeId)?.Name : "~";

            DisableSEButton = true;

            if (SingleEntrySelected)
            {
                if (SingleEntryValid && (ssvUpload?.UploadFile != null || IsRRUType))
                {
                    DisableSEButton = false;
                }

                if (!Helpers.ShouldRequireSSV(IRequestList.ProjectTypes, spectrumName, TechTypeName, RequestModel.ProjectTypeId))
                {
                    DisableSEButton = false;
                }
            }
            if (BulkUploadSelected)
            {
                var disable = true;
                var bulkUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Bulk");
                var waiverUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Waiver");
                var ssvUploadCount = UploadedRequestFiles.Count(x => x.UploadType == "SSV" && !x.Filename.Contains("RRU"));
                var items = (BulkWaiverUploadSelected) ? UploadedRequestFiles.Where(c => c.UploadType == "SSV" || c.UploadType == "Waiver")
                    : UploadedRequestFiles.Where(c => c.UploadType == "SSV");

                foreach (var item in items)
                {
                    if (BulkWaiverUploadSelected && waiverUpload.UploadFile != null)
                    {
                        disable = false;
                        break;
                    }
                    //else if (!BulkWaiverUploadSelected && (Input.BUSiteCount - BulkRequestRRUCount) != ssvUploadCount)
                    //{
                    //    disable = true;
                    //    break;
                    //}
                    else if (!BulkWaiverUploadSelected && Input.BUSiteCount <= (ssvUploadCount + BulkOptionalSSVCount)
                        && ssvUploadCount <= Input.BUSiteCount)
                    {
                        disable = false;
                        break;
                    }
                    else
                    {
                        if (item?.UploadFile != null && Input.BUSiteCount <= (ssvUploadCount + BulkOptionalSSVCount))
                        {
                            disable = false;
                        }
                        else
                        {
                            disable = true;
                            break;
                        }
                    }
                }

                if (BulkUploadData.requests != null)
                {
                    if (BulkUploadData.requests.Count == 0)
                    {
                        disable = true;
                    }

                    if (BulkUploadData.requests.Count != 0 && BulkUploadData.requests.Count == BulkOptionalSSVCount)
                    {
                        disable = false;
                    }
                }

                if (BulkWaiverUploadSelected && waiverUpload.UploadFile == null)
                {
                    disable = true;
                }

                if (bulkUpload?.UploadFile == null)
                {
                    disable = true;
                }

                DisableBUButton = disable;
            }
        }

        public void Dispose()
        {
            //AppState.OnChange -= CalStateChanged;
            Task.Run(async () =>
            {
                if (SF_Uploader != null)
                {
                    await SF_Uploader.ClearAllAsync();
                }
                if (SFBulkAcceptance_Uploader != null)
                {
                    await SFBulkAcceptance_Uploader.ClearAllAsync();
                }
                if (SFBulkASSV_Uploader != null)
                {
                    await SFBulkASSV_Uploader.ClearAllAsync();
                }
                //await InitializeForm();
            });

            GC.SuppressFinalize(this);
        }
    }
}

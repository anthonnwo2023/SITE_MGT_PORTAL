using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Helpers.Excel;
using Project.V1.DLL.Interface;
using Project.V1.Lib.Services;
using Project.V1.Models;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Notifications;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.DLL.Helpers;
using System.Linq.Expressions;
using System.Dynamic;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor.SplitButtons;

namespace Project.V1.Web.Pages.Acceptance
{
    public partial class NewRequest : IDisposable
    {
        [Parameter] public string Id { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected IRRUType IRRUType { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IBaseBand IBaseBand { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] AppState AppState { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<RRUTypeModel> RRUTypes { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<BaseBandModel> Basebands { get; set; }
        public RequestViewModel RequestModel { get; set; }
        public LTEInputModel LTEInput { get; set; } = new();
        public InputModel Input { get; set; } = new();
        public string PageText { get; set; } = "Create";
        public string BtnText { get; set; } = "Create Request";
        public string BulkUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";

        private string BulkUploadColumnError { get; set; }
        private string BulkUploadError { get; set; }
        private string SaveError { get; set; }
        private List<SfUploader> BulkUploadFiles = new();
        public List<FilesManager> UploadedRequestFiles { get; set; } = new();
        public List<FileStream> Filestreams { get; set; }
        public List<UploadFiles> UploadFiles { get; set; }
        public string UploadPath { get; set; }
        public string FilePath { get; set; }
        public string UploadFileName { get; set; }
        public bool DisableSEButton { get; set; } = true;
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
        protected SfToast ToastObj { get; set; }
        protected bool SingleEntrySelected { get; set; }
        protected bool BulkUploadSelected { get; set; }

        public List<BoolDropDown> BoolDrops { get; set; } = new()
        {
            new BoolDropDown { Name = "Yes" },
            new BoolDropDown { Name = "No" }
        };

        public class BoolDropDown
        {
            public string Name { get; set; }
        }

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

        private void ResetUpload(string filename)
        {
            var nameCompare = Path.GetFileNameWithoutExtension(filename);
            var fname = (nameCompare.Length > 3) ? Path.GetFileNameWithoutExtension(filename.Split('(')[0]) : Path.GetFileNameWithoutExtension(filename);
            FilesManager uploaderToClear = UploadedRequestFiles.Where(x => x.Filename != null).FirstOrDefault(x => x.Filename.Contains(fname));

            if (uploaderToClear != null)
            {
                if (File.Exists(uploaderToClear.UploadPath))
                {
                    uploaderToClear.Filestream.Close();
                    uploaderToClear.UploadFile.Stream.Close();

                    File.Delete(uploaderToClear.UploadPath);
                }

                uploaderToClear.Filestream = null;
                uploaderToClear.Filename = null;
                uploaderToClear.UploadFile = null;
                uploaderToClear.UploadPath = null;

                //if (SingleEntrySelected)
                //{
                //    DisableSEButton = true;
                //}

                //if (BulkUploadSelected && uploaderToClear.UploadPath == "Bulk")
                //{
                //    DisableBUButton = true;
                //}
                EnableDisableActionButton();
            }
        }

        private void OnClear(ClearingEventArgs args)
        {
            //if (SingleEntrySelected)
            //{
            //    DisableSEButton = true;
            //}
            //if (BulkUploadSelected)
            //{
            //    DisableBUButton = true;
            //}

            //foreach (var file in UploadedRequestFiles)
            //{
            //    if (File.Exists(file.UploadPath))
            //    {
            //        file.Filestream.Close();
            //        file.UploadFile.Stream.Close();

            //        File.Delete(file.UploadPath);
            //    }
            //}

            //UploadedRequestFiles.RemoveAt(UploadedRequestFiles.FindIndex(x => x.Filename.Contains(args.FilesData.First()?.Name)));
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First()?.Name);
        }

        private void OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload(args.FilesData.First()?.Name);
        }

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);

            BulkWaiverUploadSelected = false;

            Regions = await IRegion.Get(x => x.IsActive);
            SummerConfigs = await ISummerConfig.Get(x => x.IsActive);
            ProjectTypes = await IProjectType.Get(x => x.IsActive);
            RRUTypes = await IRRUType.Get(x => x.IsActive && x.VendorId == User.VendorId);
            TechTypes = await ITechType.Get(x => x.IsActive);
            AntennaMakes = await IAntennaMake.Get(x => x.IsActive);
            AntennaTypes = await IAntennaType.Get(x => x.IsActive);
            Spectrums = new();
            Basebands = (Principal.IsInRole("Super Admin"))
                ? await IBaseBand.Get(x => x.IsActive)
                : await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId);

            RequestModel = new();
            Input = new();
            RequestModel.DateSubmitted = DateTime.Now;
            RequestModel.IntegratedDate = DateTime.Now;

            UploadedRequestFiles = new();
            BulkUploadFiles = new();

            UploadedRequestFiles.AddRange(InitializeUploadFiles());
            BulkUploadFiles.Add(new SfUploader());
        }

        private void ReInitializeRequest(MouseEventArgs args)
        {
            Input.BUSiteCount = 1;
            UploadedRequestFiles = new();
            BulkUploadFiles = new();
            BulkWaiverUploadSelected = false;

            if (BulkUploadSelected)
            {
                UploadedRequestFiles.AddRange(InitializeUploadFiles());
            }
            if (SingleEntrySelected)
            {
                UploadedRequestFiles.Add(new FilesManager
                {
                    Index = 0,
                    UploadType = "SSV",
                });
            }
            BulkUploadFiles.Add(new SfUploader());

            EnableDisableActionButton();

            StateHasChanged();
        }

        private static List<FilesManager> InitializeUploadFiles()
        {
            List<FilesManager> filesManagers = new();
            List<string> initialUploadTypes = new() { "Bulk", "Waiver", "SSV" };

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

        public async Task OnTechChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, TechTypeModel> args)
        {
            Spectrums = await ISpectrum.Get(x => x.TechTypeId == args.Value && x.IsActive);
        }

        public async Task OnVendorChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, VendorModel> args)
        {
            //RRUTypes = await IRRUType.Get(x => x.VendorId == args.Value && x.IsActive);
        }

        public class LTEInputModel
        {
            public string RRUPower { get; set; }

            public string CSFDStatusGSM { get; set; }

            public string CSFDStatusWCDMA { get; set; }
        }

        public class FilesManager
        {
            public int Index { get; set; }
            public bool UploadIsSSV { get; set; } = true;
            public bool UploadIsWaiver { get; set; }
            public string UploadType { get; set; }
            public string Filename { get; set; }
            public string UploadPath { get; set; }
            public FileStream Filestream { get; set; }
            public UploadFiles UploadFile { get; set; }
        }

        public class InputModel
        {
            public int BUSiteCount { get; set; }
            public string BUSSVReport { get; set; }
            public string SSVReport { get; set; }
        }

        protected void HandleInput<T>(T args)
        {
            int result = ((dynamic)args).Value;

            int uploadCount = ((dynamic)args).PreviousValue;// UploadedRequestFiles.Count(x => x.UploadType == "SSV");

            if (result < uploadCount)
            {
                var count = uploadCount - result;
                for (var i = 1; i <= count; i++)
                {
                    UploadedRequestFiles.Remove(UploadedRequestFiles.OrderByDescending(x => x.Index).First());
                    BulkUploadFiles.RemoveAt(BulkUploadFiles.Count - 1);
                }
            }
            if (result > uploadCount)
            {
                var count = result - uploadCount;

                for (var i = 1; i <= count; i++)
                {
                    FilesManager file = new()
                    {
                        Index = uploadCount + 1 + i,
                        UploadType = "SSV",
                    };

                    UploadedRequestFiles.Add(file);
                    BulkUploadFiles.Add(new SfUploader());
                }
            }

            EnableDisableActionButton();
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

        protected async Task HandleValidSubmit(EditContext context)
        {
            bool isValid = context.Validate();
            BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";

            var variables = new Dictionary<string, object> { { "User", User.UserName }, { "App", "acceptance" } };

            try
            {
                if (SingleEntrySelected)
                {
                    FilesManager ssvReport = UploadedRequestFiles.OrderBy(x => x.Index).FirstOrDefault(x => x.UploadType == "SSV");

                    if (!ssvReport.Filename.ToLower().Contains(RequestModel.SiteId.ToLower()))
                    {
                        throw new Exception("Site Id does not match the uploaded document", new Exception("Site Id does not match the uploaded document"));
                    }

                    if (await SaveSingleRequest(RequestModel, ssvReport, true))
                    {
                        await IRequest.SetCreateState(RequestModel, variables);
                    }
                    else
                    {
                        throw new Exception(SaveError, new Exception(SaveError));
                    }
                }
                if (BulkUploadSelected)
                {
                    DisableBUButton = true;
                    WaiverUploadSelected = BulkWaiverUploadSelected;
                    Spectrums = await ISpectrum.Get(x => x.IsActive);

                    FilesManager acceptanceUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Bulk");

                    string ext = Path.GetExtension(acceptanceUpload.UploadFile.FileInfo.Name);
                    bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, false);

                    (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension, acceptanceUpload, true);

                    if (uploadResp.Length == 0 && uploadError.Length == 0)
                    {
                        (string error, List<RequestViewModel> requests) = await ProcessUpload(acceptanceUpload.UploadPath);

                        if (error.Length > 0)
                        {
                            throw new Exception(error, new Exception(error));
                        }

                        if (Input.BUSiteCount == requests.Count)
                        {
                            foreach (RequestViewModel request in requests)
                            {
                                if (!BulkWaiverUploadSelected)
                                {
                                    var uploadFile = UploadedRequestFiles.Where(x => x.UploadType == "SSV").OrderBy(x => x.Index).ElementAt(requests.IndexOf(request));

                                    if (!uploadFile.Filename.ToLower().Contains(request.SiteId.ToLower()))
                                    {
                                        throw new Exception("Site Id does not match the uploaded document", new Exception("Site Id does not match the uploaded document"));
                                    }
                                }
                            }

                            var batchNumber = HelperFunctions.GenerateIDUnique("SA-BN");

                            foreach (RequestViewModel request in requests)
                            {
                                FilesManager uploadFile = new();

                                if (BulkWaiverUploadSelected)
                                {
                                    uploadFile = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Waiver");
                                }
                                else
                                {
                                    uploadFile = UploadedRequestFiles.Where(x => x.UploadType == "SSV").OrderBy(x => x.Index).ElementAt(requests.IndexOf(request));
                                }

                                request.BulkuploadPath = Path.GetFileName(filePath);
                                request.BulkBatchNumber = batchNumber;
                                var toClose = true;

                                if (BulkWaiverUploadSelected && ((requests.IndexOf(request) + 1) != requests.Count))
                                {
                                    toClose = false;
                                }

                                if (await SaveSingleRequest(request, uploadFile, toClose))
                                {
                                    await BulkUploadFiles[requests.IndexOf(request)].ClearAll();
                                }
                                else
                                {
                                    throw new Exception(SaveError, new Exception(SaveError));
                                }
                            }

                            await IRequest.SetCreateState(requests, variables);
                        }
                        else
                        {
                            await SFBulkAcceptance_Uploader.ClearAll();


                            if (BulkUploadError == null)
                            {
                                throw new Exception("Site count does not match number of uploaded SSV Report", new Exception("Site count does not match number of uploaded  SSV Report"));
                            }
                            else
                            {
                                BulkUploadIconCss = "fas fa-paper-plane ml-2";
                                ToastContent = BulkUploadError;
                                await Task.Delay(200);

                                ErrorBtnOnClick();
                            }
                        }
                    }
                }

                AppState.TriggerRequestRecount();

                await InitializeForm();
                EnableDisableActionButton();
            }
            catch (Exception ex)
            {
                if (SingleEntrySelected)
                {
                    await SF_Uploader.ClearAll();
                }

                if (BulkUploadSelected)
                {
                    await SFBulkAcceptance_Uploader.ClearAll();
                    if (BulkWaiverUploadSelected)
                        await SF_WaiverUploader.ClearAll();

                    for (int i = 0; i < BulkUploadFiles.Count; i++)
                    {
                        var filename = UploadedRequestFiles.Where(x => x.UploadType == "SSV").OrderBy(x => x.Index).ElementAt(i).Filename;
                        if (filename != null)
                            ResetUpload(filename);
                        await BulkUploadFiles[i].ClearAll();
                    }

                    StateHasChanged();
                }

                //await InitializeForm();
                EnableDisableActionButton();

                BulkUploadIconCss = "fas fa-paper-plane ml-2";

                string msg = ex.InnerException?.Message ?? ex.Message;

                ToastContent = (msg.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";
                await Task.Delay(200);
                ErrorBtnOnClick();

                Logger.LogError("Error creating request. ", new { }, ex);
            }
        }

        private async Task<(string error, List<RequestViewModel>)> ProcessUpload(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    ExcelRequestObj excelRequestObj = new()
                    {
                        Headers = new List<string>
                        {
                            "Technology", "Site Id", "Site Name", "RNC/BSC", "Region", "Spectrum", "Bandwidth (MHz)", "Latitude", "Longitude",
                            "Antenna Make", "Antenna Type", "Antenna Height - (M)", "Antenna Azimuth", "M Tilt", "E Tilt", "Baseband", "RRU TYPE", "Power - (w)",
                            "Project Type", "Project Year", "Summer Config", "Software", "RRU Power - (w)", "CSFB Status GSM", "CSFB Status WCDMA",
                            "Integrated Date", "Date Submitted", "RET Configured", "Carrier Aggregation"
                        }
                    };

                    (DataTable dt, ExcelTransactionError error) = ExcelProcessor.ToDataTable(excelRequestObj, path, User.Fullname);

                    if (error.ErrorType.Length == 0)
                    {
                        List<RequestViewModel> requests = await GetRequestsFromDataTable(dt, error);
                        return (error.ErrorType, requests);
                    }

                    Logger.LogError($"{error.ErrorDesc} : {error.ErrorType}", new { Created = error.CreatedBy, error.DateCreated });

                    return (error.ErrorType, new List<RequestViewModel>());
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message, new { }, ex);

                    ToastContent = ex.Message;

                    await Task.Delay(1000);
                    ErrorBtnOnClick();

                    return (ex.Message, new List<RequestViewModel>());
                }

            }

            return ("File does not exist.", new List<RequestViewModel>());
        }

        private async Task<List<RequestViewModel>> GetRequestsFromDataTable(DataTable dt, ExcelTransactionError error)
        {
            List<RequestViewModel> requests = new();

            switch (error.ErrorType.Length)
            {
                case 0:
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            RequestViewModel request = ExcelProcessor.CreateItemFromRowMapper<RequestViewModel>(row);

                            if (ValidateRow(request) == false)
                            {
                                BulkUploadError = $"Invalid Excel Template uploaded. Foreign Key Mismatch on row:  {dt.Rows.IndexOf(row) + 1}, col: {BulkUploadColumnError}";
                                break;
                            }

                            request.AntennaMakeId = AntennaMakes.FirstOrDefault(x => x.Name.ToUpper() == request.AntennaMakeId.ToUpper())?.Id;
                            request.AntennaTypeId = AntennaTypes.FirstOrDefault(x => x.Name.ToUpper() == request.AntennaTypeId.ToUpper())?.Id;
                            request.BasebandId = Basebands.FirstOrDefault(x => x.Name.ToUpper() == request.BasebandId.ToUpper())?.Id;
                            request.ProjectTypeId = ProjectTypes.FirstOrDefault(x => x.Name.ToUpper() == request.ProjectTypeId.ToUpper())?.Id;
                            request.RegionId = Regions.FirstOrDefault(x => x.Name.ToUpper() == request.RegionId.ToUpper())?.Id;
                            request.SummerConfigId = SummerConfigs.FirstOrDefault(x => x.Name.ToUpper() == request.SummerConfigId.ToUpper())?.Id;
                            request.TechTypeId = TechTypes.FirstOrDefault(x => x.Name.ToUpper() == request.TechTypeId.ToUpper())?.Id;
                            request.SpectrumId = Spectrums.FirstOrDefault(x => x.Name.ToUpper() == request.SpectrumId.ToUpper())?.Id;
                            request.RRUTypeId = RRUTypes.FirstOrDefault(x => x.Name.ToUpper() == request.RRUTypeId.ToUpper())?.Id;

                            requests.Add(request);
                        }

                        return await Task.FromResult(requests);
                    }

                default:
                    return requests;
            }
        }

        private bool ValidateRow(RequestViewModel request)
        {
            return IsFKValid(x => x.Name.ToUpper() == request.AntennaMakeId.ToUpper(), AntennaMakes)
                && IsFKValid(x => x.Name.ToUpper() == request.AntennaTypeId.ToUpper(), AntennaTypes)
                && IsFKValid(x => x.Name.ToUpper() == request.BasebandId.ToUpper(), Basebands)
                && IsFKValid(x => x.Name.ToUpper() == request.ProjectTypeId.ToUpper(), ProjectTypes)
                && IsFKValid(x => x.Name.ToUpper() == request.RegionId.ToUpper(), Regions)
                && IsFKValid(x => x.Name.ToUpper() == request.SummerConfigId.ToUpper(), SummerConfigs)
                && IsFKValid(x => x.Name.ToUpper() == request.SpectrumId.ToUpper(), Spectrums)
                && IsFKValid(x => x.Name.ToUpper() == request.TechTypeId.ToUpper(), TechTypes)
                && IsFKValid(x => x.Name.ToUpper() == request.RRUTypeId.ToUpper(), RRUTypes);
        }

        private bool IsFKValid<T>(Func<T, bool> whereExpression, List<T> collection) where T : class
        {
            var isFound = collection.Any(whereExpression);

            if (!isFound)
            {
                BulkUploadColumnError = typeof(T).Name.Replace("Model", "");
            }

            return isFound;
        }

        private async Task<bool> SaveRequest(RequestViewModel request)
        {
            try
            {
                request.Id = Guid.NewGuid().ToString();
                request.DateCreated = DateTime.Now;
                request.DateSubmitted = DateTime.Now;
                request.Status = "Pending";
                request.RequestType = "SA";
                request.SSVReportIsWaiver = WaiverUploadSelected;

                request.EngineerAssigned = HelperFunctions.ModelApprover("EA", request.Id);

                request.Requester = new RequesterData
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = User?.Fullname,
                    Department = User?.Department,
                    Email = User?.Email,
                    Phone = User?.PhoneNumber,
                    Title = User?.JobTitle,
                    Username = User?.UserName,
                    VendorId = User?.VendorId
                };

                if (request.TechTypeId == TechTypes.FirstOrDefault(x => x.Name == "4G")?.Id)
                {
                    request.RRUPower = LTEInput.RRUPower;
                    request.CSFBStatusGSM = LTEInput.CSFDStatusGSM;
                    request.CSFBStatusWCDMA = LTEInput.CSFDStatusWCDMA;
                }

                if (await IRequest.CreateRequest(request))
                {
                    ToastContent = "Request Submitted successfully.";
                    return true;
                }

                ToastContent = $"An error has occurred. Request could not be created.";
                return false;
            }
            catch (Exception ex)
            {
                ToastContent = (ex.InnerException.Message.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";
                return false;
            }
        }

        private async Task<bool> SaveSingleRequest(RequestViewModel request, FilesManager file, bool toClose)
        {
            try
            {
                if (file?.UploadFile == null)
                {
                    SaveError = "Missing SSV Report";
                    return false;
                }

                string ext = Path.GetExtension(file.UploadFile.FileInfo.Name);
                bool isWaiver = (SingleEntrySelected) ? WaiverUploadSelected : BulkWaiverUploadSelected;
                bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, isWaiver);

                (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension, file, toClose);

                request.SSVReport = Path.GetFileName(filePath);

                if (uploadResp.Length != 0 || uploadError.Length != 0)
                {
                    File.Delete(UploadPath);
                    await SF_Uploader.ClearAll();

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";

                    ToastContent = $"An error has occurred. SSV Report/Waiver could not be uploaded.";
                    await Task.Delay(1000);

                    ErrorBtnOnClick();

                    return false;
                }

                if (await SaveRequest(request))
                {
                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    await Task.Delay(1000);

                    SuccessBtnOnClick();

                    return true;
                }

                BulkUploadIconCss = "fas fa-paper-plane ml-2";
                await Task.Delay(1000);

                ErrorBtnOnClick();

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<(string, string, string)> StartUpload(bool allowedExtension, FilesManager file, bool toClose)
        {
            switch (allowedExtension)
            {
                case true:
                    {
                        (string error, string path) = await UploadFile(file, toClose);

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

        private async Task<(string, string)> UploadFile(FilesManager bufile, bool toClose)
        {
            try
            {
                await Task.Run(() => bufile.UploadFile.Stream.WriteTo(bufile.Filestream));

                if (toClose)
                {
                    bufile.Filestream.Close();
                    bufile.UploadFile.Stream.Close();
                }

                return ("", bufile.UploadPath);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
                return (ex.Message, "");
            }
        }

        private async Task<bool> OnFileUploadChange(UploadChangeEventArgs args, string type, int uploadIndex)
        {
            UploadFiles UploadFile = args.Files.First();

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
            }
            else
            {
                UploadedRequestFiles.Add(new FilesManager
                {
                    Filestream = new(UploadPath, FileMode.Create, FileAccess.Write),
                    Index = uploadIndex,
                    Filename = UploadFileName,
                    UploadFile = UploadFile,
                    UploadPath = UploadPath,
                    UploadType = type,
                    UploadIsSSV = type == "SSV",
                    UploadIsWaiver = type == "Waiver",
                });
            }

            EnableDisableActionButton();

            return await Task.Run(() => true);
        }

        private void TriggerActionButton(ChangeEventArgs args)
        {
            EnableDisableActionButton();

            ClearUploadFile();
        }

        private void ClearUploadFile()
        {
            FilesManager file = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Waiver");

            if (file != null)
            {
                file.Filestream = null;
                file.Filename = null;
                file.UploadFile = null;
                file.UploadPath = null;
            }
        }

        private void EnableDisableActionButton()
        {
            var ssvUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "SSV");
            DisableSEButton = true;

            if (SingleEntrySelected && ssvUpload?.UploadFile != null)
            {
                DisableSEButton = false;
            }
            if (BulkUploadSelected)
            {
                var disable = true;
                var bulkUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Bulk");
                var waiverUpload = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Waiver");

                foreach (var item in UploadedRequestFiles.Where(c => c.UploadType == "SSV"))
                {
                    if (BulkWaiverUploadSelected && waiverUpload.UploadFile != null)
                    {
                        disable = false;
                        break;
                    }
                    else
                    {
                        if (item?.UploadFile != null)
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

                if (BulkWaiverUploadSelected && waiverUpload.UploadFile == null)
                {
                    disable = true;
                }

                if (bulkUpload.UploadFile == null)
                {
                    disable = true;
                }

                DisableBUButton = disable;
            }
        }

        public void Dispose()
        {
            //AppState.OnChange -= CalStateChanged;
            GC.SuppressFinalize(this);
        }
    }
}

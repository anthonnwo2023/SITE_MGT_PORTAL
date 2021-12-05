using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Extensions;
using Project.V1.Lib.Helpers;
using Project.V1.Lib.Helpers.Excel;
using Project.V1.DLL.Interface;
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
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor.SplitButtons;
using Project.V1.Lib.Services;
using Org.BouncyCastle.Asn1.Ocsp;

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
        public List<RequestViewModel> BulkRequestRRUData { get; set; }
        public List<RequestViewModel> BulkRequestInvalidData { get; set; } = new();
        public int BulkRequestRRUCount { get; set; } = 0;
        public (string error, List<RequestViewModel> requests) BulkUploadData { get; set; }
        public string BulkUploadPath { get; set; }
        public double TotalFileCount { get; set; } = 0;
        public int MaxFileCount { get; set; } = 50;
        public double TotalFileSize { get; set; } = 0;
        public double MaxUploadFileSize { get; set; } = 350;

        public List<BoolDropDown> BoolDrops { get; set; } = new()
        {
            new BoolDropDown { Name = "Yes" },
            new BoolDropDown { Name = "No" }
        };

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
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);

            BulkWaiverUploadSelected = false;
            ShowInvalidDialog = false;

            Regions = (await IRegion.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            SummerConfigs = (await ISummerConfig.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            ProjectTypes = (await IProjectType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            RRUTypes = (User.Vendor.Name == "MTN Nigeria") ? (await IRRUType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList() : (await IRRUType.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();
            TechTypes = (await ITechType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaMakes = (await IAntennaMake.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaTypes = (await IAntennaType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            Spectrums = new();
            Basebands = (Principal.IsInRole("Super Admin"))
                ? (await IBaseBand.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList()
                : (await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();

            RequestModel = new();
            Input = new();
            RequestModel.DateSubmitted = DateTime.Now;
            RequestModel.IntegratedDate = DateTime.Now;

            UploadedRequestFiles = new();
            BulkRequestRRUData = null;
            BulkRequestRRUCount = 0;

            UploadedRequestFiles.AddRange(InitializeUploadFiles());

            EnableDisableActionButton(IsRRUType);
        }

        private void ReInitializeRequest(MouseEventArgs args)
        {
            Input.BUSiteCount = 1;
            TotalFileCount = 0;
            TotalFileSize = 0;
            UploadedRequestFiles = new();
            ShowInvalidDialog = false;
            BulkWaiverUploadSelected = false;

            if (BulkUploadSelected)
            {
                UploadedRequestFiles.AddRange(InitializeUploadFiles());
                Spectrums = ISpectrum.Get(x => x.IsActive).GetAwaiter().GetResult().OrderBy(x => x.Name).ToList();
            }

            if (SingleEntrySelected)
            {
                Spectrums = new();
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

        private void SEProjectYear(ChangeEventArgs<double?> args)
        {
            RequestModel.ProjectYear = (double)args.Value;
        }

        private async Task RemoveSSVLeavingRRUs()
        {
            try
            {
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

                await ResetSSVIndex();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
            }

            await Task.CompletedTask;
        }

        public async Task OnTechChange(List<SpectrumViewModel> spectrums)
        {
            Spectrums = spectrums;

            await Task.CompletedTask;
        }

        public async Task OnSpectrumChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, SpectrumViewModel> args)
        {
            var specturm = Spectrums.FirstOrDefault(x => x.Id == args.Value)?.Name;

            if (specturm == null)
            {
                IsRRUType = false;
            }
            else
            {
                await CheckIfSEValid();

                IsRRUType = false;

                if (specturm.Contains("RRU"))
                {
                    IsRRUType = true;
                }
            }

            EnableDisableActionButton(IsRRUType);

            await Task.CompletedTask;
        }

        //public async Task OnVendorChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, VendorModel> args)
        //{
        //    //RRUTypes = await IRRUType.Get(x => x.VendorId == args.Value && x.IsActive);
        //}

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

        protected async Task HandleValidSubmit(EditContext context)
        {
            bool isValid = context.Validate();

            var variables = new Dictionary<string, object> { { "User", User.UserName }, { "App", "acceptance" } };

            try
            {
                if (SingleEntrySelected)
                {
                    DisableSEButton = true;
                    SEUploadIconCss = "fas fa-spin fa-spinner ml-2";
                    FilesManager ssvReport = UploadedRequestFiles.OrderBy(x => x.Index).FirstOrDefault(x => x.UploadType == "SSV");

                    var spectrumName = Spectrums.FirstOrDefault(x => x.Id == RequestModel.SpectrumId).Name;
                    var TechTypeName = TechTypes.FirstOrDefault(x => x.Id == RequestModel.TechTypeId).Name;
                    var checkName = (spectrumName.Contains("RRU")) ? $"{RequestModel.SiteId.ToUpper()}_{TechTypeName}_RRU_MOD" : $"{RequestModel.SiteId.ToUpper()}_{spectrumName.ToUpper()}";


                    if (ssvReport?.Filename != null && !ssvReport.Filename.ToUpper().Contains(checkName.ToUpper()))
                    {
                        BulkUploadIconCss = "fas fa-paper-plane ml-2";
                        ToastContent = "Site Id does not match the uploaded SSV document";
                        await Task.Delay(200);
                        ErrorBtnOnClick();

                        return;
                    }

                    if (ssvReport?.Filename == null && !spectrumName.Contains("RRU"))
                    {
                        BulkUploadIconCss = "fas fa-paper-plane ml-2";
                        ToastContent = "Please upload SSV document";
                        await Task.Delay(200);

                        ErrorBtnOnClick();

                        return;
                    }

                    if (await SaveSingleRequest(RequestModel, ssvReport, true))
                    {
                        await IRequest.SetCreateState(RequestModel, variables);
                        SEUploadIconCss = "fas fa-paper-plane ml-2";

                        await InitializeForm();
                    }
                    else
                    {
                        BulkUploadIconCss = "fas fa-paper-plane ml-2";
                        ToastContent = SaveError;
                        await Task.Delay(200);

                        ErrorBtnOnClick();

                        await InitializeForm();
                        return;
                    }
                }
                if (BulkUploadSelected)
                {
                    DisableBUButton = true;
                    BulkUploadIconCss = "fas fa-spin fa-spinner ml-2";
                    WaiverUploadSelected = BulkWaiverUploadSelected;

                    (string error, List<RequestViewModel> requests) = BulkUploadData;
                    var errorRequests = new List<RequestViewModel>();

                    if (error.Length > 0)
                    {
                        BulkUploadIconCss = "fas fa-paper-plane ml-2";
                        ToastContent = error;
                        await Task.Delay(200);
                        ErrorBtnOnClick();

                        return;
                    }

                    if (Input.BUSiteCount == requests.Count)
                    {
                        var isValidUploads = await ValidateSSVUploads(requests);

                        if (isValidUploads)
                        {
                            var batchNumber = HelperFunctions.GenerateIDUnique("SA-BN");

                            foreach (RequestViewModel request in requests)
                            {
                                FilesManager uploadFile = new();
                                var spectrumName = Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId).Name;
                                var TechTypeName = TechTypes.FirstOrDefault(x => x.Id == request.TechTypeId).Name;
                                var checkName = (spectrumName.Contains("RRU")) ? $"{request.SiteId.ToUpper()}_{TechTypeName}_RRU_MOD" : $"{request.SiteId.ToUpper()}_{spectrumName.ToUpper()}";

                                if (BulkWaiverUploadSelected)
                                {
                                    uploadFile = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "Waiver");
                                }
                                else
                                {
                                    uploadFile = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "SSV" && x.Filename.Contains(checkName));
                                }

                                request.BulkuploadPath = Path.GetFileName(BulkUploadPath);
                                request.BulkBatchNumber = batchNumber;
                                request.State = request.State.ToUpper();
                                var toClose = true;

                                if (BulkWaiverUploadSelected && ((requests.IndexOf(request) + 1) != requests.Count))
                                {
                                    toClose = false;
                                }

                                if (!await SaveSingleRequest(request, uploadFile, toClose))
                                {
                                    //throw new Exception(SaveError, new Exception(SaveError));
                                    //await Task.Delay(1000);

                                    //ErrorBtnOnClick();
                                    errorRequests.Add(request);
                                }
                            }

                            //await SFBulkASSV_Uploader.ClearAllAsync();
                            if ((requests.Count - errorRequests.Count) > 0)
                                await IRequest.SetCreateState(requests, variables);

                            await SFBulkAcceptance_Uploader.ClearAllAsync();

                            await InitializeForm();
                        }
                        else
                        {
                            if (BulkWaiverUploadSelected)
                            {
                                await SF_WaiverUploader.ClearAllAsync();

                                if (BulkRequestRRUCount > 0)
                                {
                                    BulkUploadRRUSSVFiles.ForEach(async file =>
                                    {
                                        await file.ClearAllAsync();
                                    });
                                }

                            }
                            else
                                await SFBulkASSV_Uploader.ClearAllAsync();

                            BulkUploadIconCss = "fas fa-paper-plane ml-2";
                            return;
                        }
                    }
                    else
                    {
                        await SFBulkAcceptance_Uploader.ClearAll();
                        await SFBulkASSV_Uploader.ClearAll();


                        if (BulkUploadError == null)
                        {
                            BulkUploadIconCss = "fas fa-paper-plane ml-2";
                            ToastContent = "No of Site Count does not match number of records uploaded.";
                            await Task.Delay(200);

                            ErrorBtnOnClick();
                        }
                        else
                        {
                            BulkUploadIconCss = "fas fa-paper-plane ml-2";
                            ToastContent = BulkUploadError;
                            await Task.Delay(200);

                            ErrorBtnOnClick();
                        }
                    }

                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                }

                AppState.TriggerRequestRecount();

                IsRRUType = false;
                EnableDisableActionButton(IsRRUType);
            }
            catch (Exception ex)
            {
                if (SingleEntrySelected)
                {
                    SEUploadIconCss = "fas fa-paper-plane ml-2";
                    //await SF_Uploader.ClearAll();
                }

                if (BulkUploadSelected)
                {
                    BulkUploadIconCss = "fas fa-paper-plane ml-2";

                    await SFBulkAcceptance_Uploader.ClearAllAsync();
                    if (BulkWaiverUploadSelected)
                    {
                        await SF_WaiverUploader.ClearAllAsync();

                        if (BulkRequestRRUCount > 0)
                        {
                            BulkUploadRRUSSVFiles.ForEach(async file =>
                            {
                                await file.ClearAllAsync();
                            });
                        }

                    }
                    else
                        await SFBulkASSV_Uploader.ClearAllAsync();

                    StateHasChanged();

                    await InitializeForm();
                }

                EnableDisableActionButton(IsRRUType);

                string msg = ex.InnerException?.Message ?? ex.Message;

                ToastContent = (msg.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";
                await Task.Delay(200);
                ErrorBtnOnClick();

                Logger.LogError("Error creating request. ", new { }, ex);
            }
        }

        private async Task<bool> ValidateSSVUploads(List<RequestViewModel> requests)
        {
            var isValidUploads = true;

            foreach (RequestViewModel request in requests)
            {
                var spectrumName = Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId).Name;

                var checkName = (spectrumName.Contains("RRU")) ? $"{request.SiteId.ToUpper()}" : $"{request.SiteId.ToUpper()}_{spectrumName.ToUpper()}";

                if (!BulkWaiverUploadSelected)
                {
                    var uploadFile = UploadedRequestFiles.FirstOrDefault(x => x.UploadType == "SSV" && x.Filename.Contains(checkName));

                    if (uploadFile?.Filename == null && !spectrumName.Contains("RRU"))
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
                            "Antenna Make", "Antenna Type", "Antenna Height", "Tower Height - (M)", "Antenna Azimuth", "M Tilt", "E Tilt", "Baseband", "RRU TYPE", "Power - (w)",
                            "Project Type", "Project Year", "Summer Config", "Software", "RRU Power - (w)", "CSFB Status GSM", "CSFB Status WCDMA",
                            "Integrated Date", "RET Configured", "Carrier Aggregation", "State"
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
                                BulkUploadError = $"Invalid Excel Template uploaded. Foreign Key Mismatch/Error on row:  {dt.Rows.IndexOf(row) + 1}, col: {BulkUploadColumnError}";
                                break;
                            }

                            request.AntennaMakeId = (request.AntennaMakeId != null) ? AntennaMakes.FirstOrDefault(x => x.Name.ToUpper() == request.AntennaMakeId.ToUpper())?.Id : request.AntennaMakeId;
                            request.AntennaTypeId = (request.AntennaTypeId != null) ? AntennaTypes.FirstOrDefault(x => x.Name.ToUpper() == request.AntennaTypeId.ToUpper())?.Id : request.AntennaTypeId;
                            request.BasebandId = (request.BasebandId != null) ? Basebands.FirstOrDefault(x => x.Name.ToUpper() == request.BasebandId.ToUpper())?.Id : request.BasebandId;
                            request.ProjectTypeId = (request.ProjectTypeId != null) ? ProjectTypes.FirstOrDefault(x => x.Name.ToUpper() == request.ProjectTypeId.ToUpper())?.Id : request.ProjectTypeId;
                            request.RegionId = (request.RegionId != null) ? Regions.FirstOrDefault(x => x.Name.ToUpper() == request.RegionId.ToUpper())?.Id : request.RegionId;
                            request.SummerConfigId = (request.SummerConfigId != null) ? SummerConfigs.FirstOrDefault(x => x.Name.ToUpper() == request.SummerConfigId.ToUpper())?.Id : request.SummerConfigId;
                            request.TechTypeId = (request.TechTypeId != null) ? TechTypes.FirstOrDefault(x => x.Name.ToUpper() == request.TechTypeId.ToUpper())?.Id : request.TechTypeId;
                            request.SpectrumId = (request.SpectrumId != null) ? Spectrums.FirstOrDefault(x => x.Name.ToUpper() == request.SpectrumId.ToUpper() && x.TechTypeId == request.TechTypeId)?.Id : request.SpectrumId;
                            request.RRUTypeId = (request.RRUTypeId != null) ? RRUTypes.FirstOrDefault(x => x.Name.ToUpper() == request.RRUTypeId.ToUpper())?.Id : request.RRUTypeId;

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
            bool result = IsFKValid(x => x.Name.ToUpper() == request.TechTypeId.ToUpper(), TechTypes)
            && IsFKValid(x => x.Name.ToUpper() == request.RegionId.ToUpper(), Regions)
            && IsFKValid(x => x.Name.ToUpper() == request.SpectrumId.ToUpper() && x.TechType.Name == request.TechTypeId, Spectrums)
            && IsFKValid(x => x.Name.ToUpper() == request.RRUTypeId.ToUpper(), RRUTypes)
            && IsFKValid(x => x.Name.ToUpper() == request.State.ToUpper(), NigerianStates)
            && IsFKValid(x => x.Name.ToUpper() == request.ProjectTypeId.ToUpper(), ProjectTypes);

            if (request.SiteName == null)
            {
                BulkUploadColumnError = "Site Name";
                result = false;
            }

            if (request.SummerConfigId != null)
            {
                result = result
                    && IsFKValid(x => x.Name.ToUpper() == request.SummerConfigId.ToUpper(), SummerConfigs);
            }

            //if(request.RRUTypeId != null)
            //{
            //    result = result
            //        && IsFKValid(x => x.Name.ToUpper() == request.RRUTypeId.ToUpper(), RRUTypes);
            //}

            if (request.BasebandId != null)
            {
                result = result
                    && IsFKValid(x => x.Name.ToUpper() == request.BasebandId.ToUpper(), Basebands);
            }

            if (request.AntennaMakeId != null)
            {
                result = result
                    && IsFKValid(x => x.Name.ToUpper() == request.AntennaMakeId.ToUpper(), AntennaMakes);
            }

            if (request.RETConfigured != null)
            {
                result = result
                    && IsFKValid(x => x.Name.ToUpper() == request.RETConfigured.ToUpper(), BoolDrops);
            }

            if (request.AntennaTypeId != null)
            {
                result = result
                    && IsFKValid(x => x.Name.ToUpper() == request.AntennaTypeId.ToUpper(), AntennaTypes);
            }

            if (RequestModel.TechTypeId != TechTypes.FirstOrDefault(x => x.Name == "4G")?.Id)
            {
                if (request.CarrierAggregation != null)
                {
                    result = result
                        && IsFKValid(x => x.Name.ToUpper() == request.CarrierAggregation.ToUpper(), BoolDrops);
                }
            }

            return result;
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
                request.SiteId = request.SiteId.ToUpper();
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
                    if (SingleEntrySelected)
                    {
                        request.RRUPower = LTEInput.RRUPower;
                        request.CSFBStatusGSM = LTEInput.CSFDStatusGSM;
                        request.CSFBStatusWCDMA = LTEInput.CSFDStatusWCDMA;
                    }
                }

                var (isCreated, errorMsg) = await IRequest.CreateRequest(request);

                if (isCreated)
                {
                    ToastContent = $"Request Submitted successfully {request.SiteId} - {Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId)?.Name}.";
                    return true;
                }

                ToastContent = $"Request could not be created. {errorMsg}";
                await Task.Delay(1000);

                ErrorBtnOnClick();
                return false;
            }
            catch (Exception ex)
            {
                ToastContent = (ex.InnerException.Message.Contains("unique")) ? "Duplicate entry found" : $"An error has occurred. {ex.Message}";

                await Task.Delay(1000);

                ErrorBtnOnClick();

                return false;
            }
        }

        private async Task<bool> SaveSingleRequest(RequestViewModel request, FilesManager file, bool toClose)
        {
            try
            {
                var spectrum = Spectrums.FirstOrDefault(x => x.Id == request.SpectrumId).Name;

                if (file?.UploadFile == null && !spectrum.Contains("RRU MOD"))
                {
                    SaveError = "Missing SSV Report";
                    return false;
                }

                if (file?.UploadFile != null)
                {
                    string ext = Path.GetExtension(file.UploadFile.FileInfo.Name);
                    bool isWaiver = (SingleEntrySelected) ? WaiverUploadSelected : BulkWaiverUploadSelected;
                    bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, isWaiver);

                    (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension, file, toClose);

                    request.SSVReport = Path.GetFileName(filePath);

                    if (uploadResp.Length != 0 || uploadError.Length != 0)
                    {
                        File.Delete(UploadPath);

                        if (!BulkUploadSelected)
                            await SF_Uploader.ClearAll();
                        else
                            await SFBulkASSV_Uploader.ClearAll();

                        BulkUploadIconCss = "fas fa-paper-plane ml-2";

                        ToastContent = $"An error has occurred. SSV Report/Waiver could not be uploaded.";
                        await Task.Delay(1000);

                        ErrorBtnOnClick();

                        return false;
                    }
                }

                if (await SaveRequest(request))
                {
                    BulkUploadIconCss = "fas fa-paper-plane ml-2";
                    await Task.Delay(1000);

                    SuccessBtnOnClick();

                    return true;
                }

                BulkUploadIconCss = "fas fa-paper-plane ml-2";

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
                ToastContent = $"An error has occurred. {ex.Message}.";
                await Task.Delay(1000);

                ErrorBtnOnClick();

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
                return await Task.Run<(string, string)>(() =>
                {
                    bufile.UploadFile.Stream.WriteTo(bufile.Filestream);

                    if (toClose)
                    {
                        bufile.Filestream.Close();
                        bufile.UploadFile.Stream.Close();
                    }

                    return ("", bufile.UploadPath);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
                return (ex.Message, "");
            }
        }

        private async Task<bool> OnFileUploadChange(UploadChangeEventArgs args, string type, int uploadIndex)
        {
            try
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

                    if (type == "Bulk")
                    {
                        bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, false);
                        Spectrums = await ISpectrum.Get(x => x.IsActive);

                        (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension, file, true);

                        if (uploadResp.Length == 0 && uploadError.Length == 0)
                        {
                            BulkUploadPath = filePath;
                            BulkUploadData = await ProcessUpload(file.UploadPath);

                            if (BulkUploadData.requests.Count == 0)
                            {
                                await SFBulkAcceptance_Uploader.ClearAllAsync();

                                EnableDisableActionButton(IsRRUType);

                                ToastContent = $"No Valid Request found! {BulkUploadData.error} - {BulkUploadError}";
                                await Task.Delay(200);

                                ErrorBtnOnClick();

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

                            var RRUSpectrums = Spectrums.Where(y => y.Name.Contains("RRU")).Select(x => x.Id).ToList();

                            BulkRequestRRUData = BulkUploadData.requests.Where(x => RRUSpectrums.Contains(x.SpectrumId) && !BulkRequestInvalidData.Contains(x)).ToList();
                            BulkRequestRRUCount = BulkRequestRRUData.Count;

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

                //await RemoveSSVLeavingRRUs();
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
            DisableSEButton = true;

            if (SingleEntrySelected && SingleEntryValid && (ssvUpload?.UploadFile != null || IsRRUType))
            {
                DisableSEButton = false;
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
                    else if (!BulkWaiverUploadSelected && (Input.BUSiteCount - BulkRequestRRUCount) != ssvUploadCount)
                    {
                        disable = true;
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

                if (BulkUploadData.requests != null)
                    if (BulkUploadData.requests.Count == 0)
                    {
                        disable = true;
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

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
using System.ComponentModel.DataAnnotations;
using Project.V1.DLL.Helpers;

namespace Project.V1.Web.Pages.Acceptance.Engineer
{
    public partial class ActionRequest : IDisposable
    {
        [Parameter] public string Id { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected IRRUType IRRUType { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IBaseBand IBaseBand { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] AppState AppState { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<RRUTypeModel> RRUTypes { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<BaseBandModel> Basebands { get; set; }
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

        public ActionReason Reason { get; set; } = new()
        {
            Id = Guid.NewGuid().ToString(),
        };

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
                ResetUpload();
        }

        private void OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count > 0)
                ResetUpload();
        }

        public async Task OnTechChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, TechTypeModel> args)
        {
            Spectrums = await ISpectrum.Get(x => x.TechTypeId == args.Value);
        }

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);


            Regions = await IRegion.Get(x => x.IsActive);
            SummerConfigs = await ISummerConfig.Get(x => x.IsActive);
            ProjectTypes = await IProjectType.Get(x => x.IsActive);
            RRUTypes = await IRRUType.Get(x => x.IsActive);
            TechTypes = await ITechType.Get(x => x.IsActive);
            AntennaTypes = await IAntennaType.Get(x => x.IsActive);
            AntennaMakes = await IAntennaMake.Get(x => x.IsActive);
            Basebands = (Principal.IsInRole("Super Admin"))
                ? await IBaseBand.Get(x => x.IsActive)
                : await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId);

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

                        RequestModel = (await IRequest.GetById(x => x.Id == Id));
                        //RequestStatus = RequestModel.Status;
                        //RequestModel.Status = null;
                        Spectrums = await ISpectrum.Get(x => x.IsActive && x.TechTypeId == RequestModel.TechTypeId);

                        DateTime dt = DateTime.Now;
                        //EnableSpecial = (dt.Day == DateTime.DaysInMonth(dt.Year, dt.Month));
                        int lastDayOfPrevMth = DateTime.DaysInMonth(dt.Year, dt.Month - 1);
                        EnableSpecial = (dt.Day == 1 || dt.Day == 2);
                        MinDateTime = new DateTime(dt.Year, dt.Month - 1, lastDayOfPrevMth);
                        MaxDateTime = new DateTime(dt.Year, dt.Month, 2);

                        return;
                    }

                    await InitializeForm();

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
                    string ext = Path.GetExtension(UploadFiles.FileInfo.Name);

                    bool allowedExtension = ExcelProcessor.IsAllowedExt(ext, false);

                    (string uploadResp, string filePath, string uploadError) = await StartUpload(allowedExtension);

                    if (uploadResp.Length != 0 || uploadError.Length != 0)
                    {
                        ToastContent = "Error occured, could not upload file";

                        await Task.Delay(200);
                        ErrorBtnOnClick();

                        return;
                    }
                }

                //RequestModel.Status = RequestStatus;

                await IRequest.SetState(RequestModel);

                if (ProcessAction(RequestModel, variables, IRequest))
                {
                    AppState.TriggerRequestRecount();
                    NavMan.NavigateTo("acceptance/engineer/worklist");
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

        private bool ProcessAction<T>(T requestClass, Dictionary<string, object> variables, dynamic requests) where T : RequestViewModel
        {
            try
            {
                requestClass.EngineerAssigned.IsActioned = true;
                requestClass.EngineerAssigned.Fullname = User.Fullname;
                requestClass.EngineerAssigned.Email = User.Email;
                requestClass.EngineerAssigned.PhoneNo = User.PhoneNumber;
                requestClass.EngineerAssigned.IsApproved = (Input.Status == "Accepted");
                requestClass.EngineerAssigned.ApproverComment = Input.Comment;
                requestClass.EngineerAssigned.DateApproved = (Input.Status == "Accepted") ? DateTimeOffset.UtcNow.DateTime : DateTime.MinValue;
                requestClass.EngineerAssigned.DateAssigned = DateTimeOffset.UtcNow.DateTime;
                requestClass.Status = Input.Status;

                if (requestClass.EngineerAssigned.IsApproved)
                {
                    requests.Accept(requestClass, variables);

                    return true;
                }

                requests.Reject(requestClass, variables, requestClass.EngineerAssigned.ApproverComment);

                return true;
            }
            catch
            {
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

        private async Task<bool> OnFileUploadChange(UploadChangeEventArgs args, string type)
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

        public void Dispose()
        {
            //RequestModel.Status = RequestStatus;

            GC.SuppressFinalize(this);
        }
    }
}

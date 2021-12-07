using Microsoft.AspNetCore.Components;
using Project.V1.DLL.Interface;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.SplitButtons;
using Project.V1.Lib.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Project.V1.Web.Pages.Acceptance.Shared
{
    public partial class SingleRequestControl
    {
        [Parameter] public bool ShouldEnable { get; set; }
        [Parameter] public bool ShowRequired { get; set; }
        [Parameter] public bool ShowSSVUpload { get; set; } = true;
        //[Parameter] public bool DisableSEButton { get; set; }
        //[Parameter] public string SEUploadIconCss { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        [Parameter] public RequestViewModel RequestModel { get; set; }
        [Parameter] public EventCallback<bool> CheckValid { get; set; }
        [Parameter] public List<FilesManager> UploadedRequestFiles { get; set; }
        [Parameter] public EventCallback<bool> OnCheckValidButton { get; set; }
        [Parameter] public EventCallback<ClearingEventArgs> OnClear { get; set; }
        [Parameter] public EventCallback<RemovingEventArgs> OnRemove { get; set; }
        [Parameter] public EventCallback<List<SpectrumViewModel>> TechChanged { get; set; }
        [Parameter] public EventCallback<UploadChangeEventArgs> OnFileSSVUploadChange { get; set; }


        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected IProjects IProjects { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IBaseBand IBaseBand { get; set; }
        [Inject] protected IUser IUser { get; set; }


        public string showRqdClass { get; set; } = "inline-block";
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<ProjectModel> Projects { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<BaseBandModel> Basebands { get; set; }

        protected bool SingleEntrySelected { get; set; }
        protected SfButtonGroup SingleSelector { get; set; }
        protected ButtonGroupButton SingleSSVSelector { get; set; }
        protected ButtonGroupButton SingleWaiverSelector { get; set; }

        public List<RequestViewModel> BulkRequestInvalidData { get; set; } = new();


        protected SfUploader SF_Uploader { get; set; }

        public double MaxFileSize { get; set; } = 25000000;
        public DateTime MaxDateTime { get; set; }
        public DateTime MinDateTime { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public bool SSVUploadSelected { get; set; } = true;
        public bool WaiverUploadSelected { get; set; }
        public bool IsRRUType { get; set; } = false;

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

        public List<NigerianState> NigerianStates { get; set; } = States.Select(x => new NigerianState { Name = x.ToUpper() }).ToList();

        public class BoolDropDown
        {
            public string Name { get; set; }
        }

        public class NigerianState
        {
            public string Name { get; set; }
        }

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);

            Regions = (await IRegion.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            SummerConfigs = (await ISummerConfig.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            ProjectTypes = (await IProjectType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            Projects = (User.Vendor.Name == "MTN Nigeria") ? (await IProjects.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList() : (await IProjects.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();
            TechTypes = (await ITechType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaMakes = (await IAntennaMake.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaTypes = (await IAntennaType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            Spectrums = (RequestModel.TechTypeId == null) ? new() : (await ISpectrum.Get(x => x.IsActive && x.TechTypeId == RequestModel.TechTypeId)).OrderBy(x => x.Name).ToList();
            Basebands = (Principal.IsInRole("Super Admin"))
                ? (await IBaseBand.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList()
                : (await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();

            await OnCheckValidButton.InvokeAsync(false);

            if (!ShowRequired)
                showRqdClass = "none";
        }

        protected override async Task OnInitializedAsync()
        {


            await InitializeForm();
        }

        private async Task CheckIfSEValid()
        {
            if(RequestModel.SiteId == null || RequestModel.SpectrumId == null)
                await CheckValid.InvokeAsync(false);

            if (RequestModel.SiteId != null && RequestModel.SpectrumId != null)
            {
                RequestModel.SiteId = RequestModel.SiteId.ToUpper();

                var SingleEntryValid = await IRequest.GetValidRequest(RequestModel);

                await CheckValid.InvokeAsync(SingleEntryValid);
            }

            await OnCheckValidButton.InvokeAsync(IsRRUType);
        }

        public async Task OnTechChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, TechTypeModel> args)
        {
            try
            {
                IsRRUType = false;
                Spectrums = (await ISpectrum.Get(x => x.TechTypeId == args.Value && x.IsActive)).OrderBy(x => x.Name).ToList();

                await TechChanged.InvokeAsync(Spectrums);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
            }

            await OnCheckValidButton.InvokeAsync(IsRRUType);
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

            await OnCheckValidButton.InvokeAsync(IsRRUType);

            await Task.CompletedTask;
        }
    }
}

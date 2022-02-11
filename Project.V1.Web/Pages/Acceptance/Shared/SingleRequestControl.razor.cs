using Project.V1.Web.Request;

namespace Project.V1.Web.Pages.Acceptance.Shared
{
    public partial class SingleRequestControl
    {
        [Parameter] public bool ShouldEnable { get; set; }
        [Parameter] public bool ShowRequired { get; set; }
        [Parameter] public bool ShowSSVUpload { get; set; } = true;
        [Parameter] public RequestViewModel RequestModel { get; set; }
        [Parameter] public EventCallback<bool> CheckValid { get; set; }
        [Parameter] public List<FilesManager> UploadedRequestFiles { get; set; }
        [Parameter] public EventCallback<bool> OnCheckValidButton { get; set; }
        [Parameter] public EventCallback<ClearingEventArgs> OnClear { get; set; }
        [Parameter] public EventCallback<RemovingEventArgs> OnRemove { get; set; }
        [Parameter] public EventCallback<UploadChangeEventArgs> OnFileSSVUploadChange { get; set; }


        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRequestListObject IRequestList { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IUser IUser { get; set; }


        public string ShowRqdClass { get; set; } = "inline-block";
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        protected bool SingleEntrySelected { get; set; }
        protected SfButtonGroup SingleSelector { get; set; }
        protected ButtonGroupButton SingleSSVSelector { get; set; }
        protected ButtonGroupButton SingleWaiverSelector { get; set; }

        public List<RequestViewModel> BulkRequestInvalidData { get; set; } = new();


        protected SfUploader SF_Uploader { get; set; }

        public double MaxFileSize { get; set; } = 25000000;
        public DateTime MaxDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public DateTime MinDateTime { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public bool SSVUploadSelected { get; set; } = true;
        public bool WaiverUploadSelected { get; set; }
        public bool IsRRUType { get; set; } = false;

        protected Dictionary<string, object> DescriptionHtmlAttribute { get; set; } = new Dictionary<string, object>()
        {
            { "rows", "3" },
        };

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

            await IRequestList.Initialize(Principal);

            if (RequestModel.TechTypeId != null)
                IRequestList.Spectrums = await ISpectrum.Get(x => x.IsActive, x => x.OrderBy(y => y.Name));

            await OnCheckValidButton.InvokeAsync(false);

            if (!ShowRequired)
                ShowRqdClass = "none";
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeForm();
        }

        private async Task CheckIfSEValid()
        {
            if (RequestModel.SiteId == null || RequestModel.SpectrumId == null)
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
                IRequestList.Spectrums = (await ISpectrum.Get(x => x.TechTypeId == args.Value && x.IsActive)).OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, new { }, ex);
            }

            await OnCheckValidButton.InvokeAsync(IsRRUType);
        }

        public async Task OnProjectTypeChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, ProjectTypeModel> args)
        {
            await OnCheckValidButton.InvokeAsync(IsRRUType);
            await Task.CompletedTask;
        }

        public async Task OnSpectrumChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, SpectrumViewModel> args)
        {
            var specturm = IRequestList.Spectrums.FirstOrDefault(x => x.Id == args.Value)?.Name;

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

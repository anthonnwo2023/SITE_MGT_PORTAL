namespace Project.V1.Web.Pages.Acceptance
{
    public partial class ReportDetail : IDisposable
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
        [Inject] protected IProjects IRRUType { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IBaseBand IBaseBand { get; set; }
        [Inject] protected IUser IUser { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        public string UploadIconCss { get; set; } = "fas fa-paper-plane ml-2";
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<PathInfo> Paths { get; set; }
        public List<RegionViewModel> Regions { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<ProjectModel> RRUTypes { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<BaseBandModel> Basebands { get; set; }
        public RequestViewModel RequestModel { get; set; }
        public string RequestStatus { get; set; }

        private void RedirectCancelled(MouseEventArgs args)
        {
            NavMan.NavigateTo($"acceptance/worklist/{Id}");
        }

        private static readonly string[] States = new string[]
{
            "Abia", "Adamawa", "Akwa Ibom", "Anambra", "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta", "Ebonyi", "Edo", "Ekiti",
            "Enugu", "FCT - Abuja", "Gombe", "Imo", "Jigawa", "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara","Lagos", "Nasarawa", "Niger",
            "Ogun", "Ondo", "Osun", "Oyo", "Plateau", "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
};

        public List<NigerianState> NigerianStates { get; set; } = States.Select(x => new NigerianState { Name = x }).ToList();

        public class NigerianState
        {
            public string Name { get; set; }
        }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Report Detail", Link = $"acceptance/engineer/worklist/{Id}" },
                new PathInfo { Name = $"Report", Link = "acceptance/report" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
            User = await IUser.GetUserByUsername(Principal.Identity.Name);


            Regions = await IRegion.Get(x => x.IsActive);
            SummerConfigs = await ISummerConfig.Get(x => x.IsActive);
            ProjectTypes = await IProjectType.Get(x => x.IsActive, null, "Spectrum");
            RRUTypes = await IRRUType.Get(x => x.IsActive);
            TechTypes = await ITechType.Get(x => x.IsActive);
            AntennaTypes = await IAntennaType.Get(x => x.IsActive);
            AntennaMakes = await IAntennaMake.Get(x => x.IsActive);
            Basebands = (Principal.IsInRole("Super Admin"))
                ? await IBaseBand.Get(x => x.IsActive)
                : await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId);

            RequestModel = new();
            RequestModel.DateSubmitted = DateTime.Now;
            RequestModel.IntegratedDate = DateTime.Now;
        }

        private void ResetUpload()
        {
        }

        public async Task OnTechChange(List<SpectrumViewModel> spectrums)
        {
            Spectrums = spectrums;

            await Task.CompletedTask;
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
        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    Logger.LogInformation("Loading action request page", new { });

                    if (Id != null)
                    {
                        if (!await UserAuth.IsAutorizedForAsync("Can:ViewReport"))
                        {
                            NavMan.NavigateTo("access-denied");
                            return;
                        }

                        await InitializeForm();

                        RequestModel = await IRequest.GetById(x => x.Id == Id, null, "EngineerAssigned,Requester.Vendor,AntennaMake,AntennaType");
                        RequestStatus = RequestModel.Status;
                        Spectrums = await ISpectrum.Get(x => x.IsActive && x.TechTypeId == RequestModel.TechTypeId);

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

        private async Task OnFileUploadChange(UploadChangeEventArgs args, string type)
        {
            await Task.CompletedTask;
        }

        private void EnableDisableActionButton(bool IsSERRUType)
        {

        }

        private void IsSEValid(bool SEValid)
        {

        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

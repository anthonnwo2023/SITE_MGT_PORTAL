namespace Project.V1.Web.Requests
{
    public class RequestListObject : IRequestListObject
    {
        private readonly IAntennaType IAntennaType;
        private readonly IBaseBand IBaseBand;
        private readonly IAntennaMake IAntennaMake;
        private readonly ITechType ITechType;
        private readonly IProjects IProjects;
        private readonly IProjectType IProjectType;
        private readonly ISummerConfig ISummerConfig;
        private readonly ISpectrum ISpectrum;
        private readonly IRegion IRegion;
        private readonly IUser IUser;

        public List<RegionViewModel> Regions { get; set; }
        public List<SummerConfigModel> SummerConfigs { get; set; }
        public List<SpectrumViewModel> Spectrums { get; set; }
        public List<ProjectTypeModel> ProjectTypes { get; set; }
        public List<ProjectModel> Projects { get; set; }
        public List<TechTypeModel> TechTypes { get; set; }
        public List<AntennaMakeModel> AntennaMakes { get; set; }
        public List<AntennaTypeModel> AntennaTypes { get; set; }
        public List<BaseBandModel> Basebands { get; set; }
        public ApplicationUser User { get; set; }

        public RequestListObject(IAntennaType AntennaType, IBaseBand BaseBand, IAntennaMake AntennaMake, ITechType TechType, IProjects Projects, IProjectType ProjectType,
            ISummerConfig SummerConfig, ISpectrum Spectrum, IRegion Region, IUser User)
        {
            IAntennaType = AntennaType;
            IBaseBand = BaseBand;
            IAntennaMake = AntennaMake;
            ITechType = TechType;
            IProjects = Projects;
            IProjectType = ProjectType;
            ISummerConfig = SummerConfig;
            ISpectrum = Spectrum;
            IRegion = Region;
            IUser = User;
        }

        public async Task Initialize(ClaimsPrincipal Principal, string objType)
        {
            User = await IUser.GetUserByUsername(Principal.Identity.Name);
            TechTypes = (await ITechType.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();

            if (objType == "SMPObject")
            {
                Regions = (await IRegion.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();
                SummerConfigs = (await ISummerConfig.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();
                ProjectTypes = (await IProjectType.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();
                Projects = (((User.Vendor.Name == "MTN Nigeria") ? await IProjects.Get(x => x.IsActive, x => x.OrderBy(x => x.Name)) : await IProjects.Get(x => x.IsActive && x.VendorId == User.VendorId, x => x.OrderBy(x => x.Name)))).ToList();
                AntennaMakes = (await IAntennaMake.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();
                AntennaTypes = (await IAntennaType.Get(x => x.IsActive, x => x.OrderBy(y => y.Name))).ToList();
                Spectrums = (await ISpectrum.Get(x => x.IsActive, x => x.OrderBy(y => y.Name), "TechType")).ToList();
                Basebands = (Principal.IsInRole("Super Admin"))
                    ? (await IBaseBand.Get(x => x.IsActive, null, "Vendor")).OrderBy(x => x.Name).ToList()
                    : (await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId, null, "Vendor")).OrderBy(x => x.Name).ToList();
            }
        }
    }
}

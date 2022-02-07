﻿namespace Project.V1.Web.Request
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

        public async Task Initialize(ClaimsPrincipal Principal)
        {
            User = await IUser.GetUserByUsername(Principal.Identity.Name);

            Regions = (await IRegion.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            SummerConfigs = (await ISummerConfig.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            ProjectTypes = (await IProjectType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            Projects = (User.Vendor.Name == "MTN Nigeria") ? (await IProjects.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList() : (await IProjects.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();
            TechTypes = (await ITechType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaMakes = (await IAntennaMake.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            AntennaTypes = (await IAntennaType.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList();
            Spectrums = new();
            Basebands = (Principal.IsInRole("Super Admin"))
                ? (await IBaseBand.Get(x => x.IsActive)).OrderBy(x => x.Name).ToList()
                : (await IBaseBand.Get(x => x.IsActive && x.VendorId == User.VendorId)).OrderBy(x => x.Name).ToList();
        }
    }
}
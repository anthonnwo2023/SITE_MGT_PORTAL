namespace Project.V1.Web.Request
{
    public interface IRequestListObject
    {
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

        Task Initialize(ClaimsPrincipal Principal, string objType);
    }
}
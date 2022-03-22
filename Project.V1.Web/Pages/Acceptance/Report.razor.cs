﻿namespace Project.V1.Web.Pages.Acceptance
{
    public partial class Report
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IRegion IRegion { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected IVendor IVendor { get; set; }
        [Inject] protected ITechType ITechType { get; set; }
        [Inject] protected ISpectrum ISpectrum { get; set; }
        [Inject] protected IAntennaMake IAntennaMake { get; set; }
        [Inject] protected IAntennaType IAntennaType { get; set; }
        [Inject] protected IBaseBand IBaseband { get; set; }
        [Inject] protected IProjects IRRUType { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }
        [Inject] protected IProjects IProject { get; set; }
        [Inject] protected ISummerConfig ISummerConfig { get; set; }
        [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

        List<RequestViewModel> Requests { get; set; }
        List<RegionViewModel> Regions { get; set; }
        List<TechTypeModel> TechTypes { get; set; }
        List<SpectrumViewModel> Spectrums { get; set; }
        List<AntennaMakeModel> AntennaMakes { get; set; }
        List<AntennaTypeModel> AntennaTypes { get; set; }
        List<BaseBandModel> Basebands { get; set; }
        List<ProjectModel> RRUTypes { get; set; }
        List<ProjectTypeModel> ProjectTypes { get; set; }
        List<ProjectModel> Projects { get; set; }
        List<SummerConfigModel> SummerConfigs { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public VendorModel Vendor { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected SfGrid<RequestViewModel> Grid_Request { get; set; }
        protected SfGrid<RequestViewModel> Grid_RequestGroup { get; set; }

        public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Report", Link = "acceptance/reports/general" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "ReportTable_excelexport") //Id is combination of Grid's ID and itemname
            {
                var hiddenCols = new string[] {
                    "Vendor",
                    "Site Name",
                    "RNC/BSC",
                    "Spectrum",
                    "Bandwidth",
                    "Latitude",
                    "Longitude",
                    "Antenna Make",
                    "Antenna Type",
                    "Antenna Height",
                    "M Tilt",
                    "E Tilt",
                    "Baseband",
                    "RRU Type",
                    "Power - (w)",
                    "Project Name",
                    "Project Type",
                    "Project Year",
                    "Summer Config",
                    "Software Version",
                    "RRU Power - (w)",
                    "CSFB Status GSM",
                    "CSFB Status WCDMA",
                    "RET Configured",
                    "Carrier Aggregation",
                    "Engineer",
                    "Date Integrated",
                    "Date Submitted",
                    "Date Actioned",
                    "Requester Comment",
                    "Engineer Comment",
                };

                await Grid_Request.ShowColumnsAsync(hiddenCols);

                ExcelExportProperties ExportProperties = new();
                ExportProperties.FileName = $"General_Report{DateTimeOffset.UtcNow:ddMMyyyy.Hmmss}.xlsx";
                ExportProperties.IncludeHiddenColumn = true;

                await Grid_Request.ExcelExport(ExportProperties);
                await Grid_Request.HideColumnsAsync(hiddenCols);
            }

            if (args.Item.Id == "ReportGroupTable_excelexport") //Id is combination of Grid's ID and itemname
            {
                //var hiddenCols = new string[] {
                //    "RNC/BSC",
                //    "Spectrum",
                //    "Bandwidth",
                //    "Latitude",
                //    "Longitude",
                //    "Antenna Make",
                //    "Antenna Type",
                //    "Antenna Height",
                //    "M Tilt",
                //    "E Tilt",
                //    "Baseband",
                //    "RRU Type",
                //    "Power - (w)",
                //    "Project Type",
                //    "Project Year",
                //    "Summer Config",
                //    "Software Version",
                //    "RRU Power - (w)",
                //    "CSFB Status GSM",
                //    "CSFB Status WCDMA",
                //    "RET Configured",
                //    "Carrier Aggregation",
                //    "Date Integrated",
                //    "Date Submitted",
                //};

                //await Grid_RequestGroup.ShowColumnsAsync(hiddenCols);
                await Grid_RequestGroup.ExcelExport();
                //await Grid_RequestGroup.HideColumnsAsync(hiddenCols);
            }
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    if (!await UserAuth.IsAutorizedForAsync("Can:ViewReport"))
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                    Vendor = await IVendor.GetById(x => x.Id == User.VendorId);

                    if (User.ShowAllRegionReport)
                    {
                        Requests = await IRequest.Get(x => x.Id != null, x => x.OrderByDescending(y => y.DateCreated));
                    }
                    else if (Vendor.Name == "MTN Nigeria" || (await UserManager.IsInRoleAsync(User, "User")))
                    {
                        Requests = await IRequest.Get(x => User.Regions.Select(x => x.Id).Contains(x.RegionId), x => x.OrderByDescending(y => y.DateCreated));
                    }
                    else
                    {
                        Requests = await IRequest.Get(x => x.Requester.VendorId == User.VendorId, x => x.OrderByDescending(y => y.DateCreated));
                    }

                    TechTypes = await ITechType.Get(x => x.IsActive);
                    Regions = await IRegion.Get(x => x.IsActive);
                    Spectrums = await ISpectrum.Get(x => x.IsActive);
                    AntennaMakes = await IAntennaMake.Get(x => x.IsActive);
                    AntennaTypes = await IAntennaType.Get(x => x.IsActive);
                    Basebands = await IBaseband.Get(x => x.IsActive);
                    RRUTypes = await IRRUType.Get(x => x.IsActive);
                    Projects = await IProject.Get(x => x.IsActive);
                    ProjectTypes = await IProjectType.Get(x => x.IsActive);
                    SummerConfigs = await ISummerConfig.Get(x => x.IsActive);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

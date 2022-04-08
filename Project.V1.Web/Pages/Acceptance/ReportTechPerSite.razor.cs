namespace Project.V1.Web.Pages.Acceptance
{
    public partial class ReportTechPerSite
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected IVendor IVendor { get; set; }


        List<RequestViewModel> RequestsGroup { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public VendorModel Vendor { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected SfGrid<RequestViewModel> Grid_RequestGroup { get; set; }

        public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Technology Per Site Report", Link = "acceptance/reports/site-technology" },
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
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
                    Principal = (await AuthenticationStateTask).User;
                    User = await IUser.GetUserByUsername(Principal.Identity.Name);
                    Vendor = await IVendor.GetById(x => x.Id == User.VendorId);

                    if (!await UserAuth.IsAutorizedForAsync("Can:ViewReportTPS")/* || Vendor.Name != "MTN Nigeria"*/)
                    {
                        NavMan.NavigateTo("access-denied");
                        return;
                    }

                    RequestsGroup = (await IRequest.Get(x => x.EngineerAssigned.DateApproved != DateTime.MinValue
                    && !x.Spectrum.Name.Contains("MOD") && !x.ProjectType.Name.Contains("MOD"), x => x.OrderByDescending(y => y.DateCreated), "EngineerAssigned,Requester.Vendor,AntennaMake,AntennaType,Spectrum,TechType,Region")).GroupBy(x => x.SiteId)
                        .Select(x => new RequestViewModel
                        {
                            SiteId = x.Key,
                            SiteName = x.Select(x => x.SiteName).First(),
                            RegionId = x.Select(x => x.Region.Name).First(),
                            SpectrumId = string.Join(", ", x.Select(x => x.Spectrum.Name).Distinct()),
                            TechTypeId = string.Join(", ", x.Select(x => x.TechType.Name).Distinct()),
                        }).ToList();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

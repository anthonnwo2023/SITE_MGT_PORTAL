namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Report
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] protected IUserAuthentication UserAuth { get; set; }
        [Inject] protected NavigationManager NavMan { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUser IUser { get; set; }
        [Inject] protected IVendor IVendor { get; set; }

        //List<RequestViewModel> Requests { get; set; }
        public VendorModel Vendor { get; set; }

        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        //protected SfGrid<RequestViewModel> Grid_Request { get; set; }

        public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Report", Link = "halt/reports/general" },
                new PathInfo { Name = $"Site Halt & Unhalt", Link = "halt" },
            };
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "ReportTable_excelexport") //Id is combination of Grid's ID and itemname
            {
                var hiddenCols = new string[] {
                    "Site Id",
                };

                //await Grid_Request.ShowColumnsAsync(hiddenCols);

                //ExcelExportProperties ExportProperties = new();
                //ExportProperties.FileName = $"General_Report{DateTimeOffset.UtcNow:ddMMyyyy.Hmmss}.xlsx";

                //await Grid_Request.ExcelExport(ExportProperties);
                //await Grid_Request.HideColumnsAsync(hiddenCols);
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

                    //Requests = await IRequest.Get(x => x.Requester.VendorId == User.VendorId, x => x.OrderByDescending(y => y.DateCreated));

                    //if (Vendor.Name == "MTN Nigeria")
                    //{
                    //    Requests = await IRequest.Get(x => User.Regions.Select(x => x.Id).Contains(x.RegionId), x => x.OrderByDescending(y => y.DateCreated));
                    //}
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

namespace Project.V1.Web.Pages.Acceptance;

public partial class Report : ComponentBase
{
    public List<PathInfo> Paths { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public IHttpContextAccessor Context { get; set; }
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IRequest IRequest { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] protected IVendor IVendor { get; set; }
    [Inject] protected IRequestListObject IRequestList { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

    [Inject] public HttpClient httpClient { get; set; }
    List<RequestViewModel> Requests { get; set; }
    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public VendorModel Vendor { get; set; }
    public string ErrorDetails = "";

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
    protected SfGrid<RequestViewModelDTO> Grid_Request { get; set; }
    private Syncfusion.Blazor.Data.Query QueryData { get; set; }
    private Dictionary<string, string> HeaderData = new();
    protected SfGrid<RequestViewModelDTO> Grid_RequestGroup { get; set; }

    public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };
    public SfDataManager dm { get; set; }

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"Report", Link = "acceptance/reports/general" },
            new PathInfo { Name = $"Acceptance", Link = "acceptance" },
        };
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            dm.DataAdaptor = new ODataClassHelper(dm);
        }

        base.OnAfterRender(firstRender);
        RemoteOptions Rm = (dm.DataAdaptor as ODataV4Adaptor).Options;
        Rm.EnableODataSearchFallback = true;
        (dm.DataAdaptor as ODataV4Adaptor).Options = Rm;
    }

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == "ReportTable_excelexport") //Id is combination of Grid's ID and itemname
        {
            //var hiddenCols = new string[] {
            //    "Vendor",
            //    "Site Name",
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
            //    "Project Name",
            //    "Project Type",
            //    "Project Year",
            //    "Summer Config",
            //    "Software Version",
            //    "RRU Power - (w)",
            //    "CSFB Status GSM",
            //    "CSFB Status WCDMA",
            //    "RET Configured",
            //    "Carrier Aggregation",
            //    "Engineer",
            //    "Date Integrated",
            //    "Date Submitted",
            //    "Date Actioned",
            //    "Requester Comment",
            //    "Engineer Comment",
            //};

            //await Grid_Request.ShowColumnsAsync(hiddenCols);

            ExcelExportProperties ExportProperties = new();
            ExportProperties.FileName = $"General_Report{DateTimeOffset.UtcNow:ddMMyyyy.Hmmss}.xlsx";
            ExportProperties.IncludeHiddenColumn = false;
            ExportProperties.IncludeTemplateColumn = true;

            await Grid_Request.ShowSpinnerAsync();
            await Grid_Request.ExcelExport(ExportProperties);
            //await Grid_Request.HideColumnsAsync(hiddenCols);
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
            await Grid_RequestGroup.ShowSpinnerAsync();
            await Grid_RequestGroup.ExcelExport();
            //await Grid_RequestGroup.HideColumnsAsync(hiddenCols);
        }
    }

    public void ExcelQueryCellInfoHandler(ExcelQueryCellInfoEventArgs<RequestViewModelDTO> args)
    {
        if (args.Column.HeaderText == "Date Actioned")
        {
            var request = args.Data;
            var data = string.Empty;

            if (request.Status != "Accepted" && request.Status != "Rejected")
            {
                data = request.DateUserActioned != null ? request.DateUserActioned.GetValueOrDefault()!.Date.ToShortDateString() : request.DateSubmitted.Date.ToShortDateString();
            }
            else
            {
                data = request.EngineerAssignedIsApproved ? request.EngineerAssignedDateApproved.Date.ToShortDateString() : request.EngineerAssignedDateActioned.Date.ToShortDateString();
            }

            args.Cell.Value = data;
        }
    }

    public async void ExportCompleteHandler(object args)
    {
        await Grid_Request.HideSpinnerAsync();
        //await Grid_RequestGroup.HideSpinnerAsync();
    }

    public void ActionFailure(Syncfusion.Blazor.Grids.FailureEventArgs args)
    {
        this.ErrorDetails = $"Server Error Occured. {args.Error.Message} - {args.Error.InnerException} - {args.Error.StackTrace}";
        StateHasChanged();
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

                httpClient = HttpClientFactory.CreateClient("RequestClient");
                httpClient.BaseAddress = new Uri(NavMan.BaseUri);
                HeaderData.Add("User", Context.HttpContext.User.Identity.Name);
                HeaderData.Add("IsAuthenticated", Context.HttpContext.User.Identity.IsAuthenticated.ToString());
                HeaderData.Add("Claims", string.Join(", ", Context.HttpContext.User.Claims.Select(x => x.Value)));
                
                Principal = (await AuthenticationStateTask).User;

                QueryData = new Syncfusion.Blazor.Data.Query().Sort("datecreated desc", "");
                
                await IRequestList.Initialize(Principal, "SMPObject");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
            }
        }
    }
}

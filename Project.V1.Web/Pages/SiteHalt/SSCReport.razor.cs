namespace Project.V1.Web.Pages.SiteHalt;

public partial class SSCReport : IDisposable
{
    public List<PathInfo> Paths { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public IHttpContextAccessor Context { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] public ISSCRequestUpdate ISSCRequest { get; set; }
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }

    List<SSCUpdatedCell> SSCReportRequests { get; set; } = new();

    public SSCUpdatedCell SSCRequestModel { get; set; }

    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public string ButtonProcessingIconCss { get; set; } = "fas fa-spin fa-spinner m-1 text-white";
    public string ButtonIconCss { get; set; } = "fas fa-paper-plane ml-2";

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject] public HttpClient HttpClient { get; set; }
    protected SfGrid<SSCUpdatedCell> Grid_SSRequest { get; set; }

    private Syncfusion.Blazor.Data.Query QueryData { get; set; }

    private Dictionary<string, string> HeaderData = new();
    public bool UserIsAdmin { get; set; }
    protected bool[] CompleteButtons { get; set; }
    protected bool[] UpdateButtons { get; set; }
    private string RequestUniqueId { get; set; }
    public bool Visibility { get; set; } = true;
    public bool DisableBtn { get; set; } = true;

    public List<string> ToolbarItems = new() { "ExcelExport", "ColumnChooser" };

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"SSC Report", Link = "hud/reports/ssc" },
            new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
        };
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
                }

                HttpClient = HttpClientFactory.CreateClient("RequestClient");
                HttpClient.BaseAddress = new Uri(NavMan.BaseUri);
                HeaderData.Add("User", Context.HttpContext.User.Identity.Name);
                HeaderData.Add("IsAuthenticated", Context.HttpContext.User.Identity.IsAuthenticated.ToString());
                HeaderData.Add("Claims", string.Join(", ", Context.HttpContext.User.Claims.Select(x => x.Value)));

                QueryData = new Syncfusion.Blazor.Data.Query().Sort("datecreated desc", "");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
            }
        }
    }

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == "SSCReportTable_excelexport") //Id is combination of Grid's ID and itemname
        {
            try
            {
                ExcelExportProperties ExportProperties = new();
                ExportProperties.FileName = $"SSC_Report{DateTimeOffset.UtcNow:ddMMyyyy.Hmmss}.xlsx";
                ExportProperties.IncludeHiddenColumn = false;

                await Grid_SSRequest.ExcelExport(ExportProperties);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

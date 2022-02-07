using System.Linq.Expressions;

namespace Project.V1.Web.Pages.Acceptance;

public partial class StaticReport
{
    public List<PathInfo> Paths { get; set; }
    List<StaticReportModel> Report { get; set; }
    public StaticReportModelDTO FilterObject { get; set; } = new();
    public string SEUploadIconCss { get; set; } = "fas fa-paper-plane ml-2";


    [Inject] protected NavigationManager NavMan { get; set; }
    [Inject] public ICLogger Logger { get; set; }
    [Inject] protected IUserAuthentication UserAuth { get; set; }
    [Inject] protected IStaticReport IStaticReport { get; set; }


    protected SfGrid<StaticReportModel> Grid_StaticReport { get; set; }
    public List<string> ToolbarItems = new() { "Search", "ExcelExport", "ColumnChooser" };

    private static readonly string[] Vendors = new string[]
    {
        "Ericsson", "HUAWEI", "ZTE", "AMN", "I.H.S", "Raeanna", "Huawei", "Hotspot", "Infratel"
    };

    public List<StaticDrp> StaticVendors { get; set; } = Vendors.Select(x => new StaticDrp { Name = x.ToUpper() }).ToList();

    private static readonly string[] Freqs = new string[]
    {
        "700Mhz", "800M", "1800Mhz", "2600Mhz"
    };

    public List<StaticDrp> StaticFreqs { get; set; } = Freqs.Select(x => new StaticDrp { Name = x.ToUpper() }).ToList();

    private static readonly string[] Technos = new string[]
    {
        "2G", "3G", "U900", "MS"
    };

    public List<StaticDrp> StaticTechs { get; set; } = Technos.Select(x => new StaticDrp { Name = x.ToUpper() }).ToList();

    private static readonly string[] Regions = new string[]
    {
        "LGS", "ABJ", "ASB", "IBD", "ENG", "KNO", "PHC"
    };

    public class StaticDrp
    {
        public string Name { get; set; }
    }

    public List<StaticDrp> StaticRegions { get; set; } = Regions.Select(x => new StaticDrp { Name = x.ToUpper() }).ToList();


    private static readonly string[] States = new string[]
    {
            "Abia", "Adamawa", "Akwa Ibom", "Anambra", "Bauchi", "Bayelsa", "Benue", "Borno", "Cross River", "Delta", "Ebonyi", "Edo", "Ekiti",
            "Enugu", "FCT - Abuja", "Gombe", "Imo", "Jigawa", "Kaduna", "Kano", "Katsina", "Kebbi", "Kogi", "Kwara","Lagos", "Nasarawa", "Niger",
            "Ogun", "Ondo", "Osun", "Oyo", "Plateau", "Rivers", "Sokoto", "Taraba", "Yobe", "Zamfara"
    };

    public List<StaticDrp> NigerianStates { get; set; } = States.Select(x => new StaticDrp { Name = x.ToUpper() }).ToList();

    public class StaticReportModelDTO
    {
        public string Technology { get; set; }

        public string Frequency { get; set; }

        public string SiteId { get; set; }

        public string RNC { get; set; }

        public string FinancialYear { get; set; }

        public string Region { get; set; }

        public string Vendor { get; set; }

        public DateTime DateUpgraded { get; set; }

        public DateTime DateIntegrated { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime? DateAccepted { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }

        public string Status { get; set; }

        public string Remark { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not StaticReportModelDTO other)
                return false;

            if (Technology != other.Technology || Frequency != other.Frequency || State != other.State
                || SiteId != other.SiteId || Region != other.Region || Vendor != other.Vendor || DateAccepted != other.DateAccepted)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    protected override void OnInitialized()
    {
        Paths = new()
        {
            new PathInfo { Name = $"Static Report", Link = "acceptance/reports/static" },
            new PathInfo { Name = $"Acceptance", Link = "acceptance" },
        };
    }

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == "ReportTable_excelexport") //Id is combination of Grid's ID and itemname
        {
            var hiddenCols = new string[] {
                    "RNC",
                    "FY",
                    "Remark",
                    "Date Upgraded",
                    "Date Integrated",
                };

            await Grid_StaticReport.ShowColumnsAsync(hiddenCols);
            await Grid_StaticReport.ExcelExport();
            await Grid_StaticReport.HideColumnsAsync(hiddenCols);
        }
    }
    protected async Task AuthenticationCheck(bool isAuthenticated)
    {
        if (isAuthenticated)
        {
            try
            {
                if (!await UserAuth.IsAutorizedForAsync("Can:ViewStaticReport"))
                {
                    NavMan.NavigateTo("access-denied");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading rejected requests", new { }, ex);
            }
        }
    }

    public async Task PerformFiltering(MouseEventArgs args)
    {
        try
        {
            SEUploadIconCss = "fas fa-spin fa-spinner ml-2";
            var filter = GetFilterExpression(FilterObject);

            if (filter != null)
                Report = await Task.Run(async () =>
                {
                    return (await IStaticReport.Get(filter, x => x.OrderByDescending(y => y.DateAccepted))).ToList();
                });

            SEUploadIconCss = "fas fa-paper-plane ml-2";
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message, new { }, ex);
        }
    }

    private static Expression<Func<StaticReportModel, bool>> GetFilterExpression(StaticReportModelDTO filterObject)
    {
        if (filterObject.Equals(new StaticReportModelDTO())) return null;


        Expression<Func<StaticReportModel, bool>> filter = x => x.SiteId != null;


        if (filterObject.Technology != null)
            filter = CombineFilters(filter, x => x.Technology.ToUpper() == filterObject.Technology.ToUpper());

        if (filterObject.Frequency != null)
            filter = CombineFilters(filter, x => x.Frequency.ToUpper() == filterObject.Frequency.ToUpper());

        if (filterObject.SiteId != null)
            filter = CombineFilters(filter, x => x.SiteId.ToUpper() == filterObject.SiteId.ToUpper());

        if (filterObject.Region != null)
            filter = CombineFilters(filter, x => x.Region.ToUpper() == filterObject.Region.ToUpper());

        if (filterObject.State != null)
            filter = CombineFilters(filter, x => x.State.ToUpper() == filterObject.State.ToUpper());

        if (filterObject.Vendor != null)
            filter = CombineFilters(filter, x => x.Vendor.ToUpper() == filterObject.Vendor.ToUpper());

        if (filterObject.DateAccepted.HasValue)
            filter = CombineFilters(filter, x => x.DateAccepted.Date == filterObject.DateAccepted.Value.Date);

        return filter;
    }

    private static Expression<Func<StaticReportModel, bool>> CombineFilters(
        Expression<Func<StaticReportModel, bool>> filter1,
        Expression<Func<StaticReportModel, bool>> filter2)
    {
        var rewrittenBody1 = new ExpressionReplaceVisitor(filter1.Parameters[0], filter2.Parameters[0]).Visit(filter1.Body);
        var newFilter = Expression.Lambda<Func<StaticReportModel, bool>>(
            Expression.AndAlso(rewrittenBody1, filter2.Body), filter2.Parameters);
        // newFilter is equivalent to: x => x.A > 1 && x.B > 2.5

        return newFilter;
    }
}

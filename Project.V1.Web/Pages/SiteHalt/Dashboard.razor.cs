using Syncfusion.Blazor.Charts;

namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Dashboard
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] public IHUDRequest IHUDRequest { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }


        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Halt | Unhalt | Decom", Link = "hud" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    var startDate = DateTime.Now.AddMonths(-11).FirstDay().Start();
                    var yearShim = 0;

                    for (int i = 0; i < 12; i++)
                    {
                        ChartDataDB.Add(new ChartData
                        {
                            Date = new DateTime(startDate.Year + yearShim, startDate.AddMonths(i).Month, 01).FirstDay().Start(),
                            HaltCount = 0,
                            UnHaltCount = 0,
                            DecomCount = 0
                        });

                        if (startDate.AddMonths(i).Month == 12)
                            yearShim++;
                    }

                    var chartDataDB = (await IHUDRequest.Get(x => x.DateCreated > startDate))
                        .GroupBy(x => x.DateCreated.FirstDay().Start())
                        .Select(x => new ChartData
                        {
                            Date = x.Key,
                            HaltCount = x.Count(y => y.RequestAction == "Halt"),
                            UnHaltCount = x.Count(y => y.RequestAction == "UnHalt"),
                            DecomCount = x.Count(y => y.RequestAction == "Decommission"),
                        }).ToList();

                    ChartDataDB.RemoveAll(x => chartDataDB.Any(y => y.Date == x.Date));
                    ChartDataDB.AddRange(chartDataDB);
                    ChartDataDB = ChartDataDB.OrderBy(x => x.Date).ToList();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }

        public class ChartData
        {
            public DateTime Date { get; set; }
            public double HaltCount { get; set; }
            public double UnHaltCount { get; set; }
            public double DecomCount { get; set; }
        }

        public bool Visibility { get; set; }
        public string ChartDataDate { get; set; }

        public List<string> ToolbarItems = new() { "Search" };
        protected SfGrid<SiteHUDRequestModel> Grid_DailyRequest { get; set; }

        public List<SiteHUDRequestModel> DailyChartPopUpData { get; set; }

        public List<ChartData> ChartDataDB { get; set; } = new();
        //{
        //    new ChartData { Date = new DateTime(2022, 03, 01), HaltCount = 100, UnHaltCount = 110, DecomCount = 10 },
        //    new ChartData { Date = new DateTime(2022, 04, 01), HaltCount = 200, UnHaltCount = 120, DecomCount = 20 },
        //    new ChartData { Date = new DateTime(2022, 05, 01), HaltCount = 300, UnHaltCount = 200, DecomCount = 40 },
        //    new ChartData { Date = new DateTime(2022, 06, 01), HaltCount = 400, UnHaltCount = 500, DecomCount = 35 },
        //    new ChartData { Date = new DateTime(2022, 07, 01), HaltCount = 500, UnHaltCount = 450, DecomCount = 33 },
        //    new ChartData { Date = new DateTime(2022, 08, 01), HaltCount = 600, UnHaltCount = 700, DecomCount = 21 },
        //    new ChartData { Date = new DateTime(2022, 09, 01), HaltCount = 700, UnHaltCount = 550, DecomCount = 100 },
        //    new ChartData { Date = new DateTime(2022, 10, 01), HaltCount = 800, UnHaltCount = 790, DecomCount = 120 },
        //    new ChartData { Date = new DateTime(2022, 11, 01), HaltCount = 900, UnHaltCount = 300, DecomCount = 40 },
        //    new ChartData { Date = new DateTime(2022, 12, 01), HaltCount = 1000, UnHaltCount = 720, DecomCount = 71 },
        //    new ChartData { Date = new DateTime(2023, 01, 01), HaltCount = 1100, UnHaltCount = 900, DecomCount = 80 },
        //    new ChartData { Date = new DateTime(2023, 02, 01), HaltCount = 125, UnHaltCount = 900, DecomCount = 34 },
        //    new ChartData { Date = new DateTime(2023, 03, 01), HaltCount = 324, UnHaltCount = 877, DecomCount = 65 },
        //    new ChartData { Date = new DateTime(2023, 04, 01), HaltCount = 700, UnHaltCount = 900, DecomCount = 55 },
        //    new ChartData { Date = new DateTime(2023, 05, 01), HaltCount = 550, UnHaltCount = 344, DecomCount = 0 },
        //    new ChartData { Date = new DateTime(2023, 06, 01), HaltCount = 50, UnHaltCount = 69, DecomCount = 80 },
        //};

        public void AxisLabelClickEvent(AxisLabelClickEventArgs args)
        {
            DailyChartPopUpData = new();
            DailyChartPopUpData.Add(new() { Status = "Pending" });
            ChartDataDate = args.Text;

            Visibility = true;
            StateHasChanged();
        }

        private void DialogClosed(CloseEventArgs args)
        {
            Visibility = false;
            DailyChartPopUpData = new();
        }
    }
}

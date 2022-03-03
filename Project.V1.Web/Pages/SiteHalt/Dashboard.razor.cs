namespace Project.V1.Web.Pages.SiteHalt
{
    public partial class Dashboard
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Site Halt & Unhalt", Link = "halt" },
            };
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }

        public class ChartData
        {
            public string X { get; set; }
            public double Y { get; set; }
        }

        public List<ChartData> MedalDetails = new()
        {
            new ChartData { X = "South Korea", Y = 39.4 },
            new ChartData { X = "India", Y = 61.3 },
            new ChartData { X = "Pakistan", Y = 20.4 },
            new ChartData { X = "Germany", Y = 65.1 },
            new ChartData { X = "Australia", Y = 15.8 },
            new ChartData { X = "Italy", Y = 29.2 },
            new ChartData { X = "United Kingdom", Y = 44.6 },
            new ChartData { X = "Saudi Arabia", Y = 9.7 },
            new ChartData { X = "Russia", Y = 40.8 },
            new ChartData { X = "Mexico", Y = 31 },
            new ChartData { X = "Brazil", Y = 75.9 },
            new ChartData { X = "China", Y = 51.4 }
        };
    }
}

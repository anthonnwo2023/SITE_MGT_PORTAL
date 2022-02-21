namespace Project.V1.Web.Pages.Acceptance
{
    public partial class Dashboard
    {
        public List<PathInfo> Paths { get; set; }
        [Inject] public IHttpContextAccessor Context { get; set; }
        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IRequest IRequest { get; set; }
        [Inject] protected IVendor IVendor { get; set; }
        [Inject] protected IProjectType IProjectType { get; set; }

        public List<RequestViewModel> VendorRequests { get; set; }
        public List<AcceptanceDTO> DailyRequests { get; set; }
        public List<AcceptanceDTO> MonthlyProjectTypeRequests { get; set; }
        public List<AcceptanceDTO> MonthlyRequests { get; set; }

        public DateTime DateData { get; set; } = DateTime.Now;
        public DateTime PrevDate { get; set; } = DateTime.MinValue;
        public DateTime MinDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
        public DateTime MaxDateTime { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 0, 0, 0).AddDays(1);
        public bool DateIsToday { get; set; } = true;
        public bool DateWthMth { get; set; } = true;

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public SfPivotView<AcceptanceDTO> pivot;

        public List<ToolbarItems> toolbar = new()
        {
            ToolbarItems.New,
            ToolbarItems.Load,
            ToolbarItems.Remove,
            ToolbarItems.Rename,
            ToolbarItems.SaveAs,
            ToolbarItems.Save,
            ToolbarItems.Grid,
            ToolbarItems.Chart,
            ToolbarItems.Export,
            ToolbarItems.SubTotal,
            ToolbarItems.GrandTotal,
            ToolbarItems.ConditionalFormatting,
            ToolbarItems.FieldList
        };

        protected override void OnInitialized()
        {
            Paths = new()
            {
                new PathInfo { Name = $"Acceptance", Link = "acceptance" },
            };
        }

        protected async Task SetMyDate(ChangedEventArgs<DateTime> value)
        {
            DateData = value.Value;
            DateIsToday = DateData.Date == DateTime.Now.Date;
            DateWthMth = DateData.Date >= MinDateTime && DateData.Date < MaxDateTime;

            //RequestSummary.Initialize(IProjectType, IVendor, IRequest);
            DailyRequests = RequestSummary.GetVendorRequests(DateData.Date, DateData.AddDays(1).Date);

            int lastDayOfMth = DateTime.DaysInMonth(DateData.Year, DateData.Month);

            MinDateTime = new DateTime(DateData.Year, DateData.Month, 1).Date;
            MaxDateTime = new DateTime(DateData.Year, DateData.Month, lastDayOfMth).AddDays(1).Date;

            var shouldNotReload = PrevDate.Date >= MinDateTime && PrevDate.Date < MaxDateTime;

            if (!shouldNotReload)
            {
                MonthlyRequests = RequestSummary.GetVendorRequests(MinDateTime, MaxDateTime);
                MonthlyProjectTypeRequests = RequestSummary.GetProjectTypeRequests(MinDateTime, MaxDateTime);

                PrevDate = DateData;
            }

            await Task.CompletedTask;
        }

        protected async Task AuthenticationCheck(bool isAuthenticated)
        {
            if (isAuthenticated)
            {
                try
                {
                    RequestSummary.Initialize(IProjectType, IVendor, IRequest);

                    MonthlyRequests = RequestSummary.GetVendorRequests(MinDateTime, MaxDateTime);
                    DailyRequests = RequestSummary.GetVendorRequests(DateData.Date, DateData.AddDays(1).Date);
                    MonthlyProjectTypeRequests = RequestSummary.GetProjectTypeRequests(MinDateTime, MaxDateTime);

                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error loading rejected requests", new { }, ex);
                }
            }
        }
    }
}

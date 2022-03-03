namespace Project.V1.Web.Pages.SiteHalt.Shared
{
    public partial class HaltRequestControl
    {
        [Parameter] public bool ShouldEnable { get; set; }
        [Parameter] public bool ShowRequired { get; set; }
        [Parameter] public bool ShowSSVUpload { get; set; } = true;
        [Parameter] public SiteHaltRequestModel RequestModel { get; set; }
        [Parameter] public EventCallback<bool> CheckValid { get; set; }
        [Parameter] public List<FilesManager> UploadedRequestFiles { get; set; }
        [Parameter] public EventCallback<bool> OnCheckValidButton { get; set; }
        [Parameter] public EventCallback<ClearingEventArgs> OnClear { get; set; }
        [Parameter] public EventCallback<RemovingEventArgs> OnRemove { get; set; }
        [Parameter] public EventCallback<UploadChangeEventArgs> OnFileSSVUploadChange { get; set; }


        [Inject] public ICLogger Logger { get; set; }
        [Inject] protected IUser IUser { get; set; }


        public string ShowRqdClass { get; set; } = "inline-block";
        public ClaimsPrincipal Principal { get; set; }
        public ApplicationUser User { get; set; }
        public List<ChipItem> SiteChips { get; set; } = new();
        public List<string> CurrentChips = new();

        public List<ApplicationUser> FirstLevelApprovers { get; set; } = new();
        public List<ApplicationUser> SecondLevelApprovers { get; set; } = new();
        public List<ApplicationUser> ThirdLevelApprovers { get; set; } = new();


        protected SfUploader SF_Uploader { get; set; }
        protected SfChip SiteIDChips { get; set; }

        public double MaxFileSize { get; set; } = 25000000;

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        private void OnDelete(ChipDeletedEventArgs args)
        {
            if (args.Text == null) return;

            if (CurrentChips.Contains(args.Text))
                CurrentChips.Remove(args.Text);

            RequestModel.SiteIds = string.Join(", ", CurrentChips);

            StateHasChanged();
        }

        public void CreateChips(string chipStr)
        {
            if (string.IsNullOrWhiteSpace(chipStr))
            {
                CurrentChips = new();
                var chips = SiteIDChips.GetSelectedChips();
                var values = chips.Select(x => x.Value).ToArray();
                SiteIDChips.RemoveChips(values);

                return;
            }

            var chipStrChunk = chipStr?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var exceptChips = CurrentChips.Except(chipStrChunk).ToList();
            if (exceptChips.Any())
            {
                CurrentChips.RemoveAll(x => exceptChips.Contains(x));
                SiteIDChips.RemoveChips(exceptChips.ToArray());
            }

            if (chipStrChunk != null)
                foreach (var chip in chipStrChunk)
                {
                    if (CurrentChips.Contains(chip) == false)
                    {
                        SiteIDChips.AddChip(new()
                        {
                            Text = chip,
                            Value = chip,
                        });

                        CurrentChips.Add(chip);
                    }
                }

            RequestModel.SiteIds = string.Join(", ", CurrentChips);

            StateHasChanged();
        }

        private void BlurHandler(FocusOutEventArgs args)
        {
            CreateChips(RequestModel.SiteIds);
        }

        public List<ChipItem> GetChips()
        {
            return SiteChips;
        }

        protected Dictionary<string, object> DescriptionHtmlAttribute { get; set; } = new Dictionary<string, object>()
        {
            { "rows", "3" },
        };

        private async Task InitializeForm()
        {
            Principal = (await AuthenticationStateTask).User;
        }

        protected override async Task OnInitializedAsync()
        {
            await InitializeForm();
        }
    }
}

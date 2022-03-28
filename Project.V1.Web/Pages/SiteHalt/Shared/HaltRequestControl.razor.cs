using Microsoft.JSInterop;
using Project.V1.Web.Request;

namespace Project.V1.Web.Pages.SiteHalt.Shared;

public partial class HaltRequestControl
{
    [Parameter] public bool ShouldEnable { get; set; }
    [Parameter] public bool ShowCopyText { get; set; } = true;
    [Parameter] public bool ShowRequired { get; set; }
    [Parameter] public bool ShowSSVUpload { get; set; } = true;
    [Parameter] public SiteHUDRequestModel RequestModel { get; set; }
    [Parameter] public EventCallback<SiteHUDRequestModel> OnRequestTypeChange { get; set; }
    [Parameter] public List<FilesManager> UploadedRequestFiles { get; set; }
    [Parameter] public EventCallback<bool> OnCheckValidButton { get; set; }
    [Parameter] public EventCallback<ClearingEventArgs> OnClear { get; set; }
    [Parameter] public EventCallback<RemovingEventArgs> OnRemove { get; set; }
    [Parameter] public EventCallback<UploadChangeEventArgs> OnFileSSVUploadChange { get; set; }
    [Parameter] public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    [Parameter] public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    [Parameter] public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();


    [Inject] protected ICLogger Logger { get; set; }
    [Inject] protected IHUDRequest IHUDRequest { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] protected IRequestListObject IRequestList { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
    [Inject] protected IJSRuntime JSRuntime { get; set; }


    public string ShowRqdClass { get; set; } = "inline-block";
    public ClaimsPrincipal Principal { get; set; }
    public ApplicationUser User { get; set; }
    public List<ChipItem> SiteChips { get; set; } = new();
    public List<string> CurrentChips = new();

    public List<RequestApproverModel> FirstLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> SecondLevelApprovers { get; set; } = new();
    public List<RequestApproverModel> ThirdLevelApprovers { get; set; } = new();


    protected SfUploader SF_Uploader { get; set; }
    protected SfChip SiteIDChips { get; set; } = new();

    public double MaxFileSize { get; set; } = 25000000;

    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    //private readonly Dictionary<string, object> htmlattribute = new()
    //{
    //    { "padding", "5px 2px 5px 2px" },
    //};
    private async Task CopyTextToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("copyToClipboard", RequestModel.SiteIds);
    }

    private void OnDelete(ChipDeletedEventArgs args)
    {
        //await OnCheckValidButton.InvokeAsync(false);

        if (args.Text == null) return;

        if (CurrentChips.Contains(args.Text))
            CurrentChips.Remove(args.Text);

        RequestModel.SiteIds = string.Join(", ", CurrentChips);

        //await OnCheckValidButton.InvokeAsync(true);

        StateHasChanged();
    }

    public void CreateChips(string chipStr)
    {
        if (string.IsNullOrWhiteSpace(chipStr))
        {
            CurrentChips = new();
            var values = SiteIDChips.Chips?.Select(x => x.Value).ToArray();
            SiteIDChips.RemoveChips(values);

            return;
        }

        var chipStrChunk = chipStr?.Split(new[] { ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
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

        RequestModel.SiteIds = (ShouldEnable) ? string.Join(", ", CurrentChips) : RequestModel.SiteIds;

        StateHasChanged();
    }

    private void BlurHandler(FocusOutEventArgs args)
    {
        CreateChips(RequestModel.GetSiteIds);
    }

    private async void SetRequestType(MouseEventArgs args, string value)
    {
        RequestModel.RequestAction = string.Empty;
        RequestModel.ShouldRequireApprovers = !string.IsNullOrWhiteSpace(RequestModel.SiteIds);

        if (!string.IsNullOrWhiteSpace(value))
        {
            RequestModel.ShouldRequireApprovers = !value.Equals("UnHalt");

            RequestModel.RequestAction = value;

            await OnRequestTypeChange.InvokeAsync(RequestModel);

            CreateChips(RequestModel.GetSiteIds);
        }

        StateHasChanged();
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

        //List<RequestApproverModel> SecondApprover = new();
        //List<RequestApproverModel> ThirdApprover = new();
        //if (!string.IsNullOrWhiteSpace(RequestModel.SecondApproverId))
        //    SecondApprover.Add(BaseSecondLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.SecondApproverId));
        //if (!string.IsNullOrWhiteSpace(RequestModel.ThirdApproverId))
        //    ThirdApprover.Add(BaseThirdLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.ThirdApproverId));

        //FirstLevelApprovers = BaseFirstLevelApprovers;
        FirstLevelApprovers = BaseFirstLevelApprovers.ToList();
        SecondLevelApprovers = BaseSecondLevelApprovers.ToList();
        ThirdLevelApprovers = BaseThirdLevelApprovers.ToList();

        //if (RequestModel.SiteIds != null)
        //    RequestModel.SiteIds = (RequestModel.SiteIds.Contains(".txt"))
        //        ? TextFileExtension.Initialize("HUD_SiteID", RequestModel.SiteIds).ReadFromFile() : RequestModel.SiteIds;

        if (RequestModel.ThirdApprover != null)
            SecondLevelApprovers.Remove(SecondLevelApprovers.FirstOrDefault(x => x.Username == RequestModel.ThirdApprover.Username));

        if (RequestModel.SecondApprover != null)
            ThirdLevelApprovers.Remove(ThirdLevelApprovers.FirstOrDefault(x => x.Username == RequestModel.SecondApprover.Username));

        await IRequestList.Initialize(Principal, "HUDObject");
    }

    protected override async Task OnInitializedAsync()
    {
        await InitializeForm();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!string.IsNullOrWhiteSpace(RequestModel.GetSiteIds))
                CreateChips(RequestModel.GetSiteIds);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void ChangeNonRFSMApprover(ChangeEventArgs<string, RequestApproverModel> args, string approver)
    {
        if (!ShouldEnable) return;

        SecondLevelApprovers = BaseSecondLevelApprovers.ToList();
        ThirdLevelApprovers = BaseThirdLevelApprovers.ToList();

        if (approver.Equals("approver1"))
        {
            RequestModel.FirstApprover = args.ItemData;
        }

        if (approver.Equals("approver2"))
        {
            var approver3 = (!string.IsNullOrWhiteSpace(RequestModel.ThirdApproverId))
                ? BaseThirdLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.ThirdApproverId)
                : new RequestApproverModel();
            ThirdLevelApprovers.Remove(args.ItemData);
            SecondLevelApprovers.Remove(approver3);

            RequestModel.SecondApprover = args.ItemData;
        }
        if (approver.Equals("approver3"))
        {
            var approver2 = (!string.IsNullOrWhiteSpace(RequestModel.SecondApproverId))
                ? BaseSecondLevelApprovers.FirstOrDefault(x => x.Id == RequestModel.SecondApproverId)
                : new RequestApproverModel();
            SecondLevelApprovers.Remove(args.ItemData);
            ThirdLevelApprovers.Remove(approver2);

            RequestModel.ThirdApprover = args.ItemData;
        }

        StateHasChanged();
    }

    private void TechSelectHandler(MultiSelectChangeEventArgs<string[]> args)
    {
        RequestModel.TechTypeIds = args?.Value;
    }
}

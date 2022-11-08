using Microsoft.JSInterop;
using Org.BouncyCastle.Asn1.Ocsp;
using Project.V1.Web.Requests;

namespace Project.V1.Web.Pages.SiteHalt.Shared;

public partial class HaltRequestControl
{
    [Parameter] public bool ShouldEnable { get; set; }
    [Parameter] public bool ShowStatus { get; set; }
    [Parameter] public bool ShowCopyText { get; set; } = true;
    [Parameter] public bool ShowRequired { get; set; }
    [Parameter] public bool ShowSSVUpload { get; set; } = true;
    [Parameter] public SiteHUDRequestModel HUDRequestModel { get; set; }
    [Parameter] public EventCallback<SiteHUDRequestModel> OnRequestTypeChange { get; set; }
    [Parameter] public List<FilesManager> UploadedRequestFiles { get; set; }
    [Parameter] public EventCallback<bool> OnCheckValidButton { get; set; }
    [Parameter] public EventCallback<ClearingEventArgs> OnClear { get; set; }
    [Parameter] public EventCallback<RemovingEventArgs> OnRemove { get; set; }
    [Parameter] public EventCallback<Syncfusion.Blazor.Inputs.UploadChangeEventArgs> OnFileSSVUploadChange { get; set; }
    [Parameter] public List<RequestApproverModel> BaseFirstLevelApprovers { get; set; } = new();
    [Parameter] public List<RequestApproverModel> BaseSecondLevelApprovers { get; set; } = new();
    [Parameter] public List<RequestApproverModel> BaseThirdLevelApprovers { get; set; } = new();


    [Inject] protected ICLogger Logger { get; set; }
    [Inject] protected IHUDRequest IHUDRequest { get; set; }
    [Inject] protected IUser IUser { get; set; }
    [Inject] protected IRequestListObject IRequestList { get; set; }
    [Inject] protected UserManager<ApplicationUser> UserManager { get; set; }
    [Inject] protected IJSRuntime JSRuntime { get; set; }

    public string ThirdApprovalDisplay { get; set; }
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
        await JSRuntime.InvokeVoidAsync("copyToClipboard", HUDRequestModel.SiteIds);
    }

    private void OnDelete(ChipDeletedEventArgs args)
    {
        //await OnCheckValidButton.InvokeAsync(false);

        if (args.Text == null) return;

        if (CurrentChips.Contains(args.Text))
            CurrentChips.Remove(args.Text);

        HUDRequestModel.SiteIds = string.Join(", ", CurrentChips);

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
                        Text = chip.ToUpper(),
                        Value = chip.ToUpper(),
                    });

                    CurrentChips.Add(chip.ToUpper());
                }
            }

        HUDRequestModel.SiteIds = (ShouldEnable) ? string.Join(", ", CurrentChips) : HUDRequestModel.SiteIds;

        StateHasChanged();
    }

    private void BlurHandler(FocusOutEventArgs args)
    {
        CreateChips(HUDRequestModel.GetSiteIds);
    }

    private async void SetRequestType(MouseEventArgs args, string value)
    {
        HUDRequestModel.RequestAction = string.Empty;
        HUDRequestModel.ShouldRequireApprovers = !string.IsNullOrWhiteSpace(HUDRequestModel.SiteIds);

        if (!string.IsNullOrWhiteSpace(value))
        {
            HUDRequestModel.ShouldRequireApprovers = !value.Equals("UnHalt");

            HUDRequestModel.RequestAction = value;
            HUDRequestModel.IsForceMajeure = false;

            await OnRequestTypeChange.InvokeAsync(HUDRequestModel);

            CreateChips(HUDRequestModel.GetSiteIds);
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

        if (HUDRequestModel.ThirdApprover != null)
            SecondLevelApprovers.Remove(SecondLevelApprovers.FirstOrDefault(x => x.Username == HUDRequestModel.ThirdApprover.Username));

        if (HUDRequestModel.SecondApprover != null)
            ThirdLevelApprovers.Remove(ThirdLevelApprovers.FirstOrDefault(x => x.Username == HUDRequestModel.SecondApprover.Username));

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
            if (!string.IsNullOrWhiteSpace(HUDRequestModel.GetSiteIds))
                CreateChips(HUDRequestModel.GetSiteIds);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    public void ChangeNonRFSMApprover(ChangeEventArgs<string, RequestApproverModel> args, string approver)
    {
        if (!ShouldEnable) return;

        FirstLevelApprovers = BaseFirstLevelApprovers.ToList();
        SecondLevelApprovers = BaseSecondLevelApprovers.ToList();
        ThirdLevelApprovers = BaseThirdLevelApprovers.ToList();

        if (approver.Equals("approver1"))
        {
            var approver2 = (!string.IsNullOrWhiteSpace(HUDRequestModel.SecondApproverId))
                ? BaseSecondLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.SecondApproverId)
                : new RequestApproverModel();

            var approver3 = (!string.IsNullOrWhiteSpace(HUDRequestModel.ThirdApproverId))
                ? BaseThirdLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.ThirdApproverId)
                : new RequestApproverModel();

            SecondLevelApprovers.Remove(args.ItemData);
            SecondLevelApprovers.Remove(approver3);

            ThirdLevelApprovers.Remove(args.ItemData);
            ThirdLevelApprovers.Remove(approver2);

            FirstLevelApprovers.Remove(approver3);
            FirstLevelApprovers.Remove(approver2);

            HUDRequestModel.FirstApprover = args.ItemData;
        }

        if (approver.Equals("approver2"))
        {
            var approver3 = (!string.IsNullOrWhiteSpace(HUDRequestModel.ThirdApproverId))
                ? BaseThirdLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.ThirdApproverId)
                : new RequestApproverModel();

            var approver1 = (!string.IsNullOrWhiteSpace(HUDRequestModel.FirstApproverId))
                ? BaseFirstLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.FirstApproverId)
                : new RequestApproverModel();

            FirstLevelApprovers.Remove(args.ItemData);
            FirstLevelApprovers.Remove(approver3);

            ThirdLevelApprovers.Remove(args.ItemData);
            ThirdLevelApprovers.Remove(approver1);

            SecondLevelApprovers.Remove(approver3);
            SecondLevelApprovers.Remove(approver1);

            HUDRequestModel.SecondApprover = args.ItemData;
        }

        if (approver.Equals("approver3"))
        {
            var approver1 = (!string.IsNullOrWhiteSpace(HUDRequestModel.FirstApproverId))
                ? BaseFirstLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.FirstApproverId)
                : new RequestApproverModel();

            var approver2 = (!string.IsNullOrWhiteSpace(HUDRequestModel.SecondApproverId))
                ? BaseSecondLevelApprovers.FirstOrDefault(x => x.Id == HUDRequestModel.SecondApproverId)
                : new RequestApproverModel();

            FirstLevelApprovers.Remove(approver2);
            FirstLevelApprovers.Remove(args.ItemData);

            SecondLevelApprovers.Remove(approver1);
            SecondLevelApprovers.Remove(args.ItemData);

            ThirdLevelApprovers.Remove(approver1);
            ThirdLevelApprovers.Remove(approver2);

            HUDRequestModel.ThirdApprover = args.ItemData;
        }

        StateHasChanged();
    }

    private void TechSelectHandler(MultiSelectChangeEventArgs<string[]> args)
    {
        HUDRequestModel.TechTypeIds = args?.Value;
    }

    private async void onChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
    {
        if ((bool)(args.Value))
        {
            ThirdApprovalDisplay = "hide-obj";          
        }
        else
        {
            ThirdApprovalDisplay = "show-obj";
        }

        string ChkValue = args.Value.ToString();

        await Task.CompletedTask;
    }

}

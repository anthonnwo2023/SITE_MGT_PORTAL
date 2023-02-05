namespace Project.V1.Models.SiteHalt;


//[Table("TBL_RFHUD_REQUESTS_TECH")]
//public class SiteHUDRequestModelTechTypeModel
//{
//    public string HUDRequestsId { get; set; }
//    public string TechTypesId { get; set; }
//}


[Table("TBL_RFHUD_REQUESTS")]
public class SiteHUDRequestModel : IDisposable
{
    [Key]
    public string Id { get; set; }

    public string UniqueId { get; set; }

    [Required]
    //[Column(TypeName = "blob")]
    public string RequestAction { get; set; }

    public string RequestType { get; set; } = "H|U|D";

    public string GetSiteIds
    {
        get
        {
            if (SiteIds == null)
            {
                return null;
            }

            return (SiteIds.Contains(".txt"))
               ? TextFileExtension.Initialize("HUD_SiteID", SiteIds).ReadFromFile() : SiteIds;
        }
    }

    [Required]
    public string SiteIds { get; set; }

    [Required]
    public string Reason { get; set; }

    public string SupportingDocument { get; set; }

    public string Status { get; set; } = "";

    public string CompletedBy { get; set; }

    public bool ShouldRequireApprovers { get; set; }

    public bool HasLargeSiteIdCount { get; set; }

    public bool IsForceMajeure { get; set; }

    public bool IsNotForceMajureAndRequired => !IsForceMajeure && ShouldRequireApprovers;

    [Required(ErrorMessage = "The Technology field is required.")]
    [NotMapped]
    public string[] TechTypeIds { get; set; }

    public List<TechTypeModel> TechTypes { get; set; }

    public string RequesterId { get; set; }

    [ForeignKey(nameof(RequesterId))]
    public virtual RequesterData Requester { get; set; }

    [NotMapped]
    public ApplicationUser User { get; set; }

    [NotMapped]
    [RequiredWhen(nameof(TempStatus), "Disapproved", AllowEmptyStrings = false, ErrorMessage = "The Comment is required.")]
    public string TempComment { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "The status is required.")]
    public string TempStatus { get; set; }

    public string GetStatusString => Status switch
    {
        var status when status.EndsWith("Approved") => GetApproveMessage(true),
        var status when status.EndsWith("Disapproved") => GetApproveMessage(false),
        _ => Status
    };

    public string GetApproveMessage(bool check)
    {
        var message = check ? "Approved" : "Disapproved";
        var extra = string.Empty;
        var approverCount = (ThirdApprover == null) ? 2 : 3;

        if (Status.StartsWith("FA"))
        {
            extra = $" By {FirstApprover?.Fullname} (1 of {approverCount})";
        }

        if (Status.StartsWith("SA"))
        {
            extra = $" By {SecondApprover?.Fullname} (2 of {approverCount})";
        }

        if (Status.StartsWith("TA") && RequestAction != "UnHalt")
        {
            extra = $" By {ThirdApprover?.Fullname} (3 of {approverCount})";
        }

        return message + extra;
    }

    public string GetApproverComment => GetCommentForApprover();

    public string GetCommentForApprover()
    {
        var comment = string.Empty;

        if (FirstApprover?.IsActioned == true)
        {
            comment = FirstApprover?.ApproverComment;
        }

        if (SecondApprover?.IsActioned == true)
        {
            comment = SecondApprover?.ApproverComment;
        }

        if (ThirdApprover?.IsActioned == true)
        {
            comment = ThirdApprover?.ApproverComment;
        }

        return comment;
    }

    public string FirstApproverName
    {
        get
        {
            if (FirstApprover == null)
            {
                return null;
            }

            return FirstApprover?.Fullname;
        }
    }

    public string SecondApproverName
    {
        get
        {
            if (SecondApprover == null)
            {
                return null;
            }

            return SecondApprover?.Fullname;
        }
    }

    public string ThirdApproverName
    {
        get
        {
            if (ThirdApprover == null)
            {
                return null;
            }

            return ThirdApprover?.Fullname;
        }
    }

    [RequiredWhen(nameof(ShouldRequireApprovers), true, AllowEmptyStrings = false, ErrorMessage = "The Approver is required.")]
    public string FirstApproverId { get; set; }

    [ForeignKey(nameof(FirstApproverId))]
    public virtual RequestApproverModel FirstApprover { get; set; }

    [RequiredWhen(nameof(ShouldRequireApprovers), true, AllowEmptyStrings = false, ErrorMessage = "The Approver is required.")]
    public string SecondApproverId { get; set; }

    [ForeignKey(nameof(SecondApproverId))]
    public virtual RequestApproverModel SecondApprover { get; set; }

    [RequiredWhen(nameof(IsNotForceMajureAndRequired), true, AllowEmptyStrings = false, ErrorMessage = "The Approver is required.")]
    public string ThirdApproverId { get; set; }

    [ForeignKey(nameof(ThirdApproverId))]
    public virtual RequestApproverModel ThirdApprover { get; set; }

    public DateTime DateCreated { get; set; }

    public string RequesterName => Requester?.Name;

    public virtual Task<(bool, string)> Create()
    {
        Id = Guid.NewGuid().ToString();
        Requester = GenerateRequestData(User);
        RequesterId = Requester.Id;
        DateCreated = DateTime.Now;
        Status = "Pending";

        return Task.Run(() => (false, $"Request could not be created. Not Implemented"));
    }

    public virtual Task SetCreateState(RequestApproverModel ActionedBy)
    {
        return Task.Run(() => "");
    }

    protected static RequesterData GenerateRequestData(ApplicationUser User)
    {
        return new RequesterData
        {
            Id = Guid.NewGuid().ToString(),
            Name = User?.Fullname,
            Department = User?.Department,
            Email = User?.Email,
            Phone = User?.PhoneNumber,
            Title = User?.JobTitle,
            Username = User?.UserName,
            VendorId = User?.VendorId
        };
    }

    public string Navigations
    {
        get
        {
            return "Requester,FirstApprover,SecondApprover,ThirdApprover";
        }
    }

    public Dictionary<string, object> Variables => new() { { "User", User?.UserName }, { "App", "hud" } };

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

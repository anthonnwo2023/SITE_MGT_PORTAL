namespace Project.V1.Models.SiteHalt;


public class SiteHUDRequestModelDTO
{
    public string Id { get; set; }

    public string UniqueId { get; set; }

    public string RequestAction { get; set; }

    public string RequestType { get; set; }

    public string SiteIds { get; set; }

    public string Reason { get; set; }

    public string SupportingDocument { get; set; }

    public string Status { get; set; }

    public string CompletedBy { get; set; }

    public bool ShouldRequireApprovers { get; set; }

    public bool HasLargeSiteIdCount { get; set; }

    public bool IsForceMajeure { get; set; }

    public List<string> TechTypes { get; set; }

    public string RequesterName { get; set; }

    public string UserName { get; set; }

    public string FirstApproverName { get; set; }

    public string FirstApproverComment { get; set; }

    public string SecondApproverName { get; set; }

    public string SecondApproverComment { get; set; }

    public string ThirdApproverName { get; set; }

    public string ThirdApproverComment { get; set; }

    public DateTime DateCreated { get; set; }
}

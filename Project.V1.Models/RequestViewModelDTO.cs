namespace Project.V1.Models;

public class RequestViewModelDTO
{
    [Key]
    public string Id { get; set; }

    public string UniqueId { get; set; }

    public string SiteId { get; set; }

    public string RNCBSC { get; set; }

    public string SiteName { get; set; }

    public string State { get; set; }

    public string Region { get; set; }

    public string Bandwidth { get; set; }

    public string Spectrum { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string AntennaMake { get; set; }

    public string AntennaType { get; set; }

    public string AntennaHeight { get; set; }

    public double TowerHeight { get; set; }

    public string AntennaAzimuth { get; set; }

    public string MTilt { get; set; }

    public string ETilt { get; set; }

    public string Baseband { get; set; }

    public string RRUType { get; set; }

    public string ProjectName { get; set; }

    public string Power { get; set; }

    public string RRUPower { get; set; }

    public string CSFBStatusGSM { get; set; }

    public string CSFBStatusWCDMA { get; set; }

    public string TechType { get; set; }

    public string ProjectType { get; set; }

    public double ProjectYear { get; set; }

    public string Status { get; set; }

    public string SSVReport { get; set; }

    public bool SSVReportIsWaiver { get; set; }

    public string BulkBatchNumber { get; set; }

    public string BulkuploadPath { get; set; }

    public string EngineerRejectReport { get; set; }

    public string RequesterComment { get; set; }

    public string SummerConfig { get; set; }

    public string SoftwareVersion { get; set; }

    public string RequestType { get; set; }

    public DateTime IntegratedDate { get; set; }

    public DateTime DateSubmitted { get; set; }

    public DateTime? DateUserActioned { get; set; }

    public DateTime DateCreated { get; set; }

    public string RequesterName { get; set; }

    public string EngineerAssigned { get; set; }

    public bool EngineerAssignedIsApproved { get; set; }

    public DateTime EngineerAssignedDateApproved { get; set; }

    public DateTime EngineerAssignedDateActioned { get; set; }

    public string RETConfigured { get; set; }

    public string CarrierAggregation { get; set; }

    public DateTime? DateActioned { get; set; }

    public string VendorName { get; set; }

    public string EngineerComment { get; set; }
}

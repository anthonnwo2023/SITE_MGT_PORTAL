namespace Project.V1.Models;

[Table("TBL_RFACCEPT_STATICREPORT")]
public class StaticReportModel
{
    [Key]
    public string Id { get; set; }

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

    public DateTime DateAccepted { get; set; }

    public string Scope { get; set; }

    public string State { get; set; }

    public string Status { get; set; }

    public string Remark { get; set; }
}


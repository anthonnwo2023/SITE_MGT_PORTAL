using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_REQUESTS")]
    [Index(new string[] { nameof(SiteId), nameof(SpectrumId) }, IsUnique = true)]
    public class RequestViewModel : IDisposable
    {
        [Key]
        public string Id { get; set; }

        public string UniqueId { get; set; }

        [Required]
        [ExcelColumnName("Site Id")]
        public string SiteId { get; set; }

        [ExcelColumnName("RNC/BSC")]
        public string RNCBSC { get; set; }

        [Required]
        [ExcelColumnName("Site Name")]
        public string SiteName { get; set; }

        [Required]
        [ExcelColumnName("State")]
        public string State { get; set; }

        [Required]
        [ExcelColumnName("Region")]
        public string RegionId { get; set; }

        [ForeignKey(nameof(RegionId))]
        public virtual RegionViewModel Region { get; set; }

        [ExcelColumnName("Bandwidth (MHz)")]
        public string Bandwidth { get; set; }

        [Required]
        [ExcelColumnName("Spectrum")]
        public string SpectrumId { get; set; }

        [ForeignKey(nameof(SpectrumId))]
        public virtual SpectrumViewModel Spectrum { get; set; }

        [ExcelColumnName("Latitude")]
        public double Latitude { get; set; }

        [ExcelColumnName("Longitude")]
        public double Longitude { get; set; }

        //[Required]
        [ExcelColumnName("Antenna Make")]
        public string AntennaMakeId { get; set; }

        [ForeignKey(nameof(AntennaMakeId))]
        public virtual AntennaMakeModel AntennaMake { get; set; }

        //[Required]
        [ExcelColumnName("Antenna Type")]
        public string AntennaTypeId { get; set; }

        [ForeignKey(nameof(AntennaTypeId))]
        public virtual AntennaTypeModel AntennaType { get; set; }

        [ExcelColumnName("Antenna Height")]
        public string AntennaHeight { get; set; }

        [ExcelColumnName("Tower Height - (M)")]
        public double TowerHeight { get; set; }

        [ExcelColumnName("Antenna Azimuth")]
        public string AntennaAzimuth { get; set; }

        [ExcelColumnName("M Tilt")]
        public string MTilt { get; set; }

        [ExcelColumnName("E Tilt")]
        public string ETilt { get; set; }

        //[Required]
        [ExcelColumnName("Baseband")]
        public string Baseband { get; set; }

        //[ForeignKey(nameof(BasebandId))]
        //public virtual BaseBandModel Baseband { get; set; }

        [Required]
        [ExcelColumnName("RRU TYPE")]
        public string RRUType { get; set; }

        //[ForeignKey(nameof(RRUTypeId))]
        //public virtual RRUTypeModel RRUType { get; set; }

        [ExcelColumnName("Power - (w)")]
        public string Power { get; set; }

        [ExcelColumnName("RRU Power - (w)")]
        public string RRUPower { get; set; }

        [ExcelColumnName("CSFB Status GSM")]
        public string CSFBStatusGSM { get; set; }

        [ExcelColumnName("CSFB Status WCDMA")]
        public string CSFBStatusWCDMA { get; set; }

        [Required]
        [ExcelColumnName("Technology")]
        public string TechTypeId { get; set; }

        [ForeignKey(nameof(TechTypeId))]
        public virtual TechTypeModel TechType { get; set; }

        [Required]
        [ExcelColumnName("Project Type")]
        public string ProjectTypeId { get; set; }

        [ForeignKey(nameof(ProjectTypeId))]
        public virtual ProjectTypeModel ProjectType { get; set; }

        [ExcelColumnName("Project Year")]
        public double ProjectYear { get; set; }

        public string Status { get; set; }

        public string SSVReport { get; set; }

        public bool SSVReportIsWaiver { get; set; }

        public string BulkBatchNumber { get; set; }

        public string BulkuploadPath { get; set; }

        public string EngineerRejectReport { get; set; }

        //[Required]
        [ExcelColumnName("Summer Config")]
        public string SummerConfigId { get; set; }

        [ForeignKey(nameof(SummerConfigId))]
        public virtual SummerConfigModel SummerConfig { get; set; }

        [ExcelColumnName("Software")]
        public string SoftwareVersion { get; set; }

        public string RequestType { get; set; }

        [ExcelColumnName("Integrated Date")]
        public DateTime IntegratedDate { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime DateCreated { get; set; }

        public string RequesterId { get; set; }

        [ForeignKey(nameof(RequesterId))]
        public virtual RequesterData Requester { get; set; }

        public string EngineerId { get; set; }

        [Display(Name = "Assigned Engineer")]
        [ForeignKey(nameof(EngineerId))]
        public virtual RequestApproverModel EngineerAssigned { get; set; }

        [ExcelColumnName("RET Configured")]
        public string RETConfigured { get; set; }

        [ExcelColumnName("Carrier Aggregation")]
        public string CarrierAggregation { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

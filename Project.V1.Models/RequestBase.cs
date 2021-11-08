using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Project.V1.Models
{
    public class RequestBase : IDisposable
    {
        [Key]
        public string Id { get; set; }

        public string UniqueId { get; set; }

        [Required]
        public string SiteId { get; set; }

        public string RNCBSC { get; set; }

        public string SiteName { get; set; }

        public string RegionId { get; set; }

        [ForeignKey(nameof(RegionId))]
        public virtual RegionViewModel Region { get; set; }

        public string Spectrum { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string AntennaTypeId { get; set; }

        [ForeignKey(nameof(AntennaTypeId))]
        public virtual AntennaTypeModel AntennaType { get; set; }

        public string AntennaHeight { get; set; }

        public string AntennaAzimuth { get; set; }

        public string MTilt { get; set; }

        public string ETilt { get; set; }

        public string BasebandId { get; set; }

        [ForeignKey(nameof(BasebandId))]
        public virtual BaseBandModel Baseband { get; set; }

        public string RUType { get; set; }

        public string RRUPower { get; set; }

        public string CSFDStatusGSM { get; set; }

        public string CSFDStatusWCDMA { get; set; }

        public string Power { get; set; }

        public string TechTypeId { get; set; }

        [ForeignKey(nameof(TechTypeId))]
        public virtual TechTypeModel TechType { get; set; }

        public string ProjectTypeId { get; set; }

        [ForeignKey(nameof(ProjectTypeId))]
        public virtual ProjectTypeModel ProjectType { get; set; }

        public string ProjectYear { get; set; }

        public string Status { get; set; }

        public string SSVReport { get; set; }

        public string EngineerRejectReport { get; set; }

        public virtual List<ActionReason> ActionReasons { get; set; }

        public string SummerConfigId { get; set; }

        [ForeignKey(nameof(SummerConfigId))]
        public virtual SummerConfigModel SummerConfig { get; set; }

        public string FrequencyBand { get; set; }

        public string SoftwareVersion { get; set; }

        public string RequestType { get; set; }

        public DateTime IntegratedDate { get; set; }

        public DateTime DateSubmitted { get; set; }

        public DateTime DateCreated { get; set; }

        [Display(Name = "Requester Data")]
        public virtual RequesterData Requester { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public T CreateCopy<T>() where T : RequestBase, new()
        {
            var copy = new T();

            var skipMapping = new string[] { "RRUPower", "CSFDStatusGSM", "CSFDStatusWCDMA" };

            // get properties that you actually care about
            var properties = typeof(T).GetProperties()
                .Where(x => !skipMapping.Contains(x.Name))
                //.Where(x => x.GetCustomAttribute<CopiablePropertyAttribute>() != null)
                ;

            foreach (var property in properties)
            {
                // set the value to the copy from the instance that called this method
                if (property.GetValue(this) != null)
                    property.SetValue(copy, property.GetValue(this));
            }

            return copy;
        }
    }
}

namespace Project.V1.Models.Holistic
{
    public class BaseHolistic
    {
        [Key]
        public string Id { get; set; }

        public string SiteId { get; set; }

        public string SiteName { get; set; }

        public string FinancialYear { get; set; }

        public string Remark { get; set; }

        public string Cluster { get; set; }

        public string City { get; set; }

        public string Lga { get; set; }

        public string State { get; set; }

        public string Region { get; set; }

        public string MarketingRegion { get; set; }

        public string OEM { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string PreviousTechPlan { get; set; }

        public string PreviousTechBandPlan { get; set; }

        public string CurrentTechPlan { get; set; }

        public string CurrentTechBandPlan { get; set; }

        public string BandTech { get; set; }

        public string ScopedYear { get; set; }

        public string ProjectType { get; set; }

        public string TowerOwner { get; set; }

        public string TowerProvider { get; set; }

        public string TowerHeight { get; set; }

        public string SummerConfig { get; set; }

        public string AntennaSolution { get; set; }

        public string NewSiteAntennaHeight { get; set; }

        public string FESiteAntennaHeight { get; set; }

        public string NewSiteAntennaSize { get; set; }

        public string FarEndSiteId { get; set; }

        public string HWEquipmentMatching { get; set; }

        public string Comment { get; set; }

        public string RTATP { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

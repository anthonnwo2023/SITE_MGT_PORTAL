namespace Project.V1.Models.Holistic
{
    public class NSB : BaseHolistic
    {
        public string ReleaseBatch { get; set; }

        public string CapexBOQ { get; set; }

        public string Remark { get; set; }

        public string TribandRadioSite { get; set; }

        public string PreviousTechPlan { get; set; }

        public string CurrentTechPlan { get; set; }

        public string BandTech { get; set; }

        public string ScopedYear { get; set; }

        public string ProjectType { get; set; }

        public string TowerProvider { get; set; }

        public string TowerHeight { get; set; }

        public string SummerConfig { get; set; }

        public string NewSiteAntennaHeight { get; set; }

        public string NewSiteAntennaSize { get; set; }

        public string FarEndSiteId { get; set; }

        public string HWEquipmentMatching { get; set; }

        public string Comment { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

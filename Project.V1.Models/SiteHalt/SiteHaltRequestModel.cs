namespace Project.V1.Models.SiteHalt
{
    public class SiteHaltRequestModel
    {
        public string SiteIds { get; set; }

        public string Reason { get; set; }

        public string SupportingDocument { get; set; }

        public string FirstApproverId { get; set; }

        [ForeignKey(nameof(FirstApproverId))]
        public virtual RequestApproverModel FirstApprover { get; set; }

        public string SecondApproverId { get; set; }

        [ForeignKey(nameof(SecondApproverId))]
        public virtual RequestApproverModel SecondApprover { get; set; }

        public string ThirdApproverId { get; set; }

        [ForeignKey(nameof(ThirdApproverId))]
        public virtual RequestApproverModel ThirdApprover { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_APPROVER")]
    public class RequestApproverModel : PersonModel
    {
        public string RequestId { get; set; }

        public string ApproverType { get; set; }

        [Display(Name = "Comment")]
        public string ApproverComment { get; set; }

        [Display(Name = "Request Actioned?")]
        public bool IsActioned { get; set; }

        public bool IsApproved { get; set; }

        public DateTime DateAssigned { get; set; }

        [Required]
        [Display(Name = "Date Actioned")]
        public DateTime DateApproved { get; set; }
    }
}

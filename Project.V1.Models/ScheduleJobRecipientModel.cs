using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Models
{

    [Table("TBL_RFACCEPT_SCHEDULEJOBRECIPIENT")]
    //[Index(new string[] { nameof(ToEmail) }, IsUnique = true)] // This will create unique constraint on MonthNo column only
    public class ScheduleJobRecipientModel : ObjectBase
    {
        //[Key]
        //public string Id { get; set; }

        [Display(Name = "Email Category")]
        // [Required]
        public string EmailCategory { get; set; } = "NA";

        [Display(Name = "To Email")]
        // [Required]
        public string ToEmail { get; set; }

        [Display(Name = "CC Email")]
        // [Required]
        public string CCEmail { get; set; }

        public string CreatedBy { get; set; }
        //  public bool IsActive { get; set; }
        //  public DateTime DateCreated { get; set; }

    }

}

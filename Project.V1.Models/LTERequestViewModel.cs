using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_4G_REQUESTS")]
    public class LTERequestViewModel : RequestBase
    {
        public string RRUPower { get; set; }

        public string CSFDStatusGSM { get; set; }

        public string CSFDStatusWCDMA { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_REGIONS")]
    public class RegionViewModel
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Abbr { get; set; }

        public string MailList { get; set; }

        public bool IsRegular { get; set; }

        public bool IsRural { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }
    }
}

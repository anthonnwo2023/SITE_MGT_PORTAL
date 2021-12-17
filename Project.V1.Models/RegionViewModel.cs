using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_REGIONS")]
    [Index(new string[] {nameof(Name)}, IsUnique = true)]
    public class RegionViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Abbr { get; set; }

        public string MailList { get; set; }

        public bool IsRegular { get; set; }

        public bool IsRural { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_BASEBANDS")]
    [Index(new string[] { nameof(Name) }, IsUnique = true)]
    public class BaseBandModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public virtual VendorModel Vendor { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

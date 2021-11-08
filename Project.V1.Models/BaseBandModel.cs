using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_BASEBANDS")]
    public class BaseBandModel
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string VendorId { get; set; }

        [ForeignKey(nameof(VendorId))]
        public virtual VendorModel Vendor { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

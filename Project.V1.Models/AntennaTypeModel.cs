using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_ANTENNA_TYPES")]
    public class AntennaTypeModel
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

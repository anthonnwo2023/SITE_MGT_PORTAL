using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.V1.Models
{
    [Table("TBL_RFACCEPT_SPECTRUM")]
    [Index(new string[] { nameof(Name), nameof(TechTypeId) }, IsUnique = true)]
    public class SpectrumViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string TechTypeId { get; set; }

        [ForeignKey(nameof(TechTypeId))]
        public virtual TechTypeModel TechType { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

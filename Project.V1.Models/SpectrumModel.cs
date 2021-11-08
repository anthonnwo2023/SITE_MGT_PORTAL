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
    public class SpectrumViewModel
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string TechTypeId { get; set; }

        [ForeignKey(nameof(TechTypeId))]
        public virtual TechTypeModel TechType { get; set; }

        public DateTime DateCreated { get; set; }
    }
}

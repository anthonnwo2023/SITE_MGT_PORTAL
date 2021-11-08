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
    [Table("TBL_RFACCEPT_CLAIMS")]
    [Index(new string[] { nameof(ClaimName), nameof(ClaimValue) }, IsUnique = true)]
    public class ClaimViewModel : IDisposable
    {
        [NotMapped]
        private bool disposed = false;

        [Key]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Claim Name")]
        public string ClaimName { get; set; }

        [Required]
        [Display(Name = "Claim Type")]
        public string ClaimValue { get; set; }

        [Required]
        public string CategoryId { get; set; }

        [Display(Name = "Category")]
        [ForeignKey(nameof(CategoryId))]
        public virtual ClaimCategoryModel Category { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        public DateTime DateCreated { get; set; }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //await this.DisposeAsync();
                }
            }
            this.disposed = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

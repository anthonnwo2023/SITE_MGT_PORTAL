namespace Project.V1.Models;

[Table("TBL_RFACCEPT_CLAIMS")]
[Index(new string[] { nameof(Name), nameof(Value) }, IsUnique = true)]
public class ClaimViewModel : ObjectBase, IDisposable
{
    [NotMapped]
    private bool disposed = false;

    [Required]
    [Display(Name = "Claim Type")]
    public string Value { get; set; }

    [Required]
    public string CategoryId { get; set; }

    [Display(Name = "Category")]
    [ForeignKey(nameof(CategoryId))]
    public virtual ClaimCategoryModel Category { get; set; }

    [NotMapped]
    public bool IsSelected { get; set; }

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


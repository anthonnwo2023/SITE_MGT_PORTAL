namespace Project.V1.Models;

[Table("TBL_RFACCEPT_CLAIM_CATEGORIES")]
[Index(new string[] { nameof(Name) }, IsUnique = true)]
public class ClaimCategoryModel : ObjectBase, IDisposable
{
    [NotMapped]
    private bool disposed = false;

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

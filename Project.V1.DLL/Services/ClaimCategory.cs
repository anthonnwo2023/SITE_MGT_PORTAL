using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class ClaimCategory : GenericRepo<ClaimCategoryModel>, IClaimCategory, IDisposable
{
    public ClaimCategory(ApplicationDbContext context, ICLogger logger)
        : base(context, null, logger)
    {
    }
}

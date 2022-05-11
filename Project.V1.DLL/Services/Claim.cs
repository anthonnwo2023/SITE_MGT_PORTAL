using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class ClaimService : GenericRepo<ClaimViewModel>, IClaimService, IDisposable
{
    public ClaimService(ApplicationDbContext context, ICLogger logger)
        : base(context, null, logger)
    {
    }
}

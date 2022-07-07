using Project.V1.DLL.Services.Interfaces.SiteHalt;

namespace Project.V1.DLL.Services.SiteHalt;

public class StakeHolder : GenericRepo<SiteHUDStakeholder>, IStakeholder, IDisposable
{
    public StakeHolder(ApplicationDbContext context, ICLogger logger)
        : base(context, null, logger)
    {
    }
}

using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class BaseBand : GenericRepo<BaseBandModel>, IBaseBand, IDisposable
{
    public BaseBand(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

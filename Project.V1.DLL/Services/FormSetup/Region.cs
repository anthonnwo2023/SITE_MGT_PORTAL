using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class Region : GenericRepo<RegionViewModel>, IRegion, IDisposable
{
    public Region(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

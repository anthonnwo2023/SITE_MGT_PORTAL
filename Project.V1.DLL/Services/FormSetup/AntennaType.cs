using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class AntennaType : GenericRepo<AntennaTypeModel>, IAntennaType, IDisposable
{
    public AntennaType(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class SummerConfig : GenericRepo<SummerConfigModel>, ISummerConfig, IDisposable
{
    public SummerConfig(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

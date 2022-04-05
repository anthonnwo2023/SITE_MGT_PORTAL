using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services
{
    public class TechType : GenericRepo<TechTypeModel>, ITechType, IDisposable
    {
        public TechType(ApplicationDbContext context, ICLogger logger)
            : base(context, "")
        {
        }
    }
}

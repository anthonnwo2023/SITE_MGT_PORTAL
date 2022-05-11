using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class ProjectType : GenericRepo<ProjectTypeModel>, IProjectType, IDisposable
{
    public ProjectType(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

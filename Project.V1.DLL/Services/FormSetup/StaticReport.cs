using Project.V1.DLL.Interface;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

public class StaticReport : GenericRepo<StaticReportModel>, IStaticReport, IDisposable
{
    public StaticReport(ApplicationDbContext context, ICLogger logger)
        : base(context, "", logger)
    {
    }
}

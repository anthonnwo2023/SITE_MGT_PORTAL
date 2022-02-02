using Project.V1.Data;
using Project.V1.DLL.Interface;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services
{
    public class StaticReport : GenericRepo<StaticReportModel>, IStaticReport, IDisposable
    {
        public StaticReport(ApplicationDbContext context)
            : base(context, "")
        {
        }
    }
}

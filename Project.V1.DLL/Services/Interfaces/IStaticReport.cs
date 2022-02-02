using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.DLL.Interface
{
    public interface IStaticReport : IGenericRepo<StaticReportModel>, IDisposable
    {
    }
}

using Project.V1.Data;
using Project.V1.DLL.Services.Interfaces.FormSetup;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services
{
    public class BaseBand : GenericRepo<BaseBandModel>, IBaseBand, IDisposable
    {
        public BaseBand(ApplicationDbContext context)
            : base(context, "")
        {
        }
    }
}

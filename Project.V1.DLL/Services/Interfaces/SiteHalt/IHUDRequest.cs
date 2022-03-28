using Project.V1.Models.SiteHalt;
using System;

namespace Project.V1.DLL.Services.Interfaces.SiteHalt
{
    public interface IHUDRequest : IRequestAction<SiteHUDRequestModel>, IDisposable
    {
    }
}

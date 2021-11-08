using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.DLL.Services.Interfaces
{
    public interface IClaimService : IGenericRepo<ClaimViewModel>, IDisposable
    {
        //Task<ClaimViewModel> GetByName(string Claimname, bool isActive);        
    }
}

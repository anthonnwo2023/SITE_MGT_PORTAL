using Project.V1.Data.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services.Interfaces
{
    public interface IClaimService : IGenericRepo<ClaimViewModel>, IDisposable
    {
        //Task<ClaimViewModel> GetByName(string Claimname, bool isActive);        
    }
}

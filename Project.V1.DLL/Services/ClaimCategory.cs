using Project.V1.Data;
using Project.V1.Lib.Interfaces;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services
{
    public class ClaimCategory : GenericRepo<ClaimCategoryModel>, IClaimCategory, IDisposable
    {
        public ClaimCategory(ApplicationDbContext context)
            : base(context, null)
        {
        }
    }
}

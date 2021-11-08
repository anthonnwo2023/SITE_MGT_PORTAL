using Microsoft.AspNetCore.Authorization;
using Project.V1.Data;
using Project.V1.DLL.Services.Interfaces;
using Project.V1.Lib.Interfaces;
using Project.V1.Models;
using System;

namespace Project.V1.Lib.Services
{

    [Authorize]
    public class Vendor : GenericRepo<VendorModel>, IVendor, IDisposable
    {
        public Vendor(ApplicationDbContext context)
            : base(context, null)
        {
        }
    }
}

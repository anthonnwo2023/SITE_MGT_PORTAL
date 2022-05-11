using Microsoft.AspNetCore.Authorization;
using Project.V1.Lib.Interfaces;

namespace Project.V1.Lib.Services;

[Authorize]
public class Vendor : GenericRepo<VendorModel>, IVendor, IDisposable
{
    public Vendor(ApplicationDbContext context, ICLogger logger)
        : base(context, null, logger)
    {
    }
}

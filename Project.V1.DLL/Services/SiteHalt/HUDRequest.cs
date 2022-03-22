using Project.V1.Data;
using Project.V1.DLL.Services.Interfaces.SiteHalt;
using Project.V1.Lib.Interfaces;
using Project.V1.Lib.Services;
using Project.V1.Models.SiteHalt;

namespace Project.V1.DLL.Services.SiteHalt
{
    public class HUDRequest : BaseActionOps<SiteHaltRequestModel>, IHUDRequest
    {
        public HUDRequest(ApplicationDbContext context, ICLogger logger)
            : base(context, "HUD", logger)
        {

        }
    }
}
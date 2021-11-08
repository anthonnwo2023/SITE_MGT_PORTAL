using Project.V1.Data;
using Project.V1.Lib.Interfaces;
using Project.V1.DLL.Interface;
using Project.V1.Models;

namespace Project.V1.Lib.Services
{
    public class Request : BaseActionOps<RequestViewModel>, IRequest
    {
        public Request(ApplicationDbContext context, ICLogger logger)
            : base(context, "SA", logger)
        {
        }
    }
}

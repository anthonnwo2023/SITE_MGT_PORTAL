using Microsoft.Extensions.Primitives;

namespace Project.V1.Web.Controllers;

[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public class AcceptanceController : Controller
{
    private readonly IRequest _request;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVendor _vendor;

    public AcceptanceController(IRequest request, UserManager<ApplicationUser> userManager, IVendor vendor)
    {
        (_request, _userManager, _vendor) = (request, userManager, vendor);
    }

    [HttpGet]
    public async Task<object> Get()
    {
        var queryString = Request.Query;
        var httpRequest = HttpContext.Request;
        var requestModel = new RequestViewModel();

        var Requests = Array.Empty<RequestViewModel>().AsQueryable();

        var username = queryString.TryGetValue("User", out StringValues User) ? User[0] : null;
        var isAuthenticated = queryString.TryGetValue("IsAuthenticated", out StringValues IsAuth) ? Convert.ToBoolean(IsAuth[0]) : false;
        var claimStr = queryString.TryGetValue("Claims", out StringValues Claims) ? Claims[0] : String.Empty;
        var claims = claimStr.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        if (!isAuthenticated || !claims.Contains("Can:ViewReport"))
        {
            return Requests;
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return Requests;

        var vendor = await _vendor.GetById(x => x.Id == user.VendorId);

        if (user.ShowAllRegionReport)
        {
            Requests = await _request.Get(x => x.Id != null, null, requestModel.Navigations);
        }
        else if (vendor.Name == "MTN Nigeria" || (await LoginObject.UserManager.IsInRoleAsync(user, "User")))
        {
            Requests = await _request.Get(x => user.Regions.Select(x => x.Id).Contains(x.RegionId), null, requestModel.Navigations);
        }
        else
        {
            Requests = await _request.Get(x => x.Requester.VendorId == user.VendorId, x => x.OrderByDescending(y => y.DateCreated), requestModel.Navigations);
        }

        var count = Requests.Count();

        if (queryString.Keys.Contains("$inlinecount"))
        {
            int skip = queryString.TryGetValue("$skip", out StringValues Skip) ? Convert.ToInt32(Skip[0]) : 0;
            int top = queryString.TryGetValue("$top", out StringValues Take) ? Convert.ToInt32(Take[0]) : Requests.Count();

            return new { Items = Requests.Skip(skip).Take(top), Count = count };
        }
        else
        {
            return Requests;
        }
    }
}

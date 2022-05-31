namespace Project.V1.Web.Controllers;

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
    [EnableQuery]
    public async Task<IQueryable<RequestViewModel>> Get()
    {
        var httpRequest = HttpContext.Request;
        var requestModel = new RequestViewModel();

        var Requests = Array.Empty<RequestViewModel>().AsQueryable();

        var username = httpRequest.Headers["User"].ToString();
        var isAuthenticated = Convert.ToBoolean(httpRequest.Headers["IsAuthenticated"]);
        var claims = httpRequest.Headers["Claims"]!.ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);

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

        return Requests.AsQueryable();
    }
}

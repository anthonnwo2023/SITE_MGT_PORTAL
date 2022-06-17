namespace Project.V1.Web.Controllers;

public class SSCRequestController : Controller
{
    private readonly ISSCRequestUpdate _request;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVendor _vendor;

    public SSCRequestController(ISSCRequestUpdate request, UserManager<ApplicationUser> userManager, IVendor vendor)
    {
        (_request, _userManager, _vendor) = (request, userManager, vendor);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<IQueryable<SSCUpdatedCell>> Get()
    {
        var httpRequest = HttpContext.Request;
        var requestModel = new SSCUpdatedCell();

        var Requests = Array.Empty<SSCUpdatedCell>().AsQueryable();

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

        Requests = await _request.Get(x => x.ID != 0, x => x.OrderByDescending(y => y.DATECREATED), "");

        return Requests.AsQueryable();
    }
}

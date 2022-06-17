using AutoMapper;
using AutoMapper.QueryableExtensions;
using System;

namespace Project.V1.Web.Controllers;

public class AcceptanceController : Controller
{
    private readonly IRequest _request;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IVendor _vendor;
    private readonly IMapper _mapper;

    public AcceptanceController(IRequest request, UserManager<ApplicationUser> userManager, IVendor vendor, IMapper mapper)
    {
        (_request, _userManager, _vendor, _mapper) = (request, userManager, vendor, mapper);
    }

    [HttpGet]
    public async Task<IQueryable<RequestViewModelDTO>> Get(ODataQueryOptions<RequestViewModelDTO> options)
    {
        var httpRequest = HttpContext.Request;
        var requestModel = new RequestViewModel();

        var Requests = Array.Empty<RequestViewModel>().AsQueryable();

        var username = httpRequest.Headers["User"].ToString();
        var isAuthenticated = Convert.ToBoolean(httpRequest.Headers["IsAuthenticated"]);
        var claims = httpRequest.Headers["Claims"]!.ToString().Split(", ", StringSplitOptions.RemoveEmptyEntries);

        if (!isAuthenticated || !claims.Contains("Can:ViewReport"))
        {
            return Array.Empty<RequestViewModelDTO>().AsQueryable();
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return Array.Empty<RequestViewModelDTO>().AsQueryable();

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

        var requestDTO = Requests.ProjectTo<RequestViewModelDTO>(_mapper.ConfigurationProvider);
        var count = requestDTO.Count();

        if (options.Filter != null)
        {
            requestDTO = options.Filter.ApplyTo(requestDTO, new ODataQuerySettings()).Cast<RequestViewModelDTO>();
        }

        var odataOptions = options.ApplyTo(requestDTO);

        var countOpt = odataOptions.Count();
        var odtOptParma = odataOptions.Parameter();
        var odtOptQS = odataOptions.ToQueryString();

        ((IQueryable<RequestViewModelDTO>)odataOptions).ToList().ForEach(x =>
        {
            x.DateActioned = GetDateActioned(x);
        });

        return (IQueryable<RequestViewModelDTO>)odataOptions;
    }

    private DateTime? GetDateActioned(RequestViewModelDTO request)
    {
        if (request.Status != "Accepted" && request.Status != "Rejected")
        {
            return request.DateUserActioned.GetValueOrDefault();
        }
        if (request.EngineerAssigned != null)
        {
            return (request.EngineerAssignedIsApproved) ? request.EngineerAssignedDateApproved : request.EngineerAssignedDateActioned;
        }

        return null;
    }
}

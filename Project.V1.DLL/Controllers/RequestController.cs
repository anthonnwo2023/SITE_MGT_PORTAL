namespace Project.V1.DLL.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class RequestController : ControllerBase
{
    //private readonly IRequest _request;

    //public RequestController(IRequest request)
    //{
    //    _request = request;
    //}

    //[HttpGet]
    //[EnableQuery]
    //public async Task<IQueryable<RequestViewModel>> Get()
    //{
    //    return (await _request.Get()).AsQueryable();
    //}
}

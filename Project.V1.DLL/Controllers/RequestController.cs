using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project.V1.DLL.Controllers;

[Controller]
[Authorize]
[Route("acceptance/api/[controller]")]
public class RequestController : ControllerBase
{
    public RequestController()
    {

    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("I am ok");
    }
}

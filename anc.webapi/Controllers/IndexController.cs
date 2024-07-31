using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace anc.webapi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class IndexController : ControllerBase
{
    [HttpGet]

    public IActionResult Get()
    {
        return Ok();
    }
}
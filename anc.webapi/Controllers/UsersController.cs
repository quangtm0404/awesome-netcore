using anc.applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace anc.webapi.Controllers;
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;
    public UsersController(IUserService userService)
    {
        this.userService = userService;
    }
    [DisableRateLimiting]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(string username,
        string email,
        int quota,
        int timeLimit)
    {
        await userService.CreateAsync(username, email, quota, timeLimit);
        return Ok();
    }

    [DisableRateLimiting]
    [HttpPost("rate-limit-modify")]
    public async Task<IActionResult> IncreaseRateLimit(string apiKey,
        int quota,
        int timeLimit)
    {
        await userService.UpdateRateLimitAsync(apiKey, quota, timeLimit);
        return NoContent();
    }
}
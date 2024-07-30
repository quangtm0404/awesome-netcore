/*namespace anc.webapi;
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, PartitionedRateLimiter<HttpContext, string> ipRateLimiter,
        PartitionedRateLimiter<HttpContext, string> apiKeyRateLimiter)
    {
        if (await ApplyRateLimiter(context, ipRateLimiter, "rate-limit-ip") &&
            await ApplyRateLimiter(context, apiKeyRateLimiter, "rate-limit-api-key"))
        {
            await _next(context);
        }
    }

    private async Task<bool> ApplyRateLimiter(HttpContext context, PartitionedRateLimiter<HttpContext, string> rateLimiter, string policyName)
    {
        var lease = await rateLimiter.AcquireAsync(context, 1, context.RequestAborted);
        if (!lease.IsAcquired)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = lease.RetryAfter?.TotalSeconds.ToString();
            await context.Response.WriteAsync($"Rate limit exceeded for policy: {policyName}");
            return false;
        }
        return true;
    }
}
*/
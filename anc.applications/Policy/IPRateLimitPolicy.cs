using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace anc.webapi.Policy;
public class IPRateLimitPolicy : IRateLimiterPolicy<string>
{
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => (context, lease) =>
        {
            context.HttpContext.Response.StatusCode = 429;
            context.HttpContext.Response.WriteAsync("Too many request, Please try again!");
            return new ValueTask();
        };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var ipKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        System.Console.WriteLine(ipKey);
        if (string.IsNullOrEmpty(ipKey) || ipKey == "::1")
        {
            return RateLimitPartition.GetFixedWindowLimiter(ipKey, key =>
                new FixedWindowRateLimiterOptions()
                {
                    AutoReplenishment = false,
                    PermitLimit = 2,
                    Window = TimeSpan.FromSeconds(20)
                });
        }
        else
        {
            return RateLimitPartition.GetNoLimiter(ipKey);
        }
    }
}
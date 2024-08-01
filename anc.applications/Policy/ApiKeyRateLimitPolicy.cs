using System.Net;
using System.Threading.RateLimiting;
using anc.applications.Repositories;
using anc.domains.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace anc.webapi.Policy;
public class APIRateLimitPolicy : IRateLimiterPolicy<string>
{
    public APIRateLimitPolicy()
    {
    }
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        await context.HttpContext.Response
            .WriteAsync($"Too Many Request API Key: {context.HttpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty}! Please Try Again",
                cancellationToken: cancellationToken);
    };
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var memoryCache = httpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var apiKey = httpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(apiKey))
        {
            var host = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            return RateLimitPartition.GetFixedWindowLimiter(host, key =>
               new FixedWindowRateLimiterOptions()
               {
                   AutoReplenishment = false,
                   PermitLimit = 5,
                   Window = TimeSpan.FromSeconds(20)
               });
        }
        else
        {
            memoryCache.TryGetValue(apiKey, out User? user);
            if (user is null)
            {
                var userRepo = httpContext.RequestServices.GetRequiredService<IUserRepository>();
                user = userRepo.GetUserByApiKey(apiKey) ?? throw new Exception();
                memoryCache.Set(apiKey, user);
            }
            return RateLimitPartition.GetFixedWindowLimiter(user.ApiKey, key =>
               new FixedWindowRateLimiterOptions()
               {
                   AutoReplenishment = false,
                   PermitLimit = user.Quota,
                   Window = TimeSpan.FromSeconds(user.TimeLimit)
               });
        }

    }
}
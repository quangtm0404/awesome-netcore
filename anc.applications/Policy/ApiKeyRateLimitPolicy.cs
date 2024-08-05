using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using anc.applications.Repositories;
using anc.domains.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace anc.webapi.Policy;
public class APIRateLimitPolicy : IRateLimiterPolicy<string>
{
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        if (cache is not null)
        {
            var apiKey = context.HttpContext.Request.Headers["ApiKey"];
            cache.Set($"{apiKey}_banned", string.Empty, TimeSpan.FromSeconds(30));
        }
        await context.HttpContext.Response
            .WriteAsync($"Too Many Request API Key: {context.HttpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty}! Please Try Again",
                cancellationToken: cancellationToken);
    };
    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        var watch = Stopwatch.StartNew();
        var memoryCache = httpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var logger = httpContext.RequestServices.GetRequiredService<ILogger<APIRateLimitPolicy>>();
        var apiKey = httpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty;
        logger.LogInformation("ApiKey: {0}, Time Process: {1} _ Start Process", apiKey, watch.ElapsedMilliseconds);
        if (string.IsNullOrEmpty(apiKey))
        {

            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            logger.LogInformation("RateLimitPolicy_IP Adress: {0}", ipAddress);
            return RateLimitPartition.GetFixedWindowLimiter(ipAddress, key =>
               new FixedWindowRateLimiterOptions()
               {
                   AutoReplenishment = false,
                   PermitLimit = 10,
                   Window = TimeSpan.FromSeconds(20)
               });
        }
        else
        {
            memoryCache.TryGetValue(apiKey, out User? user);
            logger.LogInformation("ApiKey: {0}, Time Process: {1}", apiKey, watch.ElapsedMilliseconds);
            if (user is null)
            {
                var userRepo = httpContext.RequestServices.GetRequiredService<IUserRepository>();
                user = userRepo.GetUserByApiKey(apiKey) ?? throw new Exception();
                memoryCache.Set(apiKey, user,
                    absoluteExpiration: DateTimeOffset.Now.AddMinutes(1));
            }
            logger.LogInformation("ApiKey: {0}, Time Process: {1} _ Done Process", apiKey, watch.ElapsedMilliseconds);
            return RateLimitPartition.GetConcurrencyLimiter(user.ApiKey, key =>
            {
                return new ConcurrencyLimiterOptions()
                {
                    PermitLimit = user.Quota
                };
            });
        }

    }
}
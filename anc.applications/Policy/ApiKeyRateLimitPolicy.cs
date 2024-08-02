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
            logger.LogInformation("Not Have API Key");
            var host = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            return RateLimitPartition.GetFixedWindowLimiter(host, key =>
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
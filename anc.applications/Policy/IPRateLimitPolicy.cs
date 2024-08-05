using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace anc.webapi.Policy;
public class IPRateLimitPolicy : IRateLimiterPolicy<string> // TPartionKey
{
    public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => (context, lease) =>
        {
            // Logic Implement khi request bị reject
            return new ValueTask();
        };

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        return RateLimitPartition.GetConcurrencyLimiter("demoKey", key =>
        {
            return new ConcurrencyLimiterOptions() 
            {

            };
        });
    }
}
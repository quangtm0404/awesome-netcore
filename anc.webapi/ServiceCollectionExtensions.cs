/*using Polly;
using Polly.RateLimiting;

namespace anc.webapi
{
    public static class MyResilienceKeys
    {
        public static readonly ResiliencePropertyKey<string> RemoteIp = new("ip-address");
        public static readonly ResiliencePropertyKey<string> ApiKey = new("apiKey");
        public static readonly ResiliencePropertyKey<HttpContext> context = new("httpContext");
    }
    public static class ServiceCollectionExtensions
    {
        public static string GetPartitionKey(HttpContext context)
        {
            //return context.Connection.RemoteIpAddress?.ToString();
            return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        public static IServiceCollection AddRateLimit(this IServiceCollection services)
        {
            var partitionedLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                // Extract the partition key (IP address).
                string partitionKey = GetPartitionKey(context) ?? string.Empty;

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey,
                    key => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        SegmentsPerWindow = 10,
                        Window = TimeSpan.FromSeconds(5)
                    });
            });

            services.AddResiliencePipeline("rate-limit", builder =>
            {
                builder.AddRateLimiter(new RateLimiterStrategyOptions
                {
                    RateLimiter = args =>
                    {
                        args.Context.Properties.TryGetValue<HttpContext>(MyResilienceKeys.context, out HttpContext? context);
                        return partitionedLimiter.AcquireAsync(context ?? throw new InvalidOperationException(), 1, args.Context.CancellationToken);
                    }
                });
            });

            return services;
        }
    }
}
*/
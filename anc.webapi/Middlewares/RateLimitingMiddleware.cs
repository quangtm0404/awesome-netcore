
// using System.Net;
// using System.Threading.RateLimiting;

// namespace anc.webapi.Middlewares;
// public class RateLimitingMiddleware : IMiddleware
// {
//     public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//     {
//         try
//         {
//             var limiter = CreateAPIKeyLimiter();
//             await limiter.AcquireAsync();
//             await next(context);
//         }
//         catch (Exception ex)
//         {
//             context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
//             await context.Response.WriteAsync("Too Many Request");
//         }
//     }
//     private PartitionedRateLimiter<HttpContext> CreateAPIKeyLimiter()
//     {
//         return PartitionedRateLimiter.Create<HttpContext, string>(resource =>
//         {
//             var apiKey = resource.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
//             return RateLimitPartition.GetTokenBucketLimiter(apiKey, key =>
//            new TokenBucketRateLimiterOptions()
//            {
//                AutoReplenishment = true,
//                QueueLimit = 10,
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                TokenLimit = 10,
//                TokensPerPeriod = 10,
//                ReplenishmentPeriod = TimeSpan.FromSeconds(10)
//            });
//         });

//     }

// }
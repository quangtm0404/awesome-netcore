using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRateLimiter(_ =>
// {
//     _.OnRejected = (context, _) =>
//     {
//         if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
//         {
//             context.HttpContext.Response.Headers.RetryAfter =
//                 ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
//         }

//         context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//         context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

//         return new ValueTask();
//     };
//     _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
//         PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//         { 
//             var userAgent = httpContext.Request.Headers["ApiKey"].ToString();

//             return RateLimitPartition.GetFixedWindowLimiter
//             (userAgent, _ =>
//                 new FixedWindowRateLimiterOptions
//                 {
//                     AutoReplenishment = true,
//                     PermitLimit = 10,
//                     Window = TimeSpan.FromSeconds(100)
//                 });
//         }),
//         PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//         {
//             var userAgent = httpContext.Connection.RemoteIpAddress?.ToString()
//                 ?? string.Empty;

//             return RateLimitPartition.GetFixedWindowLimiter
//             (userAgent, _ =>
//                 new FixedWindowRateLimiterOptions
//                 {
//                     AutoReplenishment = true,
//                     PermitLimit = 5,    
//                     Window = TimeSpan.FromSeconds(100)
//                 });
//         }));
// });
builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.AddPolicy("rate-limit-ip", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        return RateLimitPartition.GetSlidingWindowLimiter(
            ipAddress,
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(10),
                SegmentsPerWindow = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
    });
    limiterOptions.AddPolicy("rate-limit-apikey", context =>
    {
        var apiKey = context.Request.Headers["ApiKey"].ToString() ?? string.Empty;

        return RateLimitPartition.GetSlidingWindowLimiter(
            apiKey,
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(10),
                SegmentsPerWindow = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
//app.UseRateLimiter();
app.UseRateLimiter(new RateLimiterOptions()
{
    OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Please Try Again With ApiKey: " + context.HttpContext.Request.Headers["ApiKey"]);
        return new ValueTask();
    },
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var apiKey = httpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(apiKey))
        {
            return RateLimitPartition.GetFixedWindowLimiter
            (apiKey, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 1,
                    Window = TimeSpan.FromSeconds(15)
                });
        }
        else if (apiKey.Contains("Enterprise"))
        {
            return RateLimitPartition.GetFixedWindowLimiter
           (apiKey, _ =>
               new FixedWindowRateLimiterOptions
               {
                   AutoReplenishment = true,
                   PermitLimit = 20,
                   Window = TimeSpan.FromSeconds(15)
               });
        }
        else
        {
            return RateLimitPartition.GetFixedWindowLimiter
            (apiKey, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(15)
                });
        }
    }),
    RejectionStatusCode = (int)HttpStatusCode.TooManyRequests
});

// app.UseRateLimiter(new RateLimiterOptions()
// {

//     OnRejected = (context, cancellationToken) =>
//     {
//         context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//         context.HttpContext.Response.WriteAsync("Please Try Again With ApiKey: " + context.HttpContext.Request.Headers["ApiKey"]);
//         return new ValueTask();
//     }
// };
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

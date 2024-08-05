using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseRateLimiter(new RateLimiterOptions()
{
    // Limiter được sử dụng Global
    GlobalLimiter = null,
    OnRejected = (context, cancelToken) =>
    {
        // Implement Logic khi từ chối request - Vượt quá limit
        return new ValueTask();
    },
    RejectionStatusCode = (int)HttpStatusCode.TooManyRequests
}.AddPolicy("name", (context) =>
{
    return RateLimitPartition.GetConcurrencyLimiter("exampleKey", key => 
        new ConcurrencyLimiterOptions()
        {
            // Số lượng request đồng thời
            PermitLimit = int.MinValue,
            // Nếu vượt quá Permit, số lượng request sẽ được đưa vào queue
            QueueLimit = int.MinValue,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
}));
app.MapReverseProxy(proxy =>
{
    proxy.UseLoadBalancing();
});
app.Run();

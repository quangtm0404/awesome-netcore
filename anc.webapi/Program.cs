using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
app.UseRateLimiter(new Microsoft.AspNetCore.RateLimiting.RateLimiterOptions()
{
    OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Please Try Again With ApiKey: " + context.HttpContext.Request.Headers["ApiKey"]);
        return new ValueTask();
    },
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ipAddress = httpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty;
        return RateLimitPartition.GetFixedWindowLimiter
            (ipAddress, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(10)
                });
    }),
    RejectionStatusCode = (int)HttpStatusCode.TooManyRequests
});
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

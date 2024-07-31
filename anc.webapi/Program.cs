using anc.webapi;
using anc.webapi.Middlewares;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRateLimiting();
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
    }
}
.AddPolicy("ip-rate-limit", new IPRateLimitPolicy()));
// app.UseRateLimiter(new RateLimiterOptions()
// {
//     OnRejected = (context, cancellationToken) =>
//     {
//         context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
//         context.HttpContext.Response.WriteAsync("Please Try Again With ApiKey: " + context.HttpContext.Request.Headers["ApiKey"]);
//         return new ValueTask();
//     },
//     GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
//     {
//         var apiKey = httpContext.Request.Headers["ApiKey"].ToString() ?? string.Empty;
//         if (string.IsNullOrEmpty(apiKey))
//         {
//             return RateLimitPartition.GetFixedWindowLimiter
//             (apiKey, _ =>
//                 new FixedWindowRateLimiterOptions
//                 {
//                     AutoReplenishment = true,
//                     PermitLimit = 1,
//                     Window = TimeSpan.FromSeconds(15)
//                 });
//         }
//         else if (apiKey.Contains("Enterprise"))
//         {
//             return RateLimitPartition.GetFixedWindowLimiter
//            (apiKey, _ =>
//                new FixedWindowRateLimiterOptions
//                {
//                    AutoReplenishment = true,
//                    PermitLimit = 20,
//                    Window = TimeSpan.FromSeconds(15)
//                });
//         }
//         else
//         {
//             return RateLimitPartition.GetFixedWindowLimiter
//             (apiKey, _ =>
//                 new FixedWindowRateLimiterOptions
//                 {
//                     AutoReplenishment = true,
//                     PermitLimit = 5,
//                     Window = TimeSpan.FromSeconds(15)
//                 });
//         }
//     }),
//     RejectionStatusCode = (int)HttpStatusCode.TooManyRequests
// });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting("ip-rate-limit");

app.Run();

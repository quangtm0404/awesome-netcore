using anc.applications;
using anc.repositories;
using anc.webapi;
using anc.webapi.Policy;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
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
.AddPolicy("apiKey", new APIRateLimitPolicy()));



// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting("apiKey");

app.Run();


using System.Net;

using Microsoft.Extensions.Caching.Memory;


public class RateLimitingMiddleware : IMiddleware
{

    private readonly ILogger<RateLimitingMiddleware> logger;
    public RateLimitingMiddleware(ILogger<RateLimitingMiddleware> logger)
    {
        this.logger = logger;
    }
  
    public Task HandleException(HttpContext context, string apiKey) => context.Response
                .WriteAsync($"Too Many Request API Key: {apiKey}! Please Try Again");
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var apiKey = context.Request.Headers["ApiKey"].ToString();
        var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
        if (cache is not null)
        {
            cache.TryGetValue($"{apiKey}_banned", out string? value);
            if (value is not null)
            {
                logger.LogError("Banned Process");
                // Banned Limited Request
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await HandleException(context, apiKey ?? string.Empty);
            }  else 
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }


    }
}
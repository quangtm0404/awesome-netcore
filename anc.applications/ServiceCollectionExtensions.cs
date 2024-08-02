using anc.applications.HealthChecks;
using anc.webapi.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using StackExchange.Redis;

namespace anc.applications;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();
        IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.ConnectAsync(configuration.GetConnectionString("Redis")
            ?? string.Empty).Result;
        services.AddSingleton(redisConnectionMultiplexer);
        services.AddStackExchangeRedisCache(options => options.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer));
        services.AddRateLimiter(cfg =>
        {
            cfg.AddPolicy("apiKey", new APIRateLimitPolicy());
        });
        services.AddCustomHealthCheck(configuration);
        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection service,
        IConfiguration configuration)
    {
        service.AddHealthChecks()
            .AddCheck<AppHealthCheck>("app-check");
        return service;
    }

    public static IServiceCollection AddMetric(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(opt => opt
                .AddOtlpExporter()
                .AddMeter("count_concurrent_user"));
        return services;
    }
}
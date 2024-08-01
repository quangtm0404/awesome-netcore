using anc.applications.Services;
using anc.applications.Services.Interfaces;
using anc.webapi.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            .AddRedis(configuration.GetConnectionString("Redis") ?? string.Empty)
            .AddNpgSql(configuration.GetConnectionString("PostgreSQL") ?? string.Empty);
        return service;
    }
}
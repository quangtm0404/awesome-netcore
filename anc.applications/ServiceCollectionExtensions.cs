using anc.applications.Services;
using anc.applications.Services.Interfaces;
using anc.webapi.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace anc.applications;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddRateLimiter(cfg =>
        {
            cfg.AddPolicy("apiKey", new APIRateLimitPolicy());
        });
        return services;
    }
}
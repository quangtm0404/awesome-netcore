using anc.domains.Entities;
using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;
using StackExchange.Redis;

namespace anc.applications.HealthChecks;
public class AppHealthCheck : IHealthCheck
{
    private readonly IConfiguration configuration;
    private readonly ILogger<AppHealthCheck> logger;
    private readonly IMemoryCache memoryCache;
    public AppHealthCheck(IConfiguration configuration,
        ILogger<AppHealthCheck> logger,
        IMemoryCache memoryCache)
    {
        this.logger = logger;
        this.memoryCache = memoryCache;
        this.configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Check if service is healthy or not
        try
        {
            var postgres = new NpgsqlConnection(configuration.GetConnectionString("PostgreSQL"));
            var redis = ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")
                ?? string.Empty);

            if (postgres is null || redis is null)
            {
                throw new ConnectionAbortedException();
            }
            string commandText = "SELECT * FROM User";
            var users = await postgres.QueryAsync<User>(command: new CommandDefinition(commandText: commandText,
                parameters: null, transaction: null,
                commandTimeout: 30,
                commandType: System.Data.CommandType.Text));
            if (users is not null && users.Count() > 0)
            {
                foreach (var user in users)
                {
                    memoryCache.Set(user.ApiKey, user,
                        DateTimeOffset.UtcNow.AddSeconds(30));
                }
                return HealthCheckResult.Healthy();
            }
            else
            {
                return HealthCheckResult.Unhealthy("Unhealthy, not have any cache");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("unhealthy while healthcheck", ex);
        }
    }
}
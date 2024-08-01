using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace anc.applications.HealthChecks;
public class AppHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
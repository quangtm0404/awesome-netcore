using System.Threading.RateLimiting;

namespace anc.webapi.RateLimit;
public class IPRateLimiter : RateLimiter
{
    public override TimeSpan? IdleDuration => throw new NotImplementedException();

    public override RateLimiterStatistics? GetStatistics()
    {
        throw new NotImplementedException();
    }

    protected override ValueTask<RateLimitLease> AcquireAsyncCore(int permitCount,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override RateLimitLease AttemptAcquireCore(int permitCount)
    {
        throw new NotImplementedException();
    }
}
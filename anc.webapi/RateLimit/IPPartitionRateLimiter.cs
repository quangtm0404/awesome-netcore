using System.Threading.RateLimiting;

namespace anc.webapi.RateLimit;
public sealed class IPPartitionRateLimiter : PartitionedRateLimiter<HttpContext>
{
    public override RateLimiterStatistics? GetStatistics(HttpContext resource)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask<RateLimitLease> AcquireAsyncCore(HttpContext resource,
        int permitCount,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override RateLimitLease AttemptAcquireCore(HttpContext resource,
        int permitCount)
    {
        throw new NotImplementedException();
    }
    private sealed class Lease : RateLimitLease
    {
        private readonly int permitCount;
        private readonly HttpContext resource;
        private IPPartitionRateLimiter? limiter;
        public Lease(IPPartitionRateLimiter limiter,
            HttpContext resource,
            int permitCount)
        {
            this.limiter = limiter;
            this.resource = resource;
            this.permitCount = permitCount;
        }
        public override bool IsAcquired => limiter is not null;

        public override IEnumerable<string> MetadataNames => throw new NotImplementedException();

        public override bool TryGetMetadata(string metadataName, out object? metadata)
        {
            throw new NotImplementedException();
        }
    }
}

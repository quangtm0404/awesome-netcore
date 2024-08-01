using anc.applications.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace anc.applications.Services;
public class CacheService : ICacheService
{
    private readonly IDistributedCache cache;
    public CacheService(IDistributedCache cache)
    {
        this.cache = cache;
    }
    public async Task<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken)
    {
        if (IsConnected())
        {
            return JsonConvert.DeserializeObject<TValue>(await cache.GetStringAsync(key) ?? string.Empty);
        }
        else return default(TValue);
    }

    public bool IsConnected()
        => cache is not null;



    public async Task SetAsync<TValue>(string key,
        TValue value,
        int absoluteExpiration = 15,
        int slidingExpiration = 15,
        CancellationToken cancellationToken = default)
    {
        if (IsConnected())
        {
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(absoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(slidingExpiration)
            }, cancellationToken);
        }
        else
        {
            await Task.CompletedTask;
        }


    }
}
// using anc.webapi.Services.Interfaces;
// using Microsoft.Extensions.Caching.Distributed;
// using Newtonsoft.Json;

// namespace anc.webapi.Services;
// public class CacheService : ICacheService
// {
//     private readonly IDistributedCache cache;
//     public CacheService(IDistributedCache cache)
//     {
//         this.cache = cache;
//     }

//     public async Task ClearAsync()
//     {
//         await Task.CompletedTask;
//     }

//     public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
//     {
//         string? cacheValue = await cache.GetStringAsync(key, cancellationToken);
//         if (cacheValue is null)
//         {
//             return null;
//         }
//         return JsonConvert.DeserializeObject<T>(cacheValue);

//     }

//     public bool IsConnected()
//     {
//         throw new NotImplementedException();
//     }

//     public Task<bool> SetAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
//     {
//         throw new NotImplementedException();
//     }
// }
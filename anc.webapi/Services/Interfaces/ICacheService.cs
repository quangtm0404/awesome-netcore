namespace anc.webapi.Services.Interfaces;
public interface ICacheService
{
    bool IsConnected();
    Task<T> GetAsync<T>(string key,
        CancellationToken cancellationToken = default);
    Task<bool> SetAsync<T>(string key, T value,
        CancellationToken cancellationToken = default);
    Task ClearAsync();
}
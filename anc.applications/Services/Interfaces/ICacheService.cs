namespace anc.applications.Services.Interfaces;
public interface ICacheService
{
    Task<TValue?> GetAsync<TValue>(string key,
        CancellationToken cancellationToken);
    Task SetAsync<TValue>(string key,
        TValue value,
        int absoluteExpiration = 15,
        int slidingExpiration = 15,
        CancellationToken cancellationToken = default);
    bool IsConnected();
}
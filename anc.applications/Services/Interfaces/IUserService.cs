namespace anc.applications.Services.Interfaces;
public interface IUserService
{
    Task CreateAsync(string username,
        string email,
        int quota,
        int timeLimit,
        CancellationToken cancellationToken = default);
    Task UpdateRateLimitAsync(string apiKey,
        int quota,
        int timeLimit);
}
using anc.applications.Repositories;
using anc.applications.Services.Interfaces;
using anc.domains.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace anc.applications.Services;
public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IMemoryCache memoryCache;
    public UserService(IUserRepository userRepository,
        IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
        this.userRepository = userRepository;
    }
    public async Task CreateAsync(string username, string email, int quota, int timeLimit, CancellationToken cancellationToken = default)
    {
        User user = new()
        {
            ApiKey = Guid.NewGuid().ToString(),
            Email = email,
            Quota = quota,
            TimeLimit = timeLimit,
            Username = username
        };
        await userRepository.CreateUser(user);
    }

    public async Task UpdateRateLimitAsync(string apiKey, int quota, int timeLimit)
    {
        var user = userRepository.GetUserByApiKey(apiKey)
            ?? throw new Exception();
        user.Quota = quota;
        user.TimeLimit = timeLimit;
        await userRepository.UpdateUser(user);
        memoryCache.Set(user.ApiKey, user);
    }
}
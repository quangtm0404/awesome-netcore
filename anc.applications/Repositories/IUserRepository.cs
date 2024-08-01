using anc.domains.Entities;

namespace anc.applications.Repositories;
public interface IUserRepository
{
    User? GetUserByApiKey(string apiKey);
    Task<User> CreateUser(User user);
    Task UpdateUser(User user);
}
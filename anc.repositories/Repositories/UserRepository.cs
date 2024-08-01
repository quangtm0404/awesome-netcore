using anc.applications.Repositories;
using anc.domains.Entities;
using anc.repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace anc.repositories.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext dbContext;
    public UserRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<User> CreateUser(User user)
    {
        await dbContext.User.AddAsync(user);
        return dbContext.SaveChanges() > 0 ? user : new();
    }

    public User? GetUserByApiKey(string apiKey)
    => dbContext.User
        .AsNoTracking()
        .FirstOrDefault(x => x.ApiKey == apiKey);

    public async Task UpdateUser(User user)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync();
    }
}
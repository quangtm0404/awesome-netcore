using anc.webapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace anc.webapi.Data;
public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{
    public DbSet<User> User { get; set; }
}
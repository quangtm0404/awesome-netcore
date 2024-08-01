using anc.domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace anc.repositories.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<Product> Product { get; set; }
}
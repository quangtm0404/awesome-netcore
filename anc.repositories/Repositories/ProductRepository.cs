using anc.applications.Repositories;
using anc.domains.Entities;
using anc.repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace anc.repositories.Repositories;
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext dbContext;
    public ProductRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task Create(Product product,
        CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(Guid Id,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Product.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);
        if (product is not null)
        {
            product.IsDeleted = true;
            await dbContext.SaveChangesAsync();
        }

    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Product
            .Where(x => x.IsDeleted == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetById(Guid Id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Product
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == Id && x.IsDeleted == false, cancellationToken);
    }

    public async Task Update(Product product,
        CancellationToken cancellationToken = default)
    {
        dbContext.Update(product);
        await dbContext.SaveChangesAsync();
    }
}
using anc.domains.Entities;

namespace anc.applications.Repositories;
public interface IProductRepository 
{
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetById(Guid Id, 
        CancellationToken cancellationToken = default);
    Task Create(Product product,
        CancellationToken cancellationToken = default);
    Task Delete(Guid Id,
        CancellationToken cancellationToken = default);
    Task Update(Product product, 
        CancellationToken cancellationToken = default);
}
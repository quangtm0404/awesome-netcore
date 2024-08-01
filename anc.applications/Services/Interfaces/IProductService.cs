using anc.domains.Entities;

namespace anc.applications.Services.Interfaces;
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task CreateAsync(Product product,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

}
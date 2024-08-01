using anc.applications.Repositories;
using anc.applications.Services.Interfaces;
using anc.domains.Entities;

namespace anc.applications.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    public ProductService(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }
    public async Task CreateAsync(Product product,
        CancellationToken cancellationToken = default)
    {
        await productRepository.Create(product,
            cancellationToken);
    }

    public async Task DeleteAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        await productRepository.Delete(id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetById(id,
            cancellationToken);
    }

    public async Task UpdateAsync(Product product,
        CancellationToken cancellationToken = default)
    {
        await productRepository.Update(product,
            cancellationToken);
    }
}
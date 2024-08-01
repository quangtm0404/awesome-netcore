using anc.applications.Repositories;
using anc.applications.Services.Interfaces;
using anc.domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace anc.applications.Services;
public class ProductService : IProductService
{
    private readonly IProductRepository productRepository;
    private readonly ICacheService cacheService;
    public ProductService(IProductRepository productRepository,
        ICacheService cacheService)
    {
        this.cacheService = cacheService;
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
        if (cacheService.IsConnected())
        {
            var value = await cacheService.GetAsync<List<Product>>("all-products",
                cancellationToken);
            if (value is null)
            {
                value = (await productRepository.GetAllAsync(cancellationToken)).ToList();
                await cacheService.SetAsync(key: "all-products",
                    value: value,
                    absoluteExpiration: 10,
                    slidingExpiration: 10,
                    cancellationToken: cancellationToken);
            }
            return value;
        }
        else
        {
            return await productRepository.GetAllAsync(cancellationToken);
        }

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
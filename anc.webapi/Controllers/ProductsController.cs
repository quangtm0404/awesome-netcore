using anc.applications.Services.Interfaces;
using anc.domains.Entities;
using Microsoft.AspNetCore.Mvc;
namespace anc.webapi.Controllers;
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService productService;
    public ProductsController(IProductService productService)
    {
        this.productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        return Ok(await productService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        Product product = new()
        {
            Name = "Demo " + Guid.NewGuid(),
            Price = 100000
        };
        await productService.CreateAsync(product);
        return Ok();
    }

}
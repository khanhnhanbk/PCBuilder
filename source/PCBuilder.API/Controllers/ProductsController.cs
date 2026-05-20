using Microsoft.AspNetCore.Mvc;
using PCBuilder.Application;
using PCBuilder.Domain;

namespace PCBuilder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Model = dto.Model,
                Price = dto.Price,
                Type = dto.Type,
                SpecsJson = dto.SpecsJson.GetRawText()
            };

            var created = await _productService.CreateAsync(product);

            return CreatedAtAction(
                nameof(Create),
                new { id = created.Id },
                created);
        }
    }
}

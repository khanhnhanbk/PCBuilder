using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products with optional filtering and pagination
        /// </summary>
        /// <param name="search">Search by product name, brand, or model</param>
        /// <param name="type">Filter by product type</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PaginatedResponse<ProductReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] string? search = null,
            [FromQuery] ProductTypeEnum? type = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            return Ok(await _productService.GetAllAsync(search, type, minPrice, maxPrice, pageNumber, pageSize));
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, _mapper.Map<ProductReadDto>(created));
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID to update</param>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            _mapper.Map(dto, product);
            await _productService.UpdateAsync(product);
            return NoContent();
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID to delete</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _productService.GetByIdAsync(id) == null)
                return NotFound();

            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}

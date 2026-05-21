using PCBuilder.Domain;

namespace PCBuilder.Application;

public interface IProductService
{
    Task<PaginatedResponse<ProductReadDto>> GetAllAsync(
        string? search = null,
        ProductTypeEnum? type = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10);

    Task<Product?> GetByIdAsync(int id);

    Task<Product> CreateAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(int id);
}

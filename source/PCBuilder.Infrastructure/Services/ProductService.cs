using Microsoft.EntityFrameworkCore;
using PCBuilder.Application;
using PCBuilder.Domain;
using PCBuilder.Infrastructure.Data;

namespace PCBuilder.Infrastructure;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedResponse<ProductReadDto>> GetAllAsync(
        string? search = null,
        ProductTypeEnum? type = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                (p.Brand != null && p.Brand.ToLower().Contains(searchLower)) ||
                (p.Model != null && p.Model.ToLower().Contains(searchLower)));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var readDtos = products.Select(p => new ProductReadDto
        {
            Id = p.Id,
            Name = p.Name,
            Brand = p.Brand,
            Model = p.Model,
            Price = p.Price,
            Type = p.Type,
            Specs = !string.IsNullOrWhiteSpace(p.SpecsJson) && p.SpecsJson != "{}"
                ? System.Text.Json.JsonSerializer.Deserialize<object>(p.SpecsJson)
                : null
        }).ToList();

        return new PaginatedResponse<ProductReadDto>
        {
            Data = readDtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _db.Products.Add(product);

        await _db.SaveChangesAsync();

        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _db.Products
            .FirstOrDefaultAsync(x => x.Id == product.Id);

        if (existing is null)
        {
            return false;
        }

        existing.Name = product.Name;
        existing.Brand = product.Brand;
        existing.Model = product.Model;
        existing.Price = product.Price;
        existing.Type = product.Type;
        existing.SpecsJson = product.SpecsJson;

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
        {
            return false;
        }

        _db.Products.Remove(product);

        await _db.SaveChangesAsync();

        return true;
    }
}
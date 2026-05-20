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

    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products
            .AsNoTracking()
            .ToListAsync();
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
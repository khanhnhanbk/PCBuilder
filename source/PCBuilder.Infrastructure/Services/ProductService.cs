using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PCBuilder.Domain;
using PCBuilder.Infrastructure.Data;

namespace PCBuilder.Infrastructure.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken ct = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<Product> CreateCpuProductAsync(string name, string? brand, decimal? price, CpuSpec cpuSpec, CancellationToken ct = default)
    {
        var product = new Product
        {
            Name = name,
            Brand = brand,
            Price = price,
            Type = ProductTypeEnum.CPU
        };

        product.SetSpecs(cpuSpec);

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<Product?> GetProductAsync(int id, CancellationToken ct = default)
    {
        return await _db.Products.FindAsync(new object[] { id }, ct);
    }
}

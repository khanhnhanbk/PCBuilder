using Microsoft.EntityFrameworkCore;
using PCBuilder.Domain;

namespace PCBuilder.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Brand).HasMaxLength(100);
            entity.Property(x => x.Model).HasMaxLength(100);
            entity.Property(x => x.Price).HasPrecision(18, 2);

            // Store all specifications as JSON column
            entity.Property(x => x.Specs)
                .HasColumnType("jsonb"); // Use "json" for SQL Server, "jsonb" for PostgreSQL
        });
    }
}

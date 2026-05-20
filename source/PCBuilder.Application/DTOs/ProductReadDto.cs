
using PCBuilder.Domain;

namespace PCBuilder.Application;
public class ProductReadDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public decimal? Price { get; set; }

    public ProductTypeEnum Type { get; set; }

    public object? Specs { get; set; }
}
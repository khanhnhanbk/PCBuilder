using System.Text.Json;
using PCBuilder.Domain;

namespace PCBuilder.Application;

public class ProductCreateDto
{
    public string Name { get; set; } = "";

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public decimal? Price { get; set; }

    public ProductTypeEnum Type { get; set; }

    public JsonElement SpecsJson { get; set; }
}
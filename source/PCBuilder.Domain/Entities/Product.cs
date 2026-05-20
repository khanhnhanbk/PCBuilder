namespace PCBuilder.Domain;

using System.Text.Json;

public class Product
{
    public int Id { get; set; }

    public ProductTypeEnum Type { get; set; }

    public string Name { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public decimal? Price { get; set; }

    // JSON storage for flexible, strongly-typed specs via manual casting
    public string SpecsJson { get; set; } = "{}";

    public T? GetSpecs<T>()
    {
        if (string.IsNullOrWhiteSpace(SpecsJson)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(SpecsJson);
        }
        catch
        {
            return default;
        }
    }

    public void SetSpecs<T>(T specs)
    {
        SpecsJson = JsonSerializer.Serialize(specs);
    }
}

namespace PCBuilder.Domain;

public class Product
{
    public int Id { get; set; }
    public ProductTypeEnum Type { get; set; }
    public string Name { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal? Price { get; set; }

    /// <summary>
    /// All product specifications stored as JSON
    /// </summary>
    public Specifications? Specs { get; set; }
}

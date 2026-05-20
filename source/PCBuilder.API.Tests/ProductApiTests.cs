using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCBuilder.Infrastructure.Data;
using Xunit;

namespace PCBuilder.API.Tests;

public class ProductApiTests : IClassFixture<ProductApiTests.CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddProduct_ReturnsCreatedForValidRequest()
    {
        var requestBody = new
        {
            name = "Test CPU",
            brand = "TestBrand",
            model = "X1000",
            price = 499.99m,
            type = "CPU",
            specsJson = new
            {
                socket = "LGA1700",
                cores = 8,
                threads = 16,
                baseClockGhz = 3.5,
                boostClockGhz = 5.1,
                tdpWatt = 125
            }
        };

        var response = await _client.PostAsJsonAsync("/api/products", requestBody);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        Assert.Equal("Test CPU", document.RootElement.GetProperty("name").GetString());
        Assert.Equal("TestBrand", document.RootElement.GetProperty("brand").GetString());
        Assert.Equal("X1000", document.RootElement.GetProperty("model").GetString());
        Assert.Equal(499.99m, document.RootElement.GetProperty("price").GetDecimal());
        Assert.Equal("CPU", document.RootElement.GetProperty("type").GetString());
        Assert.True(document.RootElement.TryGetProperty("specsJson", out _));
    }

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                using var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}

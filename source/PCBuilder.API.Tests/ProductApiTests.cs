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

    private async Task<int> CreateTestProductAsync()
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
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);
        return document.RootElement.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task GetProduct_ReturnsOkAndMatchesCreated()
    {
        var id = await CreateTestProductAsync();

        var response = await _client.GetAsync($"/api/products/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        Assert.Equal("Test CPU", document.RootElement.GetProperty("name").GetString());
        Assert.Equal("TestBrand", document.RootElement.GetProperty("brand").GetString());
        Assert.Equal("X1000", document.RootElement.GetProperty("model").GetString());
        Assert.Equal(499.99m, document.RootElement.GetProperty("price").GetDecimal());
        Assert.Equal("CPU", document.RootElement.GetProperty("type").GetString());
        Assert.True(document.RootElement.TryGetProperty("specs", out _) || document.RootElement.TryGetProperty("specsJson", out _));
    }

    [Fact]
    public async Task UpdateProduct_ReturnsNoContentAndPersistsChanges()
    {
        var id = await CreateTestProductAsync();

        var updateBody = new
        {
            name = "Updated CPU",
            brand = "TestBrand",
            model = "X2000",
            price = 599.99m,
            type = "CPU",
            specsJson = new
            {
                socket = "LGA1700",
                cores = 12,
                threads = 24,
                baseClockGhz = 3.6,
                boostClockGhz = 5.3,
                tdpWatt = 140
            }
        };

        var putResponse = await _client.PutAsJsonAsync($"/api/products/{id}", updateBody);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var content = await getResponse.Content.ReadAsStringAsync();
        var document = JsonDocument.Parse(content);

        Assert.Equal("Updated CPU", document.RootElement.GetProperty("name").GetString());
        Assert.Equal(599.99m, document.RootElement.GetProperty("price").GetDecimal());
        Assert.Equal("X2000", document.RootElement.GetProperty("model").GetString());
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNoContentAndThenNotFound()
    {
        var id = await CreateTestProductAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/products/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
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

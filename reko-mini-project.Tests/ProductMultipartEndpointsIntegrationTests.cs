using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Upload;
using reko_mini_project.Server.Features.ImageProcessing.Validation;
using reko_mini_project.Server.Features.Products;

namespace reko_mini_project.Tests;

public class ProductMultipartEndpointsIntegrationTests : IClassFixture<ProductMultipartEndpointsIntegrationTests.TestAppFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ProductMultipartEndpointsIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_WithValidData_Returns201WithMatchingFields()
    {
        using var content = BuildValidMultipartContent();

        var response = await _client.PostAsync("/api/products", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<Product>(body, JsonOptions);
        Assert.NotNull(product);
        Assert.Equal("Tomato", product.Name);
        Assert.Equal("Vegetable", product.Category);
        Assert.Equal(1.2, product.Weight);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(StubImageUploadService.FakeUrl, product.ImageUrl);
    }

    [Fact]
    public async Task CreateProduct_WithMissingRequiredFields_Returns400()
    {
        var imageContent = new ByteArrayContent([0xFF, 0xD8, 0xFF]);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        using var content = new MultipartFormDataContent
        {
            // Name and Price intentionally omitted
            { new StringContent("Vegetable"), "Category" },
            { new StringContent("1.2"), "Weight" },
            { imageContent, "FormFile", "photo.jpg" }
        };

        var response = await _client.PostAsync("/api/products", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidImageType_Returns400()
    {
        var imageContent = new ByteArrayContent([0x00, 0x11, 0x22, 0x33]);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/gif");

        using var content = new MultipartFormDataContent
        {
            { new StringContent("Tomato"), "Name" },
            { new StringContent("Vegetable"), "Category" },
            { new StringContent("A fresh tomato"), "Description" },
            { new StringContent("1.2"), "Weight" },
            { new StringContent("9.99"), "Price" },
            { imageContent, "FormFile", "photo.gif" }
        };

        var response = await _client.PostAsync("/api/products", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_WithUnknownId_Returns404()
    {
        var productId = Guid.NewGuid();
        using var content = BuildValidMultipartContent();

        var response = await _client.PutAsync($"/api/products/{productId}/with-image", content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static MultipartFormDataContent BuildValidMultipartContent()
    {
        var imageContent = new ByteArrayContent([0xFF, 0xD8, 0xFF]);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        return new MultipartFormDataContent
        {
            { new StringContent("Tomato"), "Name" },
            { new StringContent("Vegetable"), "Category" },
            { new StringContent("A fresh tomato"), "Description" },
            { imageContent, "FormFile", "photo.jpg" },
            { new StringContent("1.2"), "Weight" },
            { new StringContent("9.99"), "Price" }
        };
    }

    private sealed class StubImageUploadService : IImageUploadService
    {
        public const string FakeUrl = "https://fake.blob/test-image.jpg";

        private readonly IImageValidator _imageValidator;

        public StubImageUploadService(IImageValidator imageValidator)
        {
            _imageValidator = imageValidator;
        }

        public Task<string> StoreImageAsync(SaveImageDataRequest request, CancellationToken cancellationToken)
        {
            if (request.FormFile is null || request.FormFile.Length == 0)
                throw new ArgumentException("No image file was provided.");

            _imageValidator.Validate(request.FormFile);
            return Task.FromResult(FakeUrl);
        }

        public Task DeleteImageAsync(string blobUrl, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    public sealed class TestAppFactory : WebApplicationFactory<Program>
    {
        // A dedicated internal service provider prevents the "multiple database providers"
        // InvalidOperationException that occurs when both SQLite (server) and InMemory (test)
        // provider services are present in the same DI container.
        private static readonly IServiceProvider _inMemoryEfServiceProvider =
            new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

        public TestAppFactory()
        {
            Environment.SetEnvironmentVariable("ALLOWED_ORIGINS", "http://localhost:5173");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                           .UseInternalServiceProvider(_inMemoryEfServiceProvider);
                });

                services.RemoveAll(typeof(IImageUploadService));
                services.AddScoped<IImageUploadService, StubImageUploadService>();
            });
        }
    }
}

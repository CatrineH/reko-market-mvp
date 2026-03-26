using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using reko_mini_project.Server.Data;

namespace reko_mini_project.Tests;

public class ProductMultipartEndpointsIntegrationTests : IClassFixture<ProductMultipartEndpointsIntegrationTests.TestAppFactory>
{
    private readonly HttpClient _client;

    public ProductMultipartEndpointsIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateWithImage_ReturnsBadRequest_WhenFormFileIsMissing()
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent("Tomato"), "Name" },
            { new StringContent("1.2"), "Weight" },
            { new StringContent("9.99"), "Price" }
        };

        var response = await _client.PostAsync("/api/products/with-image", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateWithImage_ReturnsBadRequest_WhenFormFileIsMissing()
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent("Tomato"), "Name" },
            { new StringContent("1.2"), "Weight" },
            { new StringContent("9.99"), "Price" }
        };

        var productId = Guid.NewGuid();
        var response = await _client.PutAsync($"/api/products/{productId}/with-image", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public sealed class TestAppFactory : WebApplicationFactory<Program>
    {
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
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });
            });
        }
    }
}
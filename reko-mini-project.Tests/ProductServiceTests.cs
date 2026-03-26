using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.Products;
using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Update;

namespace reko_mini_project.Tests;

public class ProductServiceTests
{
    private const string TEST_ITEM_NAME = "Test Product";
    private const double TEST_ITEM_WEIGHT = 1.0;
    private const decimal TEST_ITEM_PRICE = 9.99m;

    private static ProductService CreateService(out AppDbContext dbContext)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        dbContext = new AppDbContext(options);
        return new ProductService(dbContext);
    }

    // --- Create: input validation ---

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameIsEmpty()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest("", TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameIsWhitespace()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest("   ", TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenWeightIsZero()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest(TEST_ITEM_NAME, 0.0, TEST_ITEM_PRICE);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("weight"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenPriceIsNegative()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest(TEST_ITEM_NAME, TEST_ITEM_WEIGHT, -1m);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("price"));
    }

    // --- Create: uniqueness ---

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameAlreadyExists()
    {
        var service = CreateService(out var dbContext);
        dbContext.Products.Add(
            new Product
            {
                Id = Guid.NewGuid(),
                Name = TEST_ITEM_NAME,
                ImageUrl = "",
                Weight = TEST_ITEM_WEIGHT,
                Price = TEST_ITEM_PRICE
            });
        await dbContext.SaveChangesAsync();

        var request = new CreateProductRequest(TEST_ITEM_NAME, TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);
        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenDuplicateNameDifferentCase()
    {
        var service = CreateService(out var dbContext);
        dbContext.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = TEST_ITEM_NAME.ToLower(),
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var request = new CreateProductRequest(TEST_ITEM_NAME.ToUpper(), TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);
        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    // --- Create: success ---

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WithValidInput()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest(TEST_ITEM_NAME, TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
        Assert.NotEqual(Guid.Empty, success.Product.Id);
    }

    [Fact]
    public async Task CreateAsync_TrimsNameBeforeSaving()
    {
        var service = CreateService(out _);
        var request = new CreateProductRequest("  " + TEST_ITEM_NAME + "  ", TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);

        var result = await service.CreateAsync(request, null, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
    }

    // --- Update: not found ---

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var service = CreateService(out _);
        var request = new UpdateProductRequest(TEST_ITEM_NAME, TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);

        var result = await service.UpdateAsync(Guid.NewGuid(), request, null, CancellationToken.None);

        Assert.IsType<ProductServiceResult.NotFound>(result);
    }

    // --- Update: input validation ---

    [Fact]
    public async Task UpdateAsync_ReturnsValidationError_WhenNameIsEmpty()
    {
        var service = CreateService(out var dbContext);
        var id = Guid.NewGuid();
        dbContext.Products.Add(new Product
        {
            Id = id,
            Name = TEST_ITEM_NAME,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var request = new UpdateProductRequest("", TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);
        var result = await service.UpdateAsync(id, request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    // --- Update: uniqueness ---

    [Fact]
    public async Task UpdateAsync_ReturnsValidationError_WhenNameExistsOnDifferentProduct()
    {
        var service = CreateService(out var dbContext);
        dbContext.Products.Add(new Product
        {
            Id = Guid.NewGuid(),
            Name = "Gadget",
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        var id = Guid.NewGuid();
        dbContext.Products.Add(new Product
        {
            Id = id,
            Name = TEST_ITEM_NAME,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var request = new UpdateProductRequest("Gadget", TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);
        var result = await service.UpdateAsync(id, request, null, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenKeepingOwnName()
    {
        var service = CreateService(out var dbContext);
        var id = Guid.NewGuid();
        dbContext.Products.Add(new Product
        {
            Id = id,
            Name = TEST_ITEM_NAME,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var request = new UpdateProductRequest(TEST_ITEM_NAME, TEST_ITEM_WEIGHT, TEST_ITEM_PRICE);
        var result = await service.UpdateAsync(id, request, null, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
        Assert.Equal(TEST_ITEM_PRICE, success.Product.Price);
    }

    // --- Update: success ---

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WithValidInput()
    {
        var service = CreateService(out var dbContext);
        var id = Guid.NewGuid();
        dbContext.Products.Add(new Product
        {
            Id = id,
            Name = TEST_ITEM_NAME,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var request = new UpdateProductRequest("Updated Product", 2.5, 14.99m);
        var result = await service.UpdateAsync(id, request, null, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal("Updated Product", success.Product.Name);
        Assert.Equal(2.5, success.Product.Weight);
        Assert.Equal(14.99m, success.Product.Price);
    }
}

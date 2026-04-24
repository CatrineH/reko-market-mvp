using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.Products;
using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Update;

namespace reko_mini_project.Tests;

public class ProductServiceTests
{
    private const string TEST_ITEM_NAME = "Test Product";
    private const string TEST_ITEM_CATEGORY = "Test Category";
    private const string TEST_ITEM_DESCRIPTION = "Test Description";
    private const string TEST_ITEM_IMAGE_URL = "http://example.com/image.jpg";
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
        var productWriteData = new ProductWriteData(
            "",
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
            );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenNameIsWhitespace()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            "   ",
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
            );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenWeightIsZero()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            0.0,
            TEST_ITEM_PRICE
            );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("weight"));
    }

    [Fact]
    public async Task CreateAsync_ReturnsValidationError_WhenPriceIsNegative()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            -1m
        );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

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
                Category = TEST_ITEM_CATEGORY,
                Description = TEST_ITEM_DESCRIPTION,
                ImageUrl = "",
                Weight = TEST_ITEM_WEIGHT,
                Price = TEST_ITEM_PRICE
            });

        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
            );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

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
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME.ToUpper(),
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );
        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    // --- Create: success ---

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WithValidInput()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
        Assert.NotEqual(Guid.Empty, success.Product.Id);
    }

    [Fact]
    public async Task CreateAsync_TrimsNameBeforeSaving()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            "  " + TEST_ITEM_NAME + "  ",
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );

        var result = await service.CreateAsync(productWriteData, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
    }

    // --- Update: not found ---

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        var service = CreateService(out _);
        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );

        var result = await service.UpdateAsync(Guid.NewGuid(), productWriteData, CancellationToken.None);

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
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            "",
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
            );
        var result = await service.UpdateAsync(id, productWriteData, CancellationToken.None);

        var error = Assert.IsType<ProductServiceResult.ValidationError>(result);
        Assert.True(error.Errors.ContainsKey("name"));
    }

    // --- Update: uniqueness ---

    [Fact]
    public async Task UpdateAsync_ReturnsValidationError_WhenNameExistsOnDifferentProduct()
    {
        var service = CreateService(out var dbContext);
        var product1 = new Product
        {
            Name = TEST_ITEM_NAME,
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        };
        dbContext.Products.Add(product1);
        var product2 = new Product
        {
            Name = "Another Product",
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        };
        dbContext.Products.Add(product2);

        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );
        var result = await service.UpdateAsync(product2.Id, productWriteData, CancellationToken.None);

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
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        });
        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            TEST_ITEM_NAME,
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            TEST_ITEM_WEIGHT,
            TEST_ITEM_PRICE
        );
        var result = await service.UpdateAsync(id, productWriteData, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal(TEST_ITEM_NAME, success.Product.Name);
        Assert.Equal(TEST_ITEM_PRICE, success.Product.Price);
    }

    // --- Update: success ---

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WithValidInput()
    {
        var service = CreateService(out var dbContext);
        var existingProduct = new Product
        {
            Name = TEST_ITEM_NAME,
            Category = TEST_ITEM_CATEGORY,
            Description = TEST_ITEM_DESCRIPTION,
            ImageUrl = "",
            Weight = TEST_ITEM_WEIGHT,
            Price = TEST_ITEM_PRICE
        };
        dbContext.Products.Add(existingProduct);
        await dbContext.SaveChangesAsync();

        var productWriteData = new ProductWriteData(
            "Updated Product",
            TEST_ITEM_CATEGORY,
            TEST_ITEM_DESCRIPTION,
            TEST_ITEM_IMAGE_URL,
            2.5,
            14.99m
        );
        var result = await service.UpdateAsync(existingProduct.Id, productWriteData, CancellationToken.None);

        var success = Assert.IsType<ProductServiceResult.Success>(result);
        Assert.Equal("Updated Product", success.Product.Name);
        Assert.Equal(TEST_ITEM_CATEGORY, success.Product.Category);
        Assert.Equal(2.5, success.Product.Weight);
        Assert.Equal(14.99m, success.Product.Price);
    }
}
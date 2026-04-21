using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Update;

namespace reko_mini_project.Server.Features.Products;

public class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;
    // Todo: Global exceptions and error handling middleware can be implemented in the future to 
    // eliminate the need for some of these constant error messages and validation logic in the 
    // service layer. For now, this is a simple way to centralize error messages and avoid 
    // magic strings in the code.
    private const string NAME = "name";
    private const string WEIGHT = "weight";
    private const string PRICE = "price";
    private const string DUPLICATE_PRODUCT_ERROR = "A product with this name already exists.";
    private const string WEIGHT_ERROR = "Weight must be greater than zero.";
    private const string PRICE_ERROR = "Price must be greater than zero.";
    private const string NAME_REQUIRED_ERROR = "Name is required.";

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductServiceResult> CreateAsync(ProductWriteData productWriteData, CancellationToken cancellationToken)
    {
        var errors = ValidateFields(productWriteData.Name, productWriteData.Weight, productWriteData.Price);
        if (errors.Count > 0)
        {
            return new ProductServiceResult.ValidationError(errors);
        }

        var normalizedName = productWriteData.Name.Trim();
        if (await NameExistsAsync(normalizedName, excludeId: null, cancellationToken))
        {
            return new ProductServiceResult.ValidationError(
                new Dictionary<string, string[]> { { NAME, [DUPLICATE_PRODUCT_ERROR] } });
        }

        var product = new Product
        {
            Name = normalizedName,
            Category = productWriteData.Category,
            Description = productWriteData.Description,
            ImageUrl = productWriteData.ImageUrl,
            Weight = productWriteData.Weight,
            Price = productWriteData.Price
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ProductServiceResult.Success(product);
    }

    public async Task<ProductServiceResult> UpdateAsync(Guid id, ProductWriteData productWriteData, CancellationToken cancellationToken)
    {
        var errors = ValidateFields(productWriteData.Name, productWriteData.Weight, productWriteData.Price);
        if (errors.Count > 0)
        {
            return new ProductServiceResult.ValidationError(errors);
        }

        var product = await _dbContext.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return new ProductServiceResult.NotFound();
        }

        var normalizedName = productWriteData.Name.Trim();
        var nameExists = await NameExistsAsync(normalizedName, excludeId: id, cancellationToken);
        if (nameExists)
        {
            return new ProductServiceResult.ValidationError(
                new Dictionary<string, string[]> { { NAME, [DUPLICATE_PRODUCT_ERROR] } });
        }

        product.Name = productWriteData.Name;
        product.ImageUrl = productWriteData.ImageUrl;
        product.Weight = productWriteData.Weight;
        product.Price = productWriteData.Price;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ProductServiceResult.Success(product);
    }

    private async Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken cancellationToken)
    {
        var lowerName = name.ToLower();
        var query = _dbContext.Products.Where(p => p.Name.ToLower() == lowerName);
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync(cancellationToken);
    }

    public IDictionary<string, string[]> ValidateFields(string? name, double? weight, decimal? price)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(name))
        {
            errors[NAME] = [NAME_REQUIRED_ERROR];
        }

        if (weight is null or <= 0)
        {
            errors[WEIGHT] = [WEIGHT_ERROR];
        }

        if (price is null or <= 0)
        {
            errors[PRICE] = [PRICE_ERROR];
        }

        return errors;
    }
}
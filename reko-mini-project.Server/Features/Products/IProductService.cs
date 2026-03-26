using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Update;

namespace reko_mini_project.Server.Features.Products;

public interface IProductService
{
    Task<ProductServiceResult> CreateAsync(CreateProductRequest request, string? imageUrl, CancellationToken cancellationToken);
    Task<ProductServiceResult> UpdateAsync(Guid id, UpdateProductRequest request, string? imageUrl, CancellationToken cancellationToken);
    IDictionary<string, string[]> ValidateFields(string name, double weight, decimal price);
}

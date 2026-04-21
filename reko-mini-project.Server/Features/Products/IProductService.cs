using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Update;

namespace reko_mini_project.Server.Features.Products;

public interface IProductService
{
    Task<ProductServiceResult> CreateAsync(ProductWriteData productWriteData, CancellationToken cancellationToken);
    Task<ProductServiceResult> UpdateAsync(Guid id, ProductWriteData productWriteData, CancellationToken cancellationToken);
    IDictionary<string, string[]> ValidateFields(string? name, double? weight, decimal? price);
}

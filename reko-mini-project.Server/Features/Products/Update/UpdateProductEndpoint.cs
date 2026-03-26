using reko_mini_project.Server.Features.Products;

namespace reko_mini_project.Server.Features.Products.Update;

public static class UpdateProductEndpoint
{
    private const string _endpointName = "UpdateProduct";
    private const string _summary = "Update Product by id.";

    public static RouteHandlerBuilder MapUpdateProduct(this RouteGroupBuilder group)
    {
        return group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.UpdateAsync(id, request, null, cancellationToken);

            return result switch
            {
                ProductServiceResult.Success s => Results.Ok(s.Product),
                ProductServiceResult.ValidationError e => Results.ValidationProblem(e.Errors),
                ProductServiceResult.NotFound => Results.NotFound(),
                _ => Results.StatusCode(500)
            };
        })
        .WithName(_endpointName)
        .WithSummary(_summary);
    }
}

using reko_mini_project.Server.Features.Products;

namespace reko_mini_project.Server.Features.Products.Create;

public static class CreateProductEndpoint
{
    private const string _baseRoute = "/api/products";
    private const string _routeName = "CreateProduct";
    private const string _routeSummary = "Create new product";

    public static RouteHandlerBuilder MapCreateProduct(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async (CreateProductRequest request, IProductService productService, CancellationToken cancellationToken) =>
        {
            var result = await productService.CreateAsync(request, null, cancellationToken);

            return result switch
            {
                ProductServiceResult.Success s => Results.Created($"{_baseRoute}/{s.Product.Id}", s.Product),
                ProductServiceResult.ValidationError e => Results.ValidationProblem(e.Errors),
                _ => Results.StatusCode(500)
            };
        })
        .WithName(_routeName)
        .WithSummary(_routeSummary);
    }
}

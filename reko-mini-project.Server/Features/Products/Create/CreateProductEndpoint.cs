using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Features.Products.Create;

public static class CreateInventoryItemEndpoint
{
    private const string _baseRoute = "/api/products";
    private const string _routeName = "CreateProduct";
    private const string _routeSummary = "Create new product";

    public static RouteHandlerBuilder MapCreateProduct(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async (CreateProductRequest request, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Weight = request.Weight,
                Price = request.Price
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Created($"{_baseRoute}/{product.Id}", product);
        })
        .WithName(_routeName)
        .WithSummary(_routeSummary);
    }
}

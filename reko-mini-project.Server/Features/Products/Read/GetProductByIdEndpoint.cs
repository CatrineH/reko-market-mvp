using Microsoft.AspNetCore.Http.HttpResults;
using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Features.Products.Read;

public static class GetProductByIdEndpoint
{
    private const string _route = "/{id:guid}";
    private const string _routeName = "GetProductById";
    private const string _routeSummary = "Get Product by id";
    private const string _routeDescription = "Retrieves a product by its unique identifier. Returns the product details if found, otherwise returns a 404 Not Found response.";
    private const string _routeDisplayName = "Get Product by ID";

    public static RouteHandlerBuilder MapGetProductById(this RouteGroupBuilder group)
    {
        return group.MapGet(_route, GetProductByIdHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<Product>, NotFound>> GetProductByIdHandler(
            Guid id,
            AppDbContext dbContext,
            CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([id], cancellationToken);
        return product is not null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }
}

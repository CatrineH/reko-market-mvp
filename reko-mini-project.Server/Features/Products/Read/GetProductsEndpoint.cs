using Microsoft.AspNetCore.Http.HttpResults;
using reko_mini_project.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace reko_mini_project.Server.Features.Products.Read;

public static class GetProductsEndpoint
{
    private const string _routeName = "GetProducts";
    private const string _routeSummary = "Get all products";
    private const string _routeDescription = "Retrieves a list of all products. Returns the product details if found, otherwise returns an empty list.";
    private const string _routeDisplayName = "Get All Products";

    public static RouteHandlerBuilder MapGetProducts(this RouteGroupBuilder group)
    {
        return group.MapGet("/", GetProductsHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Produces<List<Product>>(StatusCodes.Status200OK);
    }

    private static async Task<Ok<List<Product>>> GetProductsHandler(
            AppDbContext dbContext,
            CancellationToken cancellationToken)
    {
        var products = await dbContext.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(products);
    }
}

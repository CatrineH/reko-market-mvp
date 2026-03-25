using reko_mini_project.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace reko_mini_project.Server.Features.Products.Read;

public static class GetProductsEndpoint
{
    private const string _endpointName = "GetProducts";
    private const string _summary = "Get all products";

    public static RouteHandlerBuilder MapGetProducts(this RouteGroupBuilder group)
    {
        return group.MapGet("/", async (AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var products = await dbContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);

            return Results.Ok(products);
        })
            .WithName(_endpointName)
            .WithSummary(_summary);
    }
}

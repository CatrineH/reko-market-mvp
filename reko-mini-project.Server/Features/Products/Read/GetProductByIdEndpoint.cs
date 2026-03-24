using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Features.Products.Read;

public static class GetProductByIdEndpoint
{
    private const string _endpointName = "GetProductById";
    private const string _summary = "Get Product by id";

    public static RouteHandlerBuilder MapGetProductById(this RouteGroupBuilder group)
    {
        return group.MapGet("/{id:guid}", async (Guid id, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var product = await dbContext.Products.FindAsync([id], cancellationToken);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
            .WithName(_endpointName)
            .WithSummary(_summary);
    }
}

using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Features.Products.Delete;

public static class DeleteProductEndpoint
{
    private const string _endpointName = "DeleteProduct";
    private const string _summary = "Delete product by id.";

    public static RouteHandlerBuilder MapDeleteProduct(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:guid}", async (Guid id, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var product = await dbContext.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.NoContent();
        })
            .WithName(_endpointName)
            .WithSummary(_summary);
    }
}

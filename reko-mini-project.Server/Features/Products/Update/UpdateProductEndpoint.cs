using reko_mini_project.Server.Data;

namespace reko_mini_project.Server.Features.Products.Update;

public static class UpdateProductEndpoint
{
    private const string _endpointName = "UpdateProduct";
    private const string _summary = "Update Product by id.";

    public static RouteHandlerBuilder MapUpdateProduct(this RouteGroupBuilder group)
    {
        return group.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, AppDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var product = await dbContext.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            product.Name = request.Name;
            product.Weight = request.Weight;
            product.Price = request.Price;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(product);
        })
        .WithName(_endpointName)
        .WithSummary(_summary);
    }
}

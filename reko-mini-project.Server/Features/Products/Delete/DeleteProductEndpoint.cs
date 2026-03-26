using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;

namespace reko_mini_project.Server.Features.Products.Delete;

public static class DeleteProductEndpoint
{
    private const string _endpointName = "DeleteProduct";
    private const string _summary = "Delete product by id.";

    public static RouteHandlerBuilder MapDeleteProduct(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:guid}", async (
            Guid id,
            AppDbContext dbContext,
            ImageUploadService imageUploadService,
            CancellationToken cancellationToken) =>
        {
            var product = await dbContext.Products.FindAsync([id], cancellationToken);
            if (product is null)
            {
                return Results.NotFound();
            }

            var imageUrl = product.ImageUrl;

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                try
                {
                    await imageUploadService.DeleteImageAsync(imageUrl, cancellationToken);
                }
                catch
                {
                    // Blob cleanup failure is non-critical; the product was successfully deleted.
                }
            }

            return Results.NoContent();
        })
            .WithName(_endpointName)
            .WithSummary(_summary);
    }
}

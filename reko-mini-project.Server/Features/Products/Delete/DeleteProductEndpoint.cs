using Microsoft.AspNetCore.Http.HttpResults;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;

namespace reko_mini_project.Server.Features.Products.Delete;

public static class DeleteProductEndpoint
{
    private const string _route = "/{id:guid}";
    private const string _routeName = "DeleteProduct";
    private const string _routeDisplayName = "Delete Product";
    private const string _routeSummary = "Delete product by id.";
    private const string _routeDescription = "Deletes a product by its unique identifier. The endpoint checks if the product exists, removes it from the database, and also attempts to delete the associated image from blob storage if an image URL is present. It returns appropriate HTTP responses based on the outcome of the operation.";
    public static RouteHandlerBuilder MapDeleteProduct(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id:guid}", DeleteProductHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<Results<
    NoContent,
    NotFound>>
    DeleteProductHandler(
        Guid id,
        AppDbContext dbContext,
        IImageUploadService imageUploadService,
        CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return TypedResults.NotFound();
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
                // Blob cleanup failure is non-critical, the product was successfully deleted.
            }
        }

        return TypedResults.NoContent();
    }
}

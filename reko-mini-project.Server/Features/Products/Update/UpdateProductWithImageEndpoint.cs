using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Upload;
using reko_mini_project.Server.Features.Products.ErrorResponses;

namespace reko_mini_project.Server.Features.Products.Update;

public static class UpdateProductWithImageEndpoint
{
    private const string _route = "/{id:guid}/with-image";
    private const string _routeName = "UpdateProductWithImage";
    private const string _routeSummary = "Update product by id with image upload.";
    private const string _routeDescription = "Updates a product by its unique identifier with an image upload. Returns the updated product details if successful, otherwise returns an appropriate error response.";
    private const string _routeDisplayName = "Update Product by ID with Image";
    private const string _acceptedContentType = "multipart/form-data";

    public static RouteHandlerBuilder MapUpdateProductWithImage(this RouteGroupBuilder group)
    {
        return group.MapPut(_route, UpdateProductWithImageHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Accepts<ProductFormRequest>(_acceptedContentType)
            // Image uploads are typically handled by clients that may not support 
            // antiforgery tokens, such as mobile apps or third-party integrations.
            // Disabling antiforgery validation allows these clients to interact 
            // with the endpoint without requiring additional token management.
            .DisableAntiforgery();
    }

    private static async Task<Results<
        Ok<Product>,
        ValidationProblem,
        NotFound,
        BadRequest<ErrorResponse>>>
        UpdateProductWithImageHandler(
            Guid id,
            [FromForm] ProductFormRequest request,
            AppDbContext dbContext,
            IImageUploadService imageUploadService,
            IProductService productService,
            CancellationToken cancellationToken)
    {

        var existing = await dbContext.Products.FindAsync([id], cancellationToken);
        if (existing is null)
        {
            return TypedResults.NotFound();
        }

        // validate image file presence
        if (request.FormFile is null || request.FormFile.Length == 0)
        {
            return TypedResults.BadRequest(new ErrorResponse("No image file was provided."));
        }

        var oldImageUrl = existing.ImageUrl;

        try
        {
            var newImageUrl = await imageUploadService.StoreImageAsync(
                new SaveImageDataRequest(request.FormFile),
                cancellationToken);

            var updateCommand = new ProductWriteData(
                request.Name ?? string.Empty,
                request.Category ?? string.Empty,
                request.Description ?? string.Empty,
                newImageUrl,
                request.Weight ?? 0,
                request.Price ?? 0
                );

            var result = await productService.UpdateAsync(id, updateCommand, cancellationToken);

            if (result is not ProductServiceResult.Success success)
            {
                return result switch
                {
                    ProductServiceResult.ValidationError e => TypedResults.ValidationProblem(e.Errors),
                    ProductServiceResult.NotFound => TypedResults.NotFound(),
                    _ => throw new UnreachableException()
                };
            }

            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                try
                {
                    await imageUploadService.DeleteImageAsync(oldImageUrl, cancellationToken);
                }
                catch
                {
                    // Product update has been successful, but blob cleanup failed.
                    // Log the error and continue without interrupting the user flow.
                    // In a production application, consider implementing a retry mechanism
                    // or a background job to handle orphaned blobs that failed to delete.
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Failed to delete old image at URL: {oldImageUrl}");
                    Console.ResetColor();
                }
            }

            return TypedResults.Ok(success.Product);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
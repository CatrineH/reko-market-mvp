using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;

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
            .DisableAntiforgery();
    }

    private static async Task<Results<
        Ok<Product>,
        ValidationProblem,
        NotFound>>
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
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    { SaveImageDataRequest.FormFileFieldName, [SaveImageDataRequest.FormFileRequiredMessage] }
                });
        }

        var fieldErrors = productService.ValidateFields(
            request.Name,
            request.Category,
            request.Description,
            request.Weight,
            request.Price
            );

        if (fieldErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(fieldErrors);
        }

        var oldImageUrl = existing.ImageUrl;

        var newImageUrl = await imageUploadService.StoreImageAsync(
            new SaveImageDataRequest(request.FormFile),
            cancellationToken);

        var updateCommand = new ProductWriteData(
            request.Name,
            request.Category,
            request.Description,
            newImageUrl,
            request.Weight,
            request.Price
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
}
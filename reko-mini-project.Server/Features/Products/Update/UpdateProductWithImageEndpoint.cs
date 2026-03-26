using Microsoft.AspNetCore.Mvc;
using reko_mini_project.Server.Data;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Upload;

namespace reko_mini_project.Server.Features.Products.Update;

public static class UpdateProductWithImageEndpoint
{
    private const string _endpointName = "UpdateProductWithImage";
    private const string _summary = "Update product by id with image upload.";

    public static RouteHandlerBuilder MapUpdateProductWithImage(this RouteGroupBuilder group)
    {
        return group.MapPut("/{id:guid}/with-image", async (
            Guid id,
            [FromForm] UpdateProductWithImageRequest request,
            AppDbContext dbContext,
            ImageUploadService imageUploadService,
            IProductService productService,
            CancellationToken cancellationToken) =>
        {
            var fieldErrors = productService.ValidateFields(request.Name, request.Weight, request.Price);
            if (fieldErrors.Count > 0)
            {
                return Results.ValidationProblem(fieldErrors);
            }

            var existing = await dbContext.Products.FindAsync([id], cancellationToken);
            if (existing is null)
            {
                return Results.NotFound();
            }

            var oldImageUrl = existing.ImageUrl;

            try
            {
                var newImageUrl = await imageUploadService.StoreImageAsync(
                    new SaveImageDataRequest(request.FormFile),
                    cancellationToken);

                var updateRequest = new UpdateProductRequest(
                    request.Name,
                    request.Weight,
                    request.Price);

                var result = await productService.UpdateAsync(id, updateRequest, newImageUrl, cancellationToken);

                if (result is not ProductServiceResult.Success success)
                {
                    return result switch
                    {
                        ProductServiceResult.ValidationError e => Results.ValidationProblem(e.Errors),
                        ProductServiceResult.NotFound => Results.NotFound(),
                        _ => Results.StatusCode(500)
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
                        // Blob cleanup failure is non-critical; the product update succeeded.
                    }
                }

                return Results.Ok(success.Product);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName(_endpointName)
        .WithSummary(_summary)
        .Accepts<UpdateProductWithImageRequest>("multipart/form-data")
        .DisableAntiforgery();
    }
}
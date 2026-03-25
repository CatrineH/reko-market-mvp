using Microsoft.AspNetCore.Mvc;
using reko_mini_project.Server.Features.ImageProcessing.Services;

namespace reko_mini_project.Server.Features.ImageProcessing.Upload;

public static class ImageUploadEndpoint
{
    private const string _routeName = "UploadImage";
    private const string _routeSummary = "Upload image";

    public static RouteHandlerBuilder MapImageUploadEndpoint(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async (
            [FromForm] SaveImageDataRequest request,
            ImageUploadService imageUploadService,
            CancellationToken cancellationToken
            ) =>
        {
            try
            {
                var blobUrl = await imageUploadService.StoreImageAsync(request, cancellationToken);

                return Results.Ok(new
                {
                    imageUrl = blobUrl
                });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName(_routeName)
        .WithSummary(_routeSummary)
        .Accepts<SaveImageDataRequest>("multipart/form-data")
        .DisableAntiforgery();
    }
}
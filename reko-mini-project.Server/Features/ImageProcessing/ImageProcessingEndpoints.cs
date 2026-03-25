using reko_mini_project.Server.Features.ImageProcessing.Upload;

namespace reko_mini_project.Server.Features.ImageProcessing;

public static class ImageProcessingEndpoints
{
    private const string _baseRoute = "/api/image-processing";
    private const string _groupTag1 = "ImageProcessing";

    public static IEndpointRouteBuilder MapImageProcessingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(_baseRoute)
            .WithTags(_groupTag1);

        group.MapImageUploadEndpoint();

        return app;
    }
}
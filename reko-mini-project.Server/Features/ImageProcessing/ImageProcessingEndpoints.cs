namespace reko_mini_project.Server.Features.ImageProcessing;

public static class ImageProcessingEndpoints
{
    private const string _baseRoute = "/api/image-processing";
    private const string _groupTag1 = "Image Processing";

    public static IEndpointRouteBuilder MapImageProcessingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(_baseRoute)
            .WithTags(_groupTag1);

        group.MapImageAnalysis().RequireAuthorization("SupplierPolicy");

        return app;
    }
}
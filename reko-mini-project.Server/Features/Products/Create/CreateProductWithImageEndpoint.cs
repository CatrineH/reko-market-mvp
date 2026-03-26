using Microsoft.AspNetCore.Mvc;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Upload;

namespace reko_mini_project.Server.Features.Products.Create;

public static class CreateProductWithImageEndpoint
{
    private const string _baseRoute = "/api/products";
    private const string _routeName = "CreateProductWithImage";
    private const string _routeSummary = "Create new product with image upload";

    public static RouteHandlerBuilder MapCreateProductWithImage(this RouteGroupBuilder group)
    {
        return group.MapPost("/with-image", async (
            [FromForm] CreateProductWithImageRequest request,
            ImageUploadService imageUploadService,
            IProductService productService,
            CancellationToken cancellationToken) =>
        {
            var fieldErrors = productService.ValidateFields(request.Name, request.Weight, request.Price);
            if (fieldErrors.Count > 0)
            {
                return Results.ValidationProblem(fieldErrors);
            }

            try
            {
                var imageUrl = await imageUploadService.StoreImageAsync(
                    new SaveImageDataRequest(request.FormFile),
                    cancellationToken);

                var createRequest = new CreateProductRequest(
                    request.Name,
                    request.Weight,
                    request.Price);

                var result = await productService.CreateAsync(createRequest, imageUrl, cancellationToken);

                return result switch
                {
                    ProductServiceResult.Success s => Results.Created($"{_baseRoute}/{s.Product.Id}", s.Product),
                    ProductServiceResult.ValidationError e => Results.ValidationProblem(e.Errors),
                    _ => Results.StatusCode(500)
                };
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName(_routeName)
        .WithSummary(_routeSummary)
        .Accepts<CreateProductWithImageRequest>("multipart/form-data")
        .DisableAntiforgery();
    }
}
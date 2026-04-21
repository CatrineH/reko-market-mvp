using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Upload;
using reko_mini_project.Server.Features.Products.ErrorResponses;

namespace reko_mini_project.Server.Features.Products.Create;

public static class CreateProductEndpoint
{
    private const string _baseRoute = "/api/products";
    private const string _routeName = "CreateProduct";
    private const string _routeSummary = "Create new product";
    private const string _routeDisplayName = "Create Product";
    private const string _format = "multipart/form-data";
    private const string _routeDescription = "Creates a new product with product details. The product image is stored in blob storage, and the product is created with the provided details and the URL of the uploaded image. The endpoint handles validation errors and returns appropriate HTTP responses for each scenario.";
    public static RouteHandlerBuilder MapCreateProduct(this RouteGroupBuilder group)
    {
        return group.MapPost("/", CreateProductHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Accepts<ProductFormRequest>(_format)
            .Produces<Product>(StatusCodes.Status201Created)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest)
            .Produces<BadRequest<ErrorResponse>>(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private static async Task<Results<
            Created<Product>,
            ValidationProblem,
            BadRequest<ErrorResponse>>>
            CreateProductHandler(
                [FromForm] ProductFormRequest request,
                IImageUploadService imageUploadService,
                IProductService productService,
                CancellationToken cancellationToken)
    {
        var fieldErrors = productService.ValidateFields(request.Name, request.Weight, request.Price);
        if (fieldErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(fieldErrors);
        }

        try
        {
            var imageUrl = await imageUploadService.StoreImageAsync(
                new SaveImageDataRequest(request.FormFile),
                cancellationToken);

            var productWriteData = new ProductWriteData(
                request.Name ?? string.Empty,
                request.Category ?? string.Empty,
                request.Description ?? string.Empty,
                imageUrl,
                request.Weight ?? 0,
                request.Price ?? 0
            );

            var result = await productService.CreateAsync(productWriteData, cancellationToken);

            return result switch
            {
                ProductServiceResult.Success s =>
                    TypedResults.Created($"{_baseRoute}/{s.Product.Id}", s.Product),
                ProductServiceResult.ValidationError e =>
                    TypedResults.ValidationProblem(e.Errors),
                _ => throw new UnreachableException()
            };
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;

namespace reko_mini_project.Server.Features.Products.Update;

public static class UpdateProductEndpoint
{
    private const string _route = "/{id:guid}";
    private const string _routeName = "UpdateProduct";
    private const string _routeSummary = "Update Product by id.";
    private const string _routeDescription = "Updates a product by its unique identifier. Returns the updated product details if successful, otherwise returns an appropriate error response.";
    private const string _routeDisplayName = "Update Product by ID";


    public static RouteHandlerBuilder MapUpdateProduct(this RouteGroupBuilder group)
    {
        return group.MapPut(_route, UpdateProductHandler)
            .WithName(_routeName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .WithDisplayName(_routeDisplayName)
            .Accepts<ProductWriteData>("application/json")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces<ValidationProblem>(StatusCodes.Status400BadRequest)
            .Produces<NotFound>(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<
        Ok<Product>,
        ValidationProblem,
        NotFound>>
        UpdateProductHandler(
            Guid id,
            ProductWriteData updateRequest,
            IProductService productService,
            CancellationToken cancellationToken)
    {
        var fieldErrors = productService.ValidateFields(updateRequest.Name, updateRequest.Weight, updateRequest.Price);
        if (fieldErrors.Count > 0)
        {
            return TypedResults.ValidationProblem(fieldErrors);
        }

        var result = await productService.UpdateAsync(id, updateRequest, cancellationToken);

        return result switch
        {
            ProductServiceResult.Success s => TypedResults.Ok(s.Product),
            ProductServiceResult.ValidationError e => TypedResults.ValidationProblem(e.Errors),
            ProductServiceResult.NotFound => TypedResults.NotFound(),
            _ => throw new UnreachableException()
        };
    }
}

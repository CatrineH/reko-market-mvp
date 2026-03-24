namespace reko_mini_project.Server.Features.Products.Read;

public static class GetAllProductsEndpoint
{
    private const string _endpointName = "GetAllProducts";
    private const string _summary = "Get all products";

    public static RouteHandlerBuilder MapGetAllProducts(this RouteGroupBuilder group)
    {
        //TODO: Implement actual logic to get products from database
        return group.MapGet("/", () => Results.Ok("Hello Reko!"))
                    .WithName(_endpointName)
                    .WithSummary(_summary);
    }
}
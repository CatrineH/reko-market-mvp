using reko_mini_project.Server.Features.Products.Read;

namespace reko_mini_project.Server.Features.Products;

public static class ProductEndpoints
{
    private const string _baseRoute = "/api/products";
    private const string _groupTag1 = "Products";

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(_baseRoute)
            .WithTags(_groupTag1);

        group.MapGetAllProducts();

        return app;
    }
}
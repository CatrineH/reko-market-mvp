using reko_mini_project.Server.Configurations;
using reko_mini_project.Server.Features.Products.Create;
using reko_mini_project.Server.Features.Products.Read;
using reko_mini_project.Server.Features.Products.Update;
using reko_mini_project.Server.Features.Products.Delete;

namespace reko_mini_project.Server.Features.Products;

public static class ProductEndpoints
{
    private const string _baseRoute = "/api/products";
    private const string _groupTag1 = "Products";

    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(_baseRoute)
            .WithTags(_groupTag1)
            .RequireRateLimiting(RateLimiterExtensions.WritePolicyName);

        group.MapGetProducts()
            .RequireAuthorization(AuthorizationExtensions.Policies.READ_GLOBAL_PRODUCTS);
        group.MapGetProductById()
            .RequireAuthorization(AuthorizationExtensions.Policies.READ_GLOBAL_PRODUCTS);
        group.MapCreateProduct()
            .RequireAuthorization(AuthorizationExtensions.Policies.WRITE_GLOBAL_PRODUCTS);
        group.MapUpdateProduct()
            .RequireAuthorization(AuthorizationExtensions.Policies.WRITE_GLOBAL_PRODUCTS);
        group.MapUpdateProductWithImage()
            .RequireAuthorization(AuthorizationExtensions.Policies.WRITE_GLOBAL_PRODUCTS);
        group.MapDeleteProduct()
            .RequireAuthorization(AuthorizationExtensions.Policies.WRITE_GLOBAL_PRODUCTS);

        return app;
    }
}
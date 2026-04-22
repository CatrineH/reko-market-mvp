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
            .WithTags(_groupTag1);

        group.MapGetProducts().RequireAuthorization("SupplierPolicy");
        group.MapGetProductById().RequireAuthorization("SupplierPolicy");
        group.MapCreateProduct().RequireAuthorization("SupplierPolicy");
        group.MapUpdateProduct().RequireAuthorization("AdminPolicy");
        group.MapUpdateProductWithImage().RequireAuthorization("AdminPolicy");
        group.MapDeleteProduct().RequireAuthorization("AdminPolicy");

        return app;
    }
}
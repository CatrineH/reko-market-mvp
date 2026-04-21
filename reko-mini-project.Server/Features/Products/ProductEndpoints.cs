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

        group.MapGetProducts();
        group.MapGetProductById();
        group.MapCreateProduct();
        group.MapUpdateProduct();
        group.MapUpdateProductWithImage();
        group.MapDeleteProduct();

        return app;
    }
}
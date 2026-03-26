namespace reko_mini_project.Server.Features.Products.Create;

public sealed record CreateProductWithImageRequest(
    string Name,
    double Weight,
    decimal Price,
    IFormFile? FormFile
    );
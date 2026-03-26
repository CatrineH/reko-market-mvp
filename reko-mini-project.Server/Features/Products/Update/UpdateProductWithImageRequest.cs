namespace reko_mini_project.Server.Features.Products.Update;

public sealed record UpdateProductWithImageRequest(
    string Name,
    double Weight,
    decimal Price,
    IFormFile? FormFile
    );
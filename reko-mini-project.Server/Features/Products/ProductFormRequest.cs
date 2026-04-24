namespace reko_mini_project.Server.Features.Products;

// Data Transfer Object at web layer for 
// receiving product creation requests with form data.
public sealed record ProductFormRequest(
    string Name,
    string Category,
    string Description,
    IFormFile FormFile,
    double Weight,
    decimal Price
    );
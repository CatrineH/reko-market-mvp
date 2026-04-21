namespace reko_mini_project.Server.Features.Products;

//Data Transfer Object at service layer for passing 
//product data during create and update operations.
public sealed record ProductWriteData(
    string Name,
    string Category,
    string Description,
    string ImageUrl,
    double Weight,
    decimal Price
    );
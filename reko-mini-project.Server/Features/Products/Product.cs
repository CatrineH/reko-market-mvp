namespace reko_mini_project.Server.Features.Products;

public sealed class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public decimal Price { get; set; }
}
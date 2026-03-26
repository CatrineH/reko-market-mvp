namespace reko_mini_project.Server.Features.Products;

public abstract record ProductServiceResult
{
    public sealed record Success(Product Product) : ProductServiceResult;
    public sealed record ValidationError(IDictionary<string, string[]> Errors) : ProductServiceResult;
    public sealed record NotFound : ProductServiceResult;
}
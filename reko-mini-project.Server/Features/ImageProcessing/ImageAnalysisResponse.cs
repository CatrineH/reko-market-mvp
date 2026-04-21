namespace reko_mini_project.Server.Features.ImageProcessing;

public sealed record AnalyzeImageResponse(
    string? Category,
    string? Name,
    double? Weight,
    decimal? Price
);

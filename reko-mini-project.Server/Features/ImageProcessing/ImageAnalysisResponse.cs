namespace reko_mini_project.Server.Features.ImageProcessing;

public sealed record ImageAnalysisResponse(
    string? Category,
    string? Name,
    double? Weight,
    decimal? Price
);

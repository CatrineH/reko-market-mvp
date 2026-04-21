namespace reko_mini_project.Server.Features.ImageProcessing;

public sealed record AnalyzeImageRequest(
    IFormFile? FormFile
);
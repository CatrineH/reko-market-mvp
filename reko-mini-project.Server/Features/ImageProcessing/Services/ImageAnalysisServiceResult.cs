namespace reko_mini_project.Server.Features.ImageProcessing;

public abstract record ImageAnalysisServiceResult
{
    public sealed record Success(AnalyzeImageResponse Response) : ImageAnalysisServiceResult;
    public sealed record ValidationError(IDictionary<string, string[]> Errors) : ImageAnalysisServiceResult;
    public sealed record NotFound : ImageAnalysisServiceResult;
}
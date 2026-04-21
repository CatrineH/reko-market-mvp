namespace reko_mini_project.Server.Features.ImageProcessing.Exceptions;

public sealed record ImageAnalysisErrorResponse(string Error, string? RawAiResponse);
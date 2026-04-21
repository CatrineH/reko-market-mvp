namespace reko_mini_project.Server.Features.ImageProcessing.Exceptions;

internal class AIAnalysisNotConfiguredException : Exception
{
    public AIAnalysisNotConfiguredException()
        : base("AI analysis service is not configured, image analysis cannot be performed.")
    {
    }

    public AIAnalysisNotConfiguredException(string message)
        : base(message)
    {
    }

    public AIAnalysisNotConfiguredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
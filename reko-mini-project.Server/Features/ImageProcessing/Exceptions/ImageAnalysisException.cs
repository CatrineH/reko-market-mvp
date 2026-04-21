namespace reko_mini_project.Server.Features.ImageProcessing.Exceptions;

public class ImageAnalysisException : Exception
{
    public string? RawResponse { get; }

    public ImageAnalysisException(string message, string? rawResponse)
        : base(message)
    {
        RawResponse = rawResponse;
    }
}

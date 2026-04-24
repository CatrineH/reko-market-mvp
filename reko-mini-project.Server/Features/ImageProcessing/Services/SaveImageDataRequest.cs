namespace reko_mini_project.Server.Features.ImageProcessing.Services;

public sealed record SaveImageDataRequest(IFormFile? FormFile)
{
    public const string FormFileFieldName = "formFile";
    public const string FormFileRequiredMessage = "No image file was provided.";
};
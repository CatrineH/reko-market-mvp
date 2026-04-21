namespace reko_mini_project.Server.Features.ImageProcessing.Services;

public interface IImageUploadService
{
    Task<string> StoreImageAsync(SaveImageDataRequest request, CancellationToken cancellationToken);
    Task DeleteImageAsync(string blobUrl, CancellationToken cancellationToken);
}

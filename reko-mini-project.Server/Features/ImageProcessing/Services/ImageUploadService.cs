using Azure.Storage.Blobs;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using reko_mini_project.Server.Features.ImageProcessing.Upload;
using reko_mini_project.Server.Features.ImageProcessing.Validation;

namespace reko_mini_project.Server.Features.ImageProcessing.Services;

public class ImageUploadService : IImageUploadService
{
    private readonly IImageValidator _imageValidator;
    private readonly BlobStorageOptions _blobStorageOptions;

    public ImageUploadService(IImageValidator imageValidator, IOptions<BlobStorageOptions> blobStorageOptions)
    {
        _imageValidator = imageValidator;
        _blobStorageOptions = blobStorageOptions.Value;
    }

    /// <summary>
    /// Validates and uploads the provided image file to Azure Blob Storage, returning the URL of the uploaded image.
    /// </summary>
    /// <param name="request">The request containing the image file to be uploaded.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The URL of the uploaded image.</returns>
    public async Task<string> StoreImageAsync(SaveImageDataRequest request, CancellationToken cancellationToken)
    {

        if (request.FormFile is null || request.FormFile.Length == 0)
            throw new ArgumentException("No image file was provided.");

        _imageValidator.Validate(request.FormFile);

        var containerClient = await GetContainerClientAsync(cancellationToken);
        var blobClient = containerClient.GetBlobClient(BuildBlobFileName(request.FormFile));
        await UploadFileAsync(blobClient, request.FormFile, cancellationToken);
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Deletes the image with the specified URL from Azure Blob Storage.
    /// </summary>
    /// <param name="blobUrl">The URL of the image to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    public async Task DeleteImageAsync(string blobUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
        {
            return;
        }

        var containerClient = await GetContainerClientAsync(cancellationToken);
        var blobName = new Uri(blobUrl).Segments.Last();
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }



    private async Task<BlobContainerClient> GetContainerClientAsync(CancellationToken cancellationToken)
    {
        var containerName = GetConfiguredContainerName();
        var blobServiceClient = CreateBlobServiceClient();
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
        return containerClient;
    }

    private string GetConfiguredContainerName()
    {
        if (string.IsNullOrWhiteSpace(_blobStorageOptions.ContainerName))
            throw new InvalidOperationException("BlobStorage:ContainerName is not configured.");

        return _blobStorageOptions.ContainerName;
    }

    private static string BuildBlobFileName(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        return $"{Guid.NewGuid():N}{extension}";
    }

    private static async Task UploadFileAsync(BlobClient blobClient, IFormFile file, CancellationToken cancellationToken)
    {
        await using var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(fileStream, BuildUploadOptions(file), cancellationToken);
    }

    private static BlobUploadOptions BuildUploadOptions(IFormFile file)
    {
        return new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            }
        };
    }

    private BlobServiceClient CreateBlobServiceClient()
    {
        var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2021_12_02);

        // Development scenario: use connection string
        if (!string.IsNullOrWhiteSpace(_blobStorageOptions.ConnectionString))
            return new BlobServiceClient(_blobStorageOptions.ConnectionString, options);

        // Production scenario: use managed identity with service URI
        if (Uri.TryCreate(_blobStorageOptions.ServiceUri, UriKind.Absolute, out var serviceUri))
            return new BlobServiceClient(serviceUri, new DefaultAzureCredential(), options);

        // If neither configuration is valid, throw an exception
        throw new InvalidOperationException(
            "Configure either BlobStorage:ConnectionString for development or BlobStorage:ServiceUri for managed-identity based environments.");
    }
}
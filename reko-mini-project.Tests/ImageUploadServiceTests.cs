using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Validation;

namespace reko_mini_project.Tests;

public class ImageUploadServiceTests
{
    [Fact]
    public async Task StoreImageAsync_ThrowsWhenFormFileIsNull()
    {
        var validator = new RecordingValidator();
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ContainerName = "images",
                ConnectionString = "UseDevelopmentStorage=true"
            });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(null), CancellationToken.None));

        Assert.Contains("No image file", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, validator.CallCount);
    }

    [Fact]
    public async Task StoreImageAsync_ThrowsWhenFormFileIsEmpty()
    {
        var validator = new RecordingValidator();
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ContainerName = "images",
                ConnectionString = "UseDevelopmentStorage=true"
            });

        var file = CreateFormFile("empty.jpg", "image/jpeg", []);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(file), CancellationToken.None));

        Assert.Contains("No image file", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, validator.CallCount);
    }

    [Fact]
    public async Task StoreImageAsync_CallsValidatorWithProvidedFile()
    {
        var validator = new RecordingValidator();
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ConnectionString = "UseDevelopmentStorage=true"
            });

        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(file), CancellationToken.None));

        Assert.Equal(1, validator.CallCount);
        Assert.Same(file, validator.LastValidatedFile);
    }

    [Fact]
    public async Task StoreImageAsync_PropagatesValidatorException()
    {
        var validator = new ThrowingValidator(new ArgumentException("validator failed"));
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ContainerName = "images",
                ConnectionString = "UseDevelopmentStorage=true"
            });

        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(file), CancellationToken.None));

        Assert.Contains("validator failed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task StoreImageAsync_ThrowsWhenContainerNameMissing()
    {
        var validator = new RecordingValidator();
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ConnectionString = "UseDevelopmentStorage=true"
            });

        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(file), CancellationToken.None));

        Assert.Contains("ContainerName", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, validator.CallCount);
    }

    [Fact]
    public async Task StoreImageAsync_ThrowsWhenBlobClientConfigurationIsMissing()
    {
        var validator = new RecordingValidator();
        var service = CreateService(
            validator,
            new BlobStorageOptions
            {
                ContainerName = "images",
                ConnectionString = "",
                ServiceUri = "not-a-valid-absolute-uri"
            });

        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.StoreImageAsync(new SaveImageDataRequest(file), CancellationToken.None));

        Assert.Contains("Configure either BlobStorage:ConnectionString", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, validator.CallCount);
    }

    private static ImageUploadService CreateService(IImageValidator validator, BlobStorageOptions options)
    {
        return new ImageUploadService(validator, Options.Create(options));
    }

    private static IFormFile CreateFormFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, stream.Length, "formFile", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private sealed class RecordingValidator : IImageValidator
    {
        public int CallCount { get; private set; }
        public IFormFile? LastValidatedFile { get; private set; }

        public void Validate(IFormFile file)
        {
            CallCount++;
            LastValidatedFile = file;
        }
    }

    private sealed class ThrowingValidator(Exception exception) : IImageValidator
    {
        public void Validate(IFormFile file)
        {
            throw exception;
        }
    }
}

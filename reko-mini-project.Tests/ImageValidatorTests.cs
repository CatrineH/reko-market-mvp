using Microsoft.AspNetCore.Http;
using reko_mini_project.Server.Features.ImageProcessing.Validation;

namespace reko_mini_project.Tests;

public class ImageValidatorTests
{
    private const int MaxFileSizeBytes = 1_572_864;

    [Fact]
    public void Validate_AllowsValidJpeg()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        var ex = Record.Exception(() => validator.Validate(file));

        Assert.Null(ex);
    }

    [Fact]
    public void Validate_AllowsValidPng()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.png", "image/png", [0x89, 0x50, 0x4E, 0x47]);

        var ex = Record.Exception(() => validator.Validate(file));

        Assert.Null(ex);
    }

    [Fact]
    public void Validate_AllowsValidWebP()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.webp", "image/webp", [
            0x52, 0x49, 0x46, 0x46,
            0x00, 0x00, 0x00, 0x00,
            0x57, 0x45, 0x42, 0x50
        ]);

        var ex = Record.Exception(() => validator.Validate(file));

        Assert.Null(ex);
    }

    [Fact]
    public void Validate_AllowsFileAtMaxSizeBoundary()
    {
        var validator = new ImageValidator();
        var bytes = new byte[MaxFileSizeBytes];
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        bytes[2] = 0xFF;
        var file = CreateFormFile("photo.jpeg", "image/jpeg", bytes);

        var ex = Record.Exception(() => validator.Validate(file));

        Assert.Null(ex);
    }

    [Fact]
    public void Validate_ThrowsWhenFileExceedsMaxSize()
    {
        var validator = new ImageValidator();
        var bytes = new byte[MaxFileSizeBytes + 1];
        bytes[0] = 0xFF;
        bytes[1] = 0xD8;
        bytes[2] = 0xFF;
        var file = CreateFormFile("photo.jpg", "image/jpeg", bytes);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("maximum allowed size", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ThrowsWhenMimeTypeIsNotAllowed()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.jpg", "image/gif", [0xFF, 0xD8, 0xFF]);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("not allowed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ThrowsWhenExtensionIsNotAllowed()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.gif", "image/jpeg", [0xFF, 0xD8, 0xFF]);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("extension", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ThrowsWhenFileIsTooSmallForMagicBytes()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.jpg", "image/jpeg", [0xFF, 0xD8]);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("too small", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ThrowsWhenMagicBytesDoNotMatchClaimedFormat()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.jpg", "image/jpeg", [0x00, 0x11, 0x22, 0x33]);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("does not match", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_ThrowsForWebPWhenHeaderIsOnlyElevenBytes()
    {
        var validator = new ImageValidator();
        var file = CreateFormFile("photo.webp", "image/webp", [
            0x52, 0x49, 0x46, 0x46,
            0x00, 0x00, 0x00, 0x00,
            0x57, 0x45, 0x42
        ]);

        var ex = Assert.Throws<ArgumentException>(() => validator.Validate(file));

        Assert.Contains("does not match", ex.Message, StringComparison.OrdinalIgnoreCase);
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
}

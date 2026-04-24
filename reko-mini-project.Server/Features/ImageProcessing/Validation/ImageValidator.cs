namespace reko_mini_project.Server.Features.ImageProcessing.Validation;

public class ImageValidator : IImageValidator
{
    // TODO: Should be heavily reduced, perhaps 200-300 KB
    // for better performance and to avoid sending large files to the AI model. 
    // This will require resizing the image on the client side before upload.
    private const long MaxFileSizeBytes = 1_572_864; // 1.5 MB

    // TODO: Should only allow WebP format for better compression and performance.
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    // TODO: Should only allow WebP format for better compression and performance.
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public void Validate(IFormFile file)
    {

        ValidateFileSize(file);
        ValidateFileType(file);
        ValidateFileExtension(file);
        ValidateMagicBytes(file);
    }

    private static void ValidateFileSize(IFormFile file)
    {
        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException($"Image size {file.Length} bytes exceeds the maximum allowed size of {MaxFileSizeBytes} bytes.");
    }

    private static void ValidateFileType(IFormFile file)
    {
        if (!AllowedMimeTypes.Contains(file.ContentType))
            throw new ArgumentException($"Image type '{file.ContentType}' is not allowed. Allowed types: {string.Join(", ", AllowedMimeTypes)}.");
    }

    private static void ValidateFileExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}.");
    }

    private static void ValidateMagicBytes(IFormFile file)
    {
        Span<byte> header = stackalloc byte[12];
        using var stream = file.OpenReadStream();
        var bytesRead = stream.Read(header);

        if (bytesRead < 3)
            throw new ArgumentException("File is too small to be a valid image.");

        if (IsJpeg(header) || IsPng(header) || (bytesRead >= 12 && IsWebP(header)))
            return;

        throw new ArgumentException("File content does not match a recognised image format (JPEG, PNG, or WebP).");
    }

    // TODO: Should use a more robust image validation library for better security and reliability, 
    // such as ImageSharp or Magick.NET, which can handle various edge cases and provide more comprehensive validation.
    private static bool IsJpeg(Span<byte> h) =>
        h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF;

    private static bool IsPng(Span<byte> h) =>
        h.Length >= 4 && h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47;

    private static bool IsWebP(Span<byte> h) =>
        h[0] == 0x52 && h[1] == 0x49 && h[2] == 0x46 && h[3] == 0x46 && // "RIFF"
        h[8] == 0x57 && h[9] == 0x45 && h[10] == 0x42 && h[11] == 0x50;  // "WEBP"
}
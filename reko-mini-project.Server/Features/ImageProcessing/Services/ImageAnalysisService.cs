using System.Text.Json;
using System.Text.RegularExpressions;
using OpenAI.Chat;
using reko_mini_project.Server.Features.ImageProcessing.Exceptions;
using reko_mini_project.Server.Features.ImageProcessing.Validation;

namespace reko_mini_project.Server.Features.ImageProcessing.Services;

public class ImageAnalysisService
{
    private readonly ChatClient _chatClient;
    private readonly IImageValidator _imageValidator;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string AI_ERROR_EMPTY_RESPONSE = "AI returned an empty response.";
    private const string AI_ERROR_RESPONSE_NULL = "AI response deserialized to null.";
    private const string AI_ERROR_INVALID_JSON = "AI response was not valid JSON.";

    private const string SYSTEM_PROMPT =
        """
            Analyze images of food products. 
            Return ONLY JSON.
            Use Norwegian. Keep values short.
            Format:
            {"category":"","product_name":"","estimated_weight":0,"suggested_price_nok":0}
            Rules:
            - Fill all fields
            - Weight in grams (number)
            - Price in NOK (number)
        """;

    public ImageAnalysisService(ChatClient chatClient, IImageValidator imageValidator)
    {
        _chatClient = chatClient;
        _imageValidator = imageValidator;
    }

    /// <summary>
    /// Analyzes the provided image using the AI model and returns the analysis result.
    /// </summary>
    /// <param name="formFile">The image file to be analyzed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The result of the image analysis.</returns>
    /// <exception cref="AIAnalysisNotConfiguredException">Thrown when the AI analysis is not configured.</exception>
    public async Task<ImageAnalysisResult> AnalyzeImageFromFormFileAsync(IFormFile formFile, CancellationToken cancellationToken)
    {
        if (_chatClient is null)
        {
            throw new AIAnalysisNotConfiguredException();
        }

        if (formFile is null || formFile.Length == 0)
        {
            throw new ArgumentException("No image file was provided.");
        }

        _imageValidator.Validate(formFile);

        var binaryImageData = await GetBinaryDataFromFormFile(formFile);
        ChatCompletion chatCompletion = await GetChatCompletionAsync(_chatClient, binaryImageData, cancellationToken);
        return GetAnalysisResult(chatCompletion);
    }

    private static async Task<BinaryData> GetBinaryDataFromFormFile(IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return BinaryData.FromBytes(memoryStream.ToArray());
    }

    private static async Task<ChatCompletion> GetChatCompletionAsync(ChatClient chatClient, BinaryData binaryImageData, CancellationToken cancellationToken)
    {
        return await chatClient.CompleteChatAsync(
            [
                new SystemChatMessage(SYSTEM_PROMPT),
                new UserChatMessage(
                    // TODO: We may end up using webp exclusively. If so, the content type should be updated accordingly.
                    ChatMessageContentPart.CreateImagePart(binaryImageData, "image/jpeg")
                ),
            ],
            cancellationToken: cancellationToken);
    }

    private static ImageAnalysisResult GetAnalysisResult(ChatCompletion completion)
    {
        var rawText = completion.Content[0].Text
                    ?? throw new ImageAnalysisException(AI_ERROR_EMPTY_RESPONSE, rawResponse: null);

        var json = StripCodeFences(rawText);

        try
        {
            return JsonSerializer.Deserialize<ImageAnalysisResult>(json, JsonOptions)
                ?? throw new ImageAnalysisException(AI_ERROR_RESPONSE_NULL, rawText);
        }
        catch (JsonException)
        {
            throw new ImageAnalysisException(AI_ERROR_INVALID_JSON, rawText);
        }
    }

    private static string StripCodeFences(string text)
    {
        var trimmed = text.Trim();
        return Regex.Replace(trimmed, @"^```(?:json)?\s*|\s*```$", "", RegexOptions.Multiline).Trim();
    }
}
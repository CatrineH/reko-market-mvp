using System.Text.Json.Serialization;

namespace reko_mini_project.Server.Features.ImageProcessing.Services;

public sealed record ImageAnalysisResult(
    [property: JsonPropertyName("product name")] string ProductName,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("estimated_weight")] double EstimatedWeight,
    [property: JsonPropertyName("suggested_price_nok")] decimal SuggestedPriceNok
);

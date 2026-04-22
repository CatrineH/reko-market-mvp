using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using reko_mini_project.Server.Features.Products.ErrorResponses;
using reko_mini_project.Server.Features.ImageProcessing.Services;
using reko_mini_project.Server.Features.ImageProcessing.Exceptions;
using reko_mini_project.Server.Features.ImageProcessing.Validation;

namespace reko_mini_project.Server.Features.ImageProcessing;

public static class AnalyzeImageEndpoint
{
    private const string _routeName = "AnalyzeProductImage";
    private const string _routeDisplayName = "Analyze Product Image";
    private const string _routeSummary = "Analyze an uploaded image to extract product information.";
    private const string _routeDescription =
        """
            Analyzes an uploaded image to extract product information. 
            The image is processed using AI to extract product details.
            The endpoint handles validation 
            errors and returns appropriate HTTP responses for each scenario.
        """;

    public static RouteHandlerBuilder MapImageAnalysis(this RouteGroupBuilder group)
    {
        return group.MapPost("/analyze", AnalyzeProductImageHandler)
            .WithName(_routeName)
            .WithDisplayName(_routeDisplayName)
            .WithSummary(_routeSummary)
            .WithDescription(_routeDescription)
            .DisableAntiforgery();
    }

    private static async Task<Results<
        Created<ImageAnalysisResponse>,
        ValidationProblem,
        BadRequest<ErrorResponse>,
        UnprocessableEntity<ImageAnalysisErrorResponse>>>
        AnalyzeProductImageHandler(
            [FromForm] ImageAnalysisRequest request,
            ImageAnalysisService imageAnalysisService,
            CancellationToken cancellationToken)
    {
        if (request.FormFile is null || request.FormFile.Length == 0)
        {
            return TypedResults.BadRequest(new ErrorResponse("No image file was provided."));
        }

        ImageAnalysisResult analysisResult;
        try
        {
            analysisResult = await imageAnalysisService.AnalyzeImageFromFormFileAsync(request.FormFile, cancellationToken);
        }
        catch (ImageAnalysisException ex)
        {
            return TypedResults.UnprocessableEntity(new ImageAnalysisErrorResponse(ex.Message, ex.RawResponse));
        }

        var response = new ImageAnalysisResponse(
            analysisResult.Category,
            analysisResult.ProductName,
            analysisResult.EstimatedWeight,
            analysisResult.SuggestedPriceNok);

        var analysisErrors = ImageAnalysisResponseValidator.Validate(response);

        return analysisErrors.Count > 0
            ? TypedResults.ValidationProblem(analysisErrors)
            : TypedResults.Created("/api/products/analyzed", response);
    }
}

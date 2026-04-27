using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using reko_mini_project.Server.Features.ImageProcessing.Exceptions;

namespace reko_mini_project.Server.ExceptionHandlers;

internal sealed class ProblemExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ProblemExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            BadHttpRequestException e        => (e.StatusCode,                             "Bad Request",           e.Message),
            ArgumentException e              => (StatusCodes.Status400BadRequest,           "Bad Request",           e.Message),
            InvalidDataException e           => (StatusCodes.Status400BadRequest,           "Invalid Request Data",  e.Message),
            AIAnalysisNotConfiguredException => (StatusCodes.Status503ServiceUnavailable,   "Service Unavailable",   "Image analysis service is not currently available."),
            _                               => (StatusCodes.Status500InternalServerError,  "An unexpected error occurred", (string?)null)
        };

        if (status >= 500)
            logger.LogError(exception, "Unhandled exception occurred");
        else
            logger.LogWarning(exception, "Request error {Status}: {Message}", status, exception.Message);

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = status
            }
        });
    }
}

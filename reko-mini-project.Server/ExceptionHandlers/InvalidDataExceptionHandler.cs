using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace reko_mini_project.Server.ExceptionHandlers;

internal sealed class InvalidDataExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<InvalidDataExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not InvalidDataException) return false;

        logger.LogWarning(exception, "Invalid multipart data: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Invalid request data",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            }
        });
    }
}

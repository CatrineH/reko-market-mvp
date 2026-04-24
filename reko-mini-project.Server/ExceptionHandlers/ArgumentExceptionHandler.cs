using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace reko_mini_project.Server.ExceptionHandlers;

internal sealed class ArgumentExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ArgumentExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ArgumentException) return false;

        logger.LogWarning(exception, "Invalid argument: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            }
        });
    }
}

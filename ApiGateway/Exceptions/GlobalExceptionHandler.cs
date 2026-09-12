using Microsoft.AspNetCore.Diagnostics;

namespace ApiGateway.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception in ApiGateway.");

        var (statusCode, message) = exception switch
        {
            HttpRequestException =>
                (
                    StatusCodes.Status502BadGateway,
                    "One of the backend services is currently unavailable."
                ),

            TaskCanceledException =>
                (
                    StatusCodes.Status504GatewayTimeout,
                    "A backend service took too long to respond."
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred."
                )
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                statusCode,
                message
            },
            cancellationToken);

        return true;
    }
}
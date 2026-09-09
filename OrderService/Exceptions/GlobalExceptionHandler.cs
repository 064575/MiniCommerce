using Microsoft.AspNetCore.Diagnostics;

namespace OrderService.Exceptions;

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
        _logger.LogError(exception, "Unhandled exception occurred.");

        var (statusCode, message) = exception switch
        {
            ArgumentException =>
                (StatusCodes.Status400BadRequest, exception.Message),

            InvalidOperationException =>
                (StatusCodes.Status409Conflict, exception.Message),

            HttpRequestException =>
                (StatusCodes.Status502BadGateway,
                 "Communication with another service failed."),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "An unexpected error occurred.")
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
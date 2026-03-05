using Microsoft.AspNetCore.Diagnostics;

namespace Claims.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode;
        string message;

        switch (exception)
        {
            case ArgumentException ex:
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;
            default:
                _logger.LogError(exception, "Unhandled exception");
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            message
        }, cancellationToken);

        return true;
    }
}
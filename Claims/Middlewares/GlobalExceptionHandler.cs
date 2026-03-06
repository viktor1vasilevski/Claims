using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Claims.Api.Middlewares;

/// <summary>
/// Handles exceptions globally and returns appropriate HTTP responses.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    /// <summary>
    /// Tries to handle the exception and write the response.
    /// </summary>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int statusCode;
        string message;

        switch (exception)
        {
            case CoverNotFoundException ex:
                _logger.LogWarning(ex, "Cover not found: {Message}", ex.Message);
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
                break;

            case ClaimDateOutOfRangeException ex:
                _logger.LogWarning(ex, "Claim date validation failed: {Message}", ex.Message);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case CoverHasActiveClaimsException ex:
                _logger.LogWarning(ex, "Cover has active claims: {Message}", ex.Message);
                statusCode = StatusCodes.Status409Conflict;
                message = ex.Message;
                break;

            case PremiumStrategyNotFoundException ex:
                _logger.LogWarning(ex, "Premium strategy not found: {Message}", ex.Message);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case UnhandledAuditEntityTypeException ex:
                _logger.LogError(ex, "Unhandled audit entity type: {Message}", ex.Message);
                statusCode = StatusCodes.Status500InternalServerError;
                message = ex.Message;
                break;

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
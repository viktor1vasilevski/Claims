using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Middlewares;

/// <summary>
/// Handles exceptions globally and returns appropriate HTTP responses.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
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
                logger.LogWarning(ex, "Cover not found: {Message}", ex.Message);
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
                break;

            case ClaimDateOutOfRangeException ex:
                logger.LogWarning(ex, "Claim date validation failed: {Message}", ex.Message);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case CoverHasActiveClaimsException ex:
                logger.LogWarning(ex, "Cover has active claims: {Message}", ex.Message);
                statusCode = StatusCodes.Status409Conflict;
                message = ex.Message;
                break;

            case PremiumStrategyNotFoundException ex:
                logger.LogWarning(ex, "Premium strategy not found: {Message}", ex.Message);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case ClaimNotFoundException ex:
                logger.LogWarning(ex, "Claim not found: {Message}", ex.Message);
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
                break;

            case ArgumentOutOfRangeException ex:
                logger.LogError(ex, "Argument out of range: {Message}", ex.Message);
                statusCode = StatusCodes.Status500InternalServerError;
                message = ex.Message;
                break;

            default:
                logger.LogError(exception, "Unhandled exception");
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Detail = message
            }
        });
    }
}

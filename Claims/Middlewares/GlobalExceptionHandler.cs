using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
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
                logger.LogWarning(ex, "Cover not found: {Message} | {Method} {Path} | TraceId: {TraceId}", 
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
                break;

            case ClaimDateOutOfRangeException ex:
                logger.LogWarning(ex, "Claim date validation failed: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case CoverHasActiveClaimsException ex:
                logger.LogWarning(ex, "Cover has active claims: {Message} | {Method} {Path} | TraceId: {TraceId}", 
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status409Conflict;
                message = ex.Message;
                break;

            case PremiumStrategyNotFoundException ex:
                logger.LogWarning(ex, "Premium strategy not found: {Message} | {Method} {Path} | TraceId: {TraceId}", 
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case ClaimNotFoundException ex:
                logger.LogWarning(ex, "Claim not found: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status404NotFound;
                message = ex.Message;
                break;

            case InvalidClaimNameException ex:
                logger.LogWarning(ex, "Invalid claim name: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case InvalidDamageCostException ex:
                logger.LogWarning(ex, "Invalid damage cost: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case InvalidPremiumException ex:
                logger.LogWarning(ex, "Invalid premium: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case InvalidCoverPeriodException ex:
                logger.LogWarning(ex, "Invalid cover period: {Message} | {Method} {Path} | TraceId: {TraceId}",
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status400BadRequest;
                message = ex.Message;
                break;

            case UnhandledAuditEntityTypeException ex:
                logger.LogError(ex, "Unhandled audit entity type: {Message} | {Method} {Path} | TraceId: {TraceId}", 
                    ex.Message, httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
                statusCode = StatusCodes.Status500InternalServerError;
                message = ex.Message;
                break;

            default:
                logger.LogError(exception, "Unhandled exception | {Method} {Path} | TraceId: {TraceId}", 
                    httpContext.Request.Method, httpContext.Request.Path, httpContext.TraceIdentifier);
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

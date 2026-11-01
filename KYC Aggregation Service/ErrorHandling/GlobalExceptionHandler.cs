using KYC_Aggregation_Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace KYC_Aggregation_Service.ErrorHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            InvalidCustomerRequestException =>
                (StatusCodes.Status400BadRequest,
                    "The provided customer request is invalid."),

            CustomerNotFoundException =>
                (StatusCodes.Status404NotFound,
                    "Customer data not found for the provided SSN."),

            RateLimitExceededException rateLimitException =>
                HandleRateLimit(httpContext, rateLimitException),

            _ =>
                HandleUnexpectedError(httpContext, exception)
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new { error = message },
            cancellationToken);

        return true;
    }

    private static (int StatusCode, string Message) HandleRateLimit(
        HttpContext httpContext,
        RateLimitExceededException exception)
    {
        if (exception.RetryAfterSeconds is { } retryAfterSeconds)
        {
            httpContext.Response.Headers.RetryAfter =
                retryAfterSeconds.ToString();
        }

        return (
            StatusCodes.Status429TooManyRequests,
            "The Customer Data API rate limit was exceeded.");
    }

    private (int StatusCode, string Message) HandleUnexpectedError(
        HttpContext httpContext,
        Exception exception)
    {
        logger.LogError(
            exception,
            "An unexpected error occurred while processing {Method} {Path}.",
            httpContext.Request.Method,
            httpContext.Request.Path);

        return (
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred while processing the request.");
    }
}
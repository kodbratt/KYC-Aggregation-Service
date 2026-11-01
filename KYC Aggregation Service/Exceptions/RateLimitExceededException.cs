namespace KYC_Aggregation_Service.Exceptions;

public sealed class RateLimitExceededException : Exception
{
    public RateLimitExceededException(
        int? retryAfterSeconds = null)
        : base("The Customer Data API rate limit was exceeded.")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }

    public RateLimitExceededException(
        int? retryAfterSeconds,
        Exception innerException)
        : base(
            "The Customer Data API rate limit was exceeded.",
            innerException)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }

    public int? RetryAfterSeconds { get; }
}
using System.Net;

namespace KYC_Aggregation_Service.Exceptions
{
    public sealed class CustomerDataApiException : Exception
    {
        public CustomerDataApiException(
            string message,
            HttpStatusCode statusCode,
            int? retryAfterSeconds = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            RetryAfterSeconds = retryAfterSeconds;
        }

        public HttpStatusCode StatusCode { get; }

        public int? RetryAfterSeconds { get; }
    }
}

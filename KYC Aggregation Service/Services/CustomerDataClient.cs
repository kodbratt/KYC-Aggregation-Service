using KYC_Aggregation_Service.Exceptions;
using KYC_Aggregation_Service.Models;
using System.Net;

namespace KYC_Aggregation_Service.Services
{
    public class CustomerDataClient(HttpClient httpClient) : ICustomerDataClient
    {
        public Task<PersonalDetails> GetPersonalDetailsAsync(string ssn, CancellationToken cancellationToken = default)
        {
            return GetAsync<PersonalDetails>(
               $"personal-details/{Uri.EscapeDataString(ssn)}",
               cancellationToken);
        }
        public Task<ContactDetails> GetContactDetailsAsync(string ssn, CancellationToken cancellationToken = default)
        {
            return GetAsync<ContactDetails>(
                $"contact-details/{Uri.EscapeDataString(ssn)}",
                cancellationToken);
        }
        public Task<KYCForm> GetKycFormAsync(string ssn, CancellationToken cancellationToken = default)
        {
            return GetAsync<KYCForm>(
                $"kyc-form/{Uri.EscapeDataString(ssn)}",
                cancellationToken);
        }

        private async Task<T> GetAsync<T>(
            string relativeUrl,
            CancellationToken cancellationToken)
        {
            using var response = await httpClient.GetAsync(
                relativeUrl,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new InvalidCustomerRequestException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new CustomerNotFoundException();
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                int? retryAfterSeconds = response.Headers.RetryAfter?.Delta is { } delay
                    ? Math.Max(1, (int)delay.TotalSeconds)
                    : null;

                throw new RateLimitExceededException(retryAfterSeconds);
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<T>(
                cancellationToken);

            return result
                ?? throw new InvalidOperationException(
                    $"The Customer Data API returned an empty response for '{relativeUrl}'.");
        }
    }
}

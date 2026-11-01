using KYC_Aggregation_Service.Models;

namespace KYC_Aggregation_Service.Services
{
    public interface ICustomerDataClient
    {
        Task<PersonalDetails> GetPersonalDetailsAsync(
            string ssn,
            CancellationToken cancellationToken = default);

        Task<ContactDetails> GetContactDetailsAsync(
            string ssn,
            CancellationToken cancellationToken = default);

        Task<KYCForm> GetKycFormAsync(
            string ssn,
            CancellationToken cancellationToken = default);
    }
}

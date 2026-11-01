using KYC_Aggregation_Service.Models;
using KYC_Aggregation_Service.Persistence;
using KYC_Aggregation_Service.Persistence.Entities;
using System.Globalization;

namespace KYC_Aggregation_Service.Services
{
    public class KycAggregationService(ICustomerDataClient customerDataClient, IAggregatedKycStore aggregatedKycStore) : IKycAggregationService
    {
        public async Task<AggregatedKycData?> GetBySsnAsync(
           string ssn,
           CancellationToken cancellationToken = default)
        {
            var storedEntity = await aggregatedKycStore.GetBySsnAsync(
                ssn,
                cancellationToken);

            if (storedEntity is not null)
            {
                return ToModel(storedEntity);
            }

            var personalDetailsTask =
                customerDataClient.GetPersonalDetailsAsync(
                    ssn,
                    cancellationToken);

            var contactDetailsTask =
                customerDataClient.GetContactDetailsAsync(
                    ssn,
                    cancellationToken);

            var kycFormTask =
                customerDataClient.GetKycFormAsync(
                    ssn,
                    cancellationToken);

            await Task.WhenAll(
                personalDetailsTask,
                contactDetailsTask,
                kycFormTask);

            var personalDetails = await personalDetailsTask;
            var contactDetails = await contactDetailsTask;
            var kycForm = await kycFormTask;

            var entity = new AggregatedKycEntity
            {
                Ssn = ssn,
                FirstName = personalDetails.FirstName,
                LastName = personalDetails.SurName,
                Address = personalDetails.Address,
                PhoneNumber = GetPhoneNumber(contactDetails),
                Email = GetEmail(contactDetails),
                TaxCountry = GetKycValue(kycForm, "tax_country"),
                Income = GetIncome(kycForm)
            };

            await aggregatedKycStore.SaveAsync(
                entity,
                cancellationToken);

            return ToModel(entity);
        }

        private static AggregatedKycData ToModel(
            AggregatedKycEntity entity)
        {
            return new AggregatedKycData
            {
                Ssn = entity.Ssn,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Address = entity.Address,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                TaxCountry = entity.TaxCountry,
                Income = entity.Income
            };
        }

        private static string? GetPhoneNumber(
            ContactDetails contactDetails)
        {
            return contactDetails.PhoneNumbers
                       .FirstOrDefault(phone => phone.Preferred)?
                       .Number
                   ?? contactDetails.PhoneNumbers
                       .FirstOrDefault()?
                       .Number;
        }

        private static string? GetEmail(
            ContactDetails contactDetails)
        {
            return contactDetails.Emails
                       .FirstOrDefault(email => email.Preferred)?
                       .EmailAddress
                   ?? contactDetails.Emails
                       .FirstOrDefault()?
                       .EmailAddress;
        }

        private static string? GetKycValue(
            KYCForm kycForm,
            string key)
        {
            return kycForm.Items
                .FirstOrDefault(item =>
                    string.Equals(
                        item.Key,
                        key,
                        StringComparison.OrdinalIgnoreCase))
                ?.Value;
        }

        private static int? GetIncome(KYCForm kycForm)
        {
            var incomeValue = GetKycValue(
                kycForm,
                "annual_income");

            return int.TryParse(
                incomeValue,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var income)
                ? income
                : null;
        }
    }
}
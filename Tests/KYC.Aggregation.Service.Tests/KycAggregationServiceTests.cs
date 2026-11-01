using KYC_Aggregation_Service.Models;
using KYC_Aggregation_Service.Persistence;
using KYC_Aggregation_Service.Persistence.Entities;
using KYC_Aggregation_Service.Services;
using Xunit;

namespace KYC.Aggregation.Service.Tests;

public sealed class KycAggregationServiceTests
{
    [Fact]
    public async Task GetBySsnAsync_WhenDataIsStored_ReturnsStoredDataWithoutCallingCustomerApi()
    {
        const string ssn = "19800115-1234";
        var store = new FakeAggregatedKycStore
        {
            StoredEntity = new AggregatedKycEntity
            {
                Ssn = ssn,
                FirstName = "Lars",
                LastName = "Larsson",
                Address = "Smågatan 1",
                PhoneNumber = "+46 70 123 45 67",
                Email = "lars.larsson@example.com",
                TaxCountry = "SE",
                Income = 550000
            }
        };
        var customerDataClient = new FakeCustomerDataClient();
        var service = new KycAggregationService(customerDataClient, store);

        var result = await service.GetBySsnAsync(ssn);

        Assert.NotNull(result);
        Assert.Equal(ssn, result.Ssn);
        Assert.Equal("Lars", result.FirstName);
        Assert.Equal("SE", result.TaxCountry);
        Assert.Equal(0, customerDataClient.CallCount);
        

    }

    [Fact]
    public async Task GetBySsnAsync_WhenDataIsNotStored_AggregatesAndStoresCustomerData()
    {
        const string ssn = "19900220-5678";
        var store = new FakeAggregatedKycStore();
        var customerDataClient = new FakeCustomerDataClient
        {
            PersonalDetails = new PersonalDetails
            {
                FirstName = "Sven",
                SurName = "Svensson",
                Address = "Storgatan 1"
            },
            ContactDetails = new ContactDetails
            {
                Emails =
                [
                    new Email
                    {
                        Preferred = true,
                        EmailAddress = "sven.svensson@example.com"
                    }
                ],
                PhoneNumbers =
                [
                    new PhoneNumber
                    {
                        Preferred = true,
                        Number = "+46 70 123 45 67"
                    }
                ]
            },
            KycForm = new KYCForm
            {
                Items =
                [
                    new KYCItem { Key = "tax_country", Value = "SE" },
                    new KYCItem { Key = "annual_income", Value = "550000" }
                ]
            }
        };
        var service = new KycAggregationService(customerDataClient, store);

        var result = await service.GetBySsnAsync(ssn);

        Assert.NotNull(result);
        Assert.Equal(ssn, result.Ssn);
        Assert.Equal("Sven", result.FirstName);
        Assert.Equal("Svensson", result.LastName);
        Assert.Equal("Storgatan 1", result.Address);
        Assert.Equal("+46 70 123 45 67", result.PhoneNumber);
        Assert.Equal("sven.svensson@example.com", result.Email);
        Assert.Equal("SE", result.TaxCountry);
        Assert.Equal(550000, result.Income);
        Assert.NotNull(store.SavedEntity);
        Assert.Equal(ssn, store.SavedEntity.Ssn);
        Assert.Equal("SE", store.SavedEntity.TaxCountry);
        Assert.Equal(550000, store.SavedEntity.Income);
        Assert.Equal(3, customerDataClient.CallCount);
    }

    private sealed class FakeAggregatedKycStore : IAggregatedKycStore
    {
        public AggregatedKycEntity? StoredEntity { get; init; }

        public AggregatedKycEntity? SavedEntity { get; private set; }

        public Task<AggregatedKycEntity?> GetBySsnAsync(
            string ssn,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(StoredEntity);
        }

        public Task SaveAsync(
            AggregatedKycEntity entity,
            CancellationToken cancellationToken)
        {
            SavedEntity = entity;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeCustomerDataClient : ICustomerDataClient
    {
        public PersonalDetails PersonalDetails { get; init; } = new();

        public ContactDetails ContactDetails { get; init; } = new();

        public KYCForm KycForm { get; init; } = new();

        public int CallCount { get; private set; }

        public Task<PersonalDetails> GetPersonalDetailsAsync(
            string ssn,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(PersonalDetails);
        }

        public Task<ContactDetails> GetContactDetailsAsync(
            string ssn,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(ContactDetails);
        }

        public Task<KYCForm> GetKycFormAsync(
            string ssn,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(KycForm);
        }
    }
}

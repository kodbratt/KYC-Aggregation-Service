using KYC_Aggregation_Service.Models;
using KYC_Aggregation_Service.Persistence;
using KYC_Aggregation_Service.Persistence.Entities;
using KYC_Aggregation_Service.Services;
using Xunit;

namespace KYC.Aggregation.Service.Tests;

public sealed class KycAggregationServiceTests
{
    private FakeCustomerDataClient customerClient = null!;

    [Fact]
    public async Task GetBySsnAsync_WhenDataIsStored_ReturnsStoredDataWithoutCallingCustomerApi()
    {
        const string ssn = "19800115-1234";
        var store = new FakeAggregatedKycStore(
            new AggregatedKycEntity
            {
                Ssn = ssn,
                FirstName = "Lars",
                LastName = "Larsson",
                Address = "Smågatan 1",
                PhoneNumber = "+46 70 123 45 67",
                Email = "lars.larsson@example.com",
                TaxCountry = "SE",
                Income = 550000
            });

        var customerDataClient = new FakeCustomerDataClient();
        customerClient = customerDataClient;

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
        customerClient = customerDataClient;

        var service = new KycAggregationService(customerDataClient, store);
        var result = await service.GetBySsnAsync(ssn);

        Assert.NotNull(result);
        Assert.Equal("Sven", result!.FirstName);
        Assert.Equal("SE", result.TaxCountry);
        Assert.Equal(550000, result.Income);
        Assert.Equal(3, customerClient.CallCount);
        Assert.NotNull(store.SavedEntity);
        Assert.Equal(ssn, store.SavedEntity!.Ssn);
    }

    private sealed class FakeAggregatedKycStore : IAggregatedKycStore
    {
        public FakeAggregatedKycStore(AggregatedKycEntity? storedEntity = null)
        {
            StoredEntity = storedEntity;
        }

        public AggregatedKycEntity? StoredEntity { get; set; }
        public AggregatedKycEntity? SavedEntity { get; private set; }

        public Task<AggregatedKycEntity?> GetBySsnAsync(string ssn, CancellationToken cancellationToken)
            => Task.FromResult(StoredEntity);

        public Task SaveAsync(AggregatedKycEntity entity, CancellationToken cancellationToken)
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

        public Task<PersonalDetails> GetPersonalDetailsAsync(string ssn, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(PersonalDetails);
        }

        public Task<ContactDetails> GetContactDetailsAsync(string ssn, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(ContactDetails);
        }

        public Task<KYCForm> GetKycFormAsync(string ssn, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(KycForm);
        }
    }
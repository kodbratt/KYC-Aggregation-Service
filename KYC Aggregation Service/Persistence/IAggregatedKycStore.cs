using KYC_Aggregation_Service.Persistence.Entities;

namespace KYC_Aggregation_Service.Persistence
{
    public interface IAggregatedKycStore
    {
        Task<AggregatedKycEntity?> GetBySsnAsync(
            string ssn,
            CancellationToken cancellationToken);

        Task SaveAsync(
            AggregatedKycEntity entity,
            CancellationToken cancellationToken);
    }
}

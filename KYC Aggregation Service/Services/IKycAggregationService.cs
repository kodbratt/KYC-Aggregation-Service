using KYC_Aggregation_Service.Models;

namespace KYC_Aggregation_Service.Services
{
    public interface IKycAggregationService
    {
       Task<AggregatedKycData?> GetBySsnAsync(string ssn, CancellationToken cancellationToken = default);
    }
}

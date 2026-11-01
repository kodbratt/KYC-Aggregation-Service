using KYC_Aggregation_Service.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KYC_Aggregation_Service.Persistence
{
    public sealed class EfAggregatedKycStore(KycDbContext dbContext) : IAggregatedKycStore
    {
        public async Task<AggregatedKycEntity?> GetBySsnAsync(
            string ssn,
            CancellationToken cancellationToken)
        {
            return await dbContext.AggregatedKycData
                .SingleOrDefaultAsync(
                    item => item.Ssn == ssn,
                    cancellationToken);
        }

        public async Task SaveAsync(
           AggregatedKycEntity entity,
           CancellationToken cancellationToken)
        {
            dbContext.AggregatedKycData.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

using KYC_Aggregation_Service.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace KYC_Aggregation_Service.Persistence;

public class KycDbContext(DbContextOptions<KycDbContext> options)
    : DbContext(options)
{
    public DbSet<AggregatedKycEntity> AggregatedKycData =>
        Set<AggregatedKycEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AggregatedKycEntity>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.HasIndex(item => item.Ssn)
                .IsUnique();

            entity.Property(item => item.Ssn)
                .IsRequired()
                .HasMaxLength(20);
        });
    }
}

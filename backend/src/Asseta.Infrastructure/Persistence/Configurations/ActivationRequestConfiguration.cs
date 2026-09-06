using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActivationRequestConfiguration : IEntityTypeConfiguration<ActivationRequest>
{
    public void Configure(EntityTypeBuilder<ActivationRequest> builder)
    {
        builder.ToTable("activation_requests");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(r => r.TriggerSource)
            .HasColumnName("trigger_source")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ActivationTriggerSource>(v, true))
            .IsRequired();

        builder.Property(r => r.InitiatedByTrustedPersonId)
            .HasColumnName("initiated_by_trusted_person_id");

        builder.Property(r => r.Reason)
            .HasColumnName("reason")
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ActivationRequestStatus>(v, true))
            .IsRequired();

        builder.Property(r => r.GracePeriodExpiresAtUtc)
            .HasColumnName("grace_period_expires_at_utc")
            .IsRequired();

        builder.Property(r => r.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(r => r.ActivatedAtUtc)
            .HasColumnName("activated_at_utc");

        builder.Property(r => r.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(r => r.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(r => r.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasMany(r => r.Confirmations)
            .WithOne()
            .HasForeignKey(c => c.ActivationRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.OwnerId, r.Status });
    }
}

using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class OwnerActivationConfigConfiguration : IEntityTypeConfiguration<OwnerActivationConfig>
{
    public void Configure(EntityTypeBuilder<OwnerActivationConfig> builder)
    {
        builder.ToTable("owner_activation_configs");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(c => c.CheckInIntervalDays)
            .HasColumnName("check_in_interval_days")
            .HasDefaultValue(30)
            .IsRequired();

        builder.Property(c => c.GracePeriodHours)
            .HasColumnName("grace_period_hours")
            .HasDefaultValue(48)
            .IsRequired();

        builder.Property(c => c.MinConfirmationsRequired)
            .HasColumnName("min_confirmations_required")
            .HasDefaultValue(1)
            .IsRequired();

        builder.Property(c => c.LastCheckInAtUtc)
            .HasColumnName("last_check_in_at_utc")
            .IsRequired();

        builder.Property(c => c.NextCheckInDueUtc)
            .HasColumnName("next_check_in_due_utc")
            .IsRequired();

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<HeartbeatStatus>(v, true))
            .IsRequired();

        builder.Property(c => c.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasIndex(c => c.OwnerId)
            .IsUnique();

        builder.HasIndex(c => new { c.NextCheckInDueUtc, c.Status });
    }
}

using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class TrustedPersonPairingCodeConfiguration : IEntityTypeConfiguration<TrustedPersonPairingCode>
{
    public void Configure(EntityTypeBuilder<TrustedPersonPairingCode> builder)
    {
        builder.ToTable("trusted_person_pairing_codes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.TrustedPersonId)
            .HasColumnName("trusted_person_id")
            .IsRequired();

        builder.Property(c => c.CodeHash)
            .HasColumnName("code_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Salt)
            .HasColumnName("salt")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(c => c.FailedAttempts)
            .HasColumnName("failed_attempts")
            .IsRequired();

        builder.Property(c => c.LockoutUntil)
            .HasColumnName("lockout_until");

        builder.Property(c => c.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder.Property(c => c.IsUsed)
            .HasColumnName("is_used")
            .IsRequired();

        builder.Property(c => c.UsedAt)
            .HasColumnName("used_at");

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.Property(c => c.DeletedAtUtc)
            .HasColumnName("deleted_at");

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.HasIndex(c => new { c.TrustedPersonId, c.IsUsed, c.ExpiresAt });
    }
}

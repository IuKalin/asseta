using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActivationConfirmationConfiguration : IEntityTypeConfiguration<ActivationConfirmation>
{
    public void Configure(EntityTypeBuilder<ActivationConfirmation> builder)
    {
        builder.ToTable("activation_confirmations");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.ActivationRequestId)
            .HasColumnName("activation_request_id")
            .IsRequired();

        builder.Property(c => c.TrustedPersonId)
            .HasColumnName("trusted_person_id")
            .IsRequired();

        builder.Property(c => c.IsConfirmed)
            .HasColumnName("is_confirmed")
            .IsRequired();

        builder.Property(c => c.Note)
            .HasColumnName("note")
            .HasMaxLength(500);

        builder.Property(c => c.ConfirmedAtUtc)
            .HasColumnName("confirmed_at_utc")
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at_utc");

        builder.HasIndex(c => new { c.ActivationRequestId, c.TrustedPersonId })
            .IsUnique();
    }
}

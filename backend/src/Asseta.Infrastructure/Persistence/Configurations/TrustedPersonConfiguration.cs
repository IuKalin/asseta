using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class TrustedPersonConfiguration : IEntityTypeConfiguration<TrustedPerson>
{
    public void Configure(EntityTypeBuilder<TrustedPerson> builder)
    {
        builder.ToTable("trusted_people");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(t => t.DelegateUserId)
            .HasColumnName("delegate_user_id")
            .IsRequired(false);

        builder.Property(t => t.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Relationship)
            .HasColumnName("relationship")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.RoleDescription)
            .HasColumnName("role_description")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(t => t.TrustLevel)
            .HasColumnName("trust_level")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TrustedPersonStatus>(v, true))
            .IsRequired();

        builder.Property(t => t.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.Property(t => t.DeletedAtUtc)
            .HasColumnName("deleted_at");

        builder.Property(t => t.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.HasMany(t => t.PairingCodes)
            .WithOne(c => c.TrustedPerson)
            .HasForeignKey(c => c.TrustedPersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Permissions)
            .WithOne(p => p.TrustedPerson)
            .HasForeignKey(p => p.TrustedPersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.OwnerId, t.IsDeleted });
    }
}

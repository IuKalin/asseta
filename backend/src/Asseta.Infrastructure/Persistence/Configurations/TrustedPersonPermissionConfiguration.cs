using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class TrustedPersonPermissionConfiguration : IEntityTypeConfiguration<TrustedPersonPermission>
{
    public void Configure(EntityTypeBuilder<TrustedPersonPermission> builder)
    {
        builder.ToTable("trusted_person_permissions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.TrustedPersonId)
            .HasColumnName("trusted_person_id")
            .IsRequired();

        builder.Property(p => p.PermissionType)
            .HasColumnName("permission_type")
            .HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PermissionType>(v, true))
            .IsRequired();

        builder.Property(p => p.TargetCategoryId)
            .HasColumnName("target_category_id");

        builder.Property(p => p.TargetActionCardId)
            .HasColumnName("target_action_card_id");

        builder.Property(p => p.CanView)
            .HasColumnName("can_view")
            .IsRequired();

        builder.Property(p => p.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.Property(p => p.DeletedAtUtc)
            .HasColumnName("deleted_at");

        builder.Property(p => p.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.HasOne(p => p.TargetCategory)
            .WithMany()
            .HasForeignKey(p => p.TargetCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.TargetActionCard)
            .WithMany()
            .HasForeignKey(p => p.TargetActionCardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => new { p.TrustedPersonId, p.TargetCategoryId });
        builder.HasIndex(p => new { p.TrustedPersonId, p.TargetActionCardId });
    }
}

using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ContinuityItemConfiguration : IEntityTypeConfiguration<ContinuityItem>
{
    public void Configure(EntityTypeBuilder<ContinuityItem> builder)
    {
        builder.ToTable("continuity_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasColumnName("id");

        builder.Property(i => i.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(i => i.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Priority)
            .HasColumnName("priority")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<PriorityLevel>(v, true))
            .IsRequired();

        builder.Property(i => i.DocumentLocationHint)
            .HasColumnName("document_location_hint")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(i => i.AssignedTrustedPersonId)
            .HasColumnName("assigned_trusted_person_id")
            .IsRequired(false);

        builder.Property(i => i.ActionCardId)
            .HasColumnName("action_card_id")
            .IsRequired(false);

        builder.Property(i => i.CipherNotesBlob)
            .HasColumnName("cipher_notes_blob")
            .IsRequired(false);

        builder.Property(i => i.CipherNonce)
            .HasColumnName("cipher_nonce")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(i => i.CipherAuthTag)
            .HasColumnName("cipher_auth_tag")
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(i => i.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        builder.Property(i => i.IsCompleted)
            .HasColumnName("is_completed")
            .HasDefaultValue(false);

        builder.Property(i => i.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(i => i.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        builder.Property(i => i.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(i => i.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(i => i.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Foreign Key
        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(i => new { i.OwnerId, i.IsDeleted })
            .HasDatabaseName("idx_continuity_items_owner_deleted");

        builder.HasIndex(i => i.CategoryId)
            .HasDatabaseName("idx_continuity_items_category");

        builder.HasIndex(i => i.ActionCardId)
            .HasDatabaseName("idx_continuity_items_action_card");

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

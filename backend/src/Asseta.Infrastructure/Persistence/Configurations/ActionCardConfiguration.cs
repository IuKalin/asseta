using Asseta.Domain.Entities;
using Asseta.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActionCardConfiguration : IEntityTypeConfiguration<ActionCard>
{
    public void Configure(EntityTypeBuilder<ActionCard> builder)
    {
        builder.ToTable("action_cards");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(c => c.ContinuityItemId)
            .HasColumnName("continuity_item_id")
            .IsRequired(false);

        builder.Property(c => c.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Summary)
            .HasColumnName("summary")
            .IsRequired(false);

        builder.Property(c => c.Urgency)
            .HasColumnName("urgency_stage")
            .HasMaxLength(30)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<UrgencyStage>(v, true))
            .IsRequired();

        builder.Property(c => c.Priority)
            .HasColumnName("priority")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<PriorityLevel>(v, true))
            .IsRequired();

        builder.Property(c => c.AssignedTrustedPersonId)
            .HasColumnName("assigned_trusted_person_id")
            .IsRequired(false);

        builder.Property(c => c.DocumentLocationHint)
            .HasColumnName("document_location_hint")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.DigitalStorageLink)
            .HasColumnName("digital_storage_link")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.OwnsOne(c => c.CipherInstructions, b =>
        {
            b.Property(p => p.CipherBlob)
                .HasColumnName("cipher_instructions_blob")
                .IsRequired(false);

            b.Property(p => p.Nonce)
                .HasColumnName("cipher_nonce")
                .HasMaxLength(64)
                .IsRequired(false);

            b.Property(p => p.AuthTag)
                .HasColumnName("cipher_auth_tag")
                .HasMaxLength(64)
                .IsRequired(false);
        });

        builder.Property(c => c.IsCompleted)
            .HasColumnName("is_completed")
            .HasDefaultValue(false);

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(c => c.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(c => c.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Relationships
        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ContinuityItem)
            .WithOne()
            .HasForeignKey<ActionCard>(c => c.ContinuityItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Steps)
            .WithOne(s => s.ActionCard)
            .HasForeignKey(s => s.ActionCardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne(co => co.ActionCard)
            .HasForeignKey(co => co.ActionCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => new { c.OwnerId, c.IsDeleted })
            .HasDatabaseName("idx_action_cards_owner_deleted");

        builder.HasIndex(c => new { c.OwnerId, c.Urgency, c.IsDeleted })
            .HasDatabaseName("idx_action_cards_owner_urgency_deleted");

        builder.HasIndex(c => c.Urgency)
            .HasDatabaseName("idx_action_cards_urgency");

        builder.HasIndex(c => c.ContinuityItemId)
            .HasDatabaseName("idx_action_cards_continuity_item");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

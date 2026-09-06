using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActionCardContactConfiguration : IEntityTypeConfiguration<ActionCardContact>
{
    public void Configure(EntityTypeBuilder<ActionCardContact> builder)
    {
        builder.ToTable("action_card_contacts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");

        builder.Property(c => c.ActionCardId)
            .HasColumnName("action_card_id")
            .IsRequired();

        builder.Property(c => c.ContactName)
            .HasColumnName("contact_name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.RelationshipOrRole)
            .HasColumnName("relationship_or_role")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(30)
            .IsRequired(false);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired(false);

        builder.Property(c => c.ContactNotes)
            .HasColumnName("contact_notes")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(c => c.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(c => c.ActionCardId)
            .HasDatabaseName("idx_action_card_contacts_card");

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActionCardStepConfiguration : IEntityTypeConfiguration<ActionCardStep>
{
    public void Configure(EntityTypeBuilder<ActionCardStep> builder)
    {
        builder.ToTable("action_card_steps");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.ActionCardId)
            .HasColumnName("action_card_id")
            .IsRequired();

        builder.Property(s => s.StepOrder)
            .HasColumnName("step_order")
            .IsRequired();

        builder.Property(s => s.Instruction)
            .HasColumnName("instruction")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.EstimatedDuration)
            .HasColumnName("estimated_duration")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(s => s.IsCompleted)
            .HasColumnName("is_completed")
            .HasDefaultValue(false);

        builder.Property(s => s.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(s => s.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(s => new { s.ActionCardId, s.StepOrder })
            .HasDatabaseName("idx_action_card_steps_card_order");

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

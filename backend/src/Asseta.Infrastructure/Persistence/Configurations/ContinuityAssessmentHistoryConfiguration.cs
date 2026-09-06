using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ContinuityAssessmentHistoryConfiguration : IEntityTypeConfiguration<ContinuityAssessmentHistory>
{
    public void Configure(EntityTypeBuilder<ContinuityAssessmentHistory> builder)
    {
        builder.ToTable("continuity_assessment_history");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id)
            .HasColumnName("id");

        builder.Property(h => h.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(h => h.AssessmentVersion)
            .HasColumnName("assessment_version")
            .HasMaxLength(20)
            .HasDefaultValue("v1")
            .IsRequired();

        builder.Property(h => h.RawResponses)
            .HasColumnName("raw_responses")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(h => h.ItemsGeneratedCount)
            .HasColumnName("items_generated_count")
            .HasDefaultValue(0);

        builder.Property(h => h.InitialReadinessScore)
            .HasColumnName("initial_readiness_score")
            .HasDefaultValue(0);

        builder.Property(h => h.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(h => h.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(h => h.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(h => h.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(h => new { h.OwnerId, h.CreatedAtUtc })
            .HasDatabaseName("idx_continuity_assessment_history_owner");
    }
}

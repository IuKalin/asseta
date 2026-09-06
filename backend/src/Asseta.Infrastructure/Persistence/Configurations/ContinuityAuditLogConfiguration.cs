using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ContinuityAuditLogConfiguration : IEntityTypeConfiguration<ContinuityAuditLog>
{
    public void Configure(EntityTypeBuilder<ContinuityAuditLog> builder)
    {
        builder.ToTable("continuity_audit_logs");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id");

        builder.Property(a => a.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(a => a.ItemId)
            .HasColumnName("item_id")
            .IsRequired(false);

        builder.Property(a => a.Action)
            .HasColumnName("action")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.PayloadSnapshot)
            .HasColumnName("payload_snapshot")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45)
            .IsRequired(false);

        builder.Property(a => a.CorrelationId)
            .HasColumnName("correlation_id")
            .IsRequired();

        builder.Property(a => a.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(a => a.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        builder.HasIndex(a => new { a.OwnerId, a.CreatedAtUtc })
            .HasDatabaseName("idx_continuity_audit_logs_owner_time");
    }
}

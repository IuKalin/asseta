using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ContinuityCategoryConfiguration : IEntityTypeConfiguration<ContinuityCategory>
{
    public void Configure(EntityTypeBuilder<ContinuityCategory> builder)
    {
        builder.ToTable("continuity_categories");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.NameVi)
            .HasColumnName("name_vi")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.NameEn)
            .HasColumnName("name_en")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Icon)
            .HasColumnName("icon")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.SortOrder)
            .HasColumnName("sort_order")
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(c => c.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Seed 6 standard categories defined in SPEC.md
        builder.HasData(
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e01"), "FINANCIAL", "Tài chính & Nghĩa vụ tiền tệ", "Financial & Monetary Obligations", "wallet", 1),
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e02"), "PROPERTY", "Tài sản & Bất động sản", "Property & Real Estate", "home", 2),
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e03"), "INSURANCE", "Bảo hiểm & Quyền lợi sức khỏe", "Insurance & Health Policies", "shield", 3),
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e04"), "BUSINESS", "Doanh nghiệp & Quan hệ đối tác", "Business Operations & Partners", "briefcase", 4),
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e05"), "DOCUMENTS", "Hồ sơ & Giấy tờ pháp lý", "Legal Documents & Records", "file-text", 5),
            new ContinuityCategory(Guid.Parse("018e6e5a-7341-789a-9e12-2d93e1104e06"), "FAMILY", "Gia đình & Nghĩa vụ cá nhân", "Family & Personal Obligations", "users", 6)
        );
    }
}

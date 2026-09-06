using Asseta.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asseta.Infrastructure.Persistence.Configurations;

public class ActionCardTemplateConfiguration : IEntityTypeConfiguration<ActionCardTemplate>
{
    public void Configure(EntityTypeBuilder<ActionCardTemplate> builder)
    {
        builder.ToTable("action_card_templates");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.TemplateCode)
            .HasColumnName("template_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.TemplateCode)
            .IsUnique();

        builder.Property(t => t.CategoryCode)
            .HasColumnName("category_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.TitleVi)
            .HasColumnName("title_vi")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.TitleEn)
            .HasColumnName("title_en")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.DefaultUrgency)
            .HasColumnName("default_urgency")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.DefaultPriority)
            .HasColumnName("default_priority")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.SuggestedStepsJson)
            .HasColumnName("suggested_steps")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(t => t.SuggestedRolesJson)
            .HasColumnName("suggested_roles")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(t => t.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.Property(t => t.DeletedAtUtc)
            .HasColumnName("deleted_at")
            .IsRequired(false);

        // Seed 6 Standard Templates
        builder.HasData(
            new ActionCardTemplate(
                Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                "TPL_BANK_LOAN",
                "FINANCIAL",
                "Xử lý Khoản vay & Nghĩa vụ Trả nợ Ngân hàng",
                "Handle Bank Loan & Debt Obligations",
                "FIRST_72_HOURS",
                "CRITICAL",
                "[{\"stepOrder\":1,\"instruction\":\"Liên hệ cán bộ tín dụng phụ trách khoản vay để đối soát lịch trả nợ\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra hợp đồng tín dụng và khế ước nhận nợ gốc trong tủ tài liệu\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":3,\"instruction\":\"Đảm bảo tài khoản thanh toán tự động có đủ số dư cho kỳ trích nợ kế tiếp\",\"estimatedDuration\":\"20 phút\"}]",
                "[\"Cán bộ tín dụng ngân hàng\",\"Kế toán phụ trách\",\"Người đồng bảo lãnh\"]"
            ),
            new ActionCardTemplate(
                Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                "TPL_RENTAL_PROPERTY",
                "PROPERTY",
                "Quản lý Bất động sản Cho thuê & Khách thuê",
                "Manage Rental Property & Tenants",
                "FIRST_7_DAYS",
                "IMPORTANT",
                "[{\"stepOrder\":1,\"instruction\":\"Thông báo cho người thuê về đầu mối liên hệ tiếp nhận tiền thuê\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra hợp đồng thuê và hóa đơn phí quản lý/dịch vụ của tòa nhà\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Lưu giữ biên lai thanh toán định kỳ vào hồ sơ bất động sản\",\"estimatedDuration\":\"10 phút\"}]",
                "[\"Người thuê nhà\",\"Ban quản lý tòa nhà\",\"Môi giới quản lý\"]"
            ),
            new ActionCardTemplate(
                Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                "TPL_LIFE_INSURANCE",
                "INSURANCE",
                "Yêu cầu Quyền lợi Bồi thường Bảo hiểm",
                "Claim Insurance Benefits & Coverage",
                "IMMEDIATE",
                "CRITICAL",
                "[{\"stepOrder\":1,\"instruction\":\"Gọi hotline công ty bảo hiểm thông báo sự kiện bảo hiểm\",\"estimatedDuration\":\"20 phút\"},{\"stepOrder\":2,\"instruction\":\"Tìm giấy chứng nhận bảo hiểm nhân thọ và phụ lục hợp đồng\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":3,\"instruction\":\"Thu thập hồ sơ bệnh án hoặc giấy tờ xác nhận y tế từ bệnh viện\",\"estimatedDuration\":\"1-2 ngày\"}]",
                "[\"Đại lý bảo hiểm phục vụ\",\"Tổng đài bồi thường\",\"Bác sĩ điều trị\"]"
            ),
            new ActionCardTemplate(
                Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                "TPL_BUSINESS_OPS",
                "BUSINESS",
                "Ủy quyền Điều hành Khẩn cấp & Vận hành Doanh nghiệp",
                "Emergency Business Operations & Power of Attorney",
                "IMMEDIATE",
                "CRITICAL",
                "[{\"stepOrder\":1,\"instruction\":\"Họp khẩn ban lãnh đạo/đồng sáng lập kích hoạt quy trình ủy quyền\",\"estimatedDuration\":\"1 giờ\"},{\"stepOrder\":2,\"instruction\":\"Thông báo nhân sự chủ chốt duy trì hoạt động kinh doanh thường nhật\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Kiểm tra các lệnh chi lương và nghĩa vụ thanh toán nhà cung cấp\",\"estimatedDuration\":\"45 phút\"}]",
                "[\"Đồng sáng lập (Co-founder)\",\"Kế toán trưởng\",\"Luật sư doanh nghiệp\"]"
            ),
            new ActionCardTemplate(
                Guid.Parse("a5555555-5555-5555-5555-555555555555"),
                "TPL_LEGAL_DOCS",
                "DOCUMENTS",
                "Tiếp cận Tủ Hồ sơ Pháp lý & Hợp đồng Cốt tử",
                "Access Vital Legal Documents & Contracts",
                "FIRST_72_HOURS",
                "IMPORTANT",
                "[{\"stepOrder\":1,\"instruction\":\"Tìm chìa khóa hoặc vị trí cất giữ tủ hồ sơ bảo mật\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra danh mục sổ đỏ, giấy khai sinh, đăng ký kết hôn, giấy phép ĐKKD\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Chụp lưu bản sao số hóa và niêm phong lại tài liệu gốc\",\"estimatedDuration\":\"20 phút\"}]",
                "[\"Người giữ chìa khóa phụ\",\"Luật sư riêng\"]"
            ),
            new ActionCardTemplate(
                Guid.Parse("a6666666-6666-6666-6666-666666666666"),
                "TPL_FAMILY_SUPPORT",
                "FAMILY",
                "Duy trì Nghĩa vụ & Chi phí Người phụ thuộc",
                "Maintain Family Support & Dependent Obligations",
                "FIRST_7_DAYS",
                "IMPORTANT",
                "[{\"stepOrder\":1,\"instruction\":\"Kiểm tra hạn nộp học phí của con hoặc viện phí người cao tuổi\",\"estimatedDuration\":\"20 phút\"},{\"stepOrder\":2,\"instruction\":\"Thiết lập người phụ trách đưa đón và chăm sóc sinh hoạt hàng ngày\",\"estimatedDuration\":\"30 phút\"}]",
                "[\"Người giám hộ tạm thời\",\"Giáo viên chủ nhiệm\",\"Bác sĩ gia đình\"]"
            )
        );
    }
}

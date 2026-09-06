using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Asseta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAllModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "action_card_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    title_vi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    title_en = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    default_urgency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    default_priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    suggested_steps = table.Column<string>(type: "jsonb", nullable: false),
                    suggested_roles = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_card_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "activation_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trigger_source = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    initiated_by_trusted_person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    grace_period_expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    activated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    row_version = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activation_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "continuity_assessment_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "v1"),
                    raw_responses = table.Column<string>(type: "jsonb", nullable: false),
                    items_generated_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    initial_readiness_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_continuity_assessment_history", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "continuity_audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    payload_snapshot = table.Column<string>(type: "jsonb", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_continuity_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "continuity_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_continuity_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "owner_activation_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    check_in_interval_days = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    grace_period_hours = table.Column<int>(type: "integer", nullable: false, defaultValue: 48),
                    min_confirmations_required = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    last_check_in_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    next_check_in_due_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    row_version = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_owner_activation_configs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trusted_people",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    trust_level = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delegate_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trusted_people", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    master_key_verifier = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    encryption_salt = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "activation_confirmations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    activation_request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trusted_person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    confirmed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activation_confirmations", x => x.id);
                    table.ForeignKey(
                        name: "FK_activation_confirmations_activation_requests_activation_req~",
                        column: x => x.activation_request_id,
                        principalTable: "activation_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "continuity_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    document_location_hint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    assigned_trusted_person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action_card_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cipher_notes_blob = table.Column<string>(type: "text", nullable: true),
                    cipher_nonce = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    cipher_auth_tag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    row_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_continuity_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_continuity_items_continuity_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "continuity_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trusted_person_pairing_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trusted_person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    salt = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false),
                    lockout_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trusted_person_pairing_codes", x => x.id);
                    table.ForeignKey(
                        name: "FK_trusted_person_pairing_codes_trusted_people_trusted_person_~",
                        column: x => x.trusted_person_id,
                        principalTable: "trusted_people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "action_cards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    continuity_item_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    summary = table.Column<string>(type: "text", nullable: true),
                    urgency_stage = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    assigned_trusted_person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_location_hint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    digital_storage_link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    cipher_instructions_blob = table.Column<string>(type: "text", nullable: true),
                    cipher_nonce = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    cipher_auth_tag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    row_version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_cards", x => x.id);
                    table.ForeignKey(
                        name: "FK_action_cards_continuity_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "continuity_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_action_cards_continuity_items_continuity_item_id",
                        column: x => x.continuity_item_id,
                        principalTable: "continuity_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "action_card_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    relationship_or_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    contact_notes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_card_contacts", x => x.id);
                    table.ForeignKey(
                        name: "FK_action_card_contacts_action_cards_action_card_id",
                        column: x => x.action_card_id,
                        principalTable: "action_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "action_card_steps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_card_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    instruction = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    estimated_duration = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action_card_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_action_card_steps_action_cards_action_card_id",
                        column: x => x.action_card_id,
                        principalTable: "action_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trusted_person_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trusted_person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    target_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_action_card_id = table.Column<Guid>(type: "uuid", nullable: true),
                    can_view = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trusted_person_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_trusted_person_permissions_action_cards_target_action_card_~",
                        column: x => x.target_action_card_id,
                        principalTable: "action_cards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trusted_person_permissions_continuity_categories_target_cat~",
                        column: x => x.target_category_id,
                        principalTable: "continuity_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trusted_person_permissions_trusted_people_trusted_person_id",
                        column: x => x.trusted_person_id,
                        principalTable: "trusted_people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "action_card_templates",
                columns: new[] { "id", "category_code", "created_at", "default_priority", "default_urgency", "deleted_at", "suggested_roles", "suggested_steps", "template_code", "title_en", "title_vi", "updated_at" },
                values: new object[,]
                {
                    { new Guid("a1111111-1111-1111-1111-111111111111"), "FINANCIAL", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9014), "CRITICAL", "FIRST_72_HOURS", null, "[\"Cán bộ tín dụng ngân hàng\",\"Kế toán phụ trách\",\"Người đồng bảo lãnh\"]", "[{\"stepOrder\":1,\"instruction\":\"Liên hệ cán bộ tín dụng phụ trách khoản vay để đối soát lịch trả nợ\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra hợp đồng tín dụng và khế ước nhận nợ gốc trong tủ tài liệu\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":3,\"instruction\":\"Đảm bảo tài khoản thanh toán tự động có đủ số dư cho kỳ trích nợ kế tiếp\",\"estimatedDuration\":\"20 phút\"}]", "TPL_BANK_LOAN", "Handle Bank Loan & Debt Obligations", "Xử lý Khoản vay & Nghĩa vụ Trả nợ Ngân hàng", null },
                    { new Guid("a2222222-2222-2222-2222-222222222222"), "PROPERTY", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9056), "IMPORTANT", "FIRST_7_DAYS", null, "[\"Người thuê nhà\",\"Ban quản lý tòa nhà\",\"Môi giới quản lý\"]", "[{\"stepOrder\":1,\"instruction\":\"Thông báo cho người thuê về đầu mối liên hệ tiếp nhận tiền thuê\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra hợp đồng thuê và hóa đơn phí quản lý/dịch vụ của tòa nhà\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Lưu giữ biên lai thanh toán định kỳ vào hồ sơ bất động sản\",\"estimatedDuration\":\"10 phút\"}]", "TPL_RENTAL_PROPERTY", "Manage Rental Property & Tenants", "Quản lý Bất động sản Cho thuê & Khách thuê", null },
                    { new Guid("a3333333-3333-3333-3333-333333333333"), "INSURANCE", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9060), "CRITICAL", "IMMEDIATE", null, "[\"Đại lý bảo hiểm phục vụ\",\"Tổng đài bồi thường\",\"Bác sĩ điều trị\"]", "[{\"stepOrder\":1,\"instruction\":\"Gọi hotline công ty bảo hiểm thông báo sự kiện bảo hiểm\",\"estimatedDuration\":\"20 phút\"},{\"stepOrder\":2,\"instruction\":\"Tìm giấy chứng nhận bảo hiểm nhân thọ và phụ lục hợp đồng\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":3,\"instruction\":\"Thu thập hồ sơ bệnh án hoặc giấy tờ xác nhận y tế từ bệnh viện\",\"estimatedDuration\":\"1-2 ngày\"}]", "TPL_LIFE_INSURANCE", "Claim Insurance Benefits & Coverage", "Yêu cầu Quyền lợi Bồi thường Bảo hiểm", null },
                    { new Guid("a4444444-4444-4444-4444-444444444444"), "BUSINESS", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9092), "CRITICAL", "IMMEDIATE", null, "[\"Đồng sáng lập (Co-founder)\",\"Kế toán trưởng\",\"Luật sư doanh nghiệp\"]", "[{\"stepOrder\":1,\"instruction\":\"Họp khẩn ban lãnh đạo/đồng sáng lập kích hoạt quy trình ủy quyền\",\"estimatedDuration\":\"1 giờ\"},{\"stepOrder\":2,\"instruction\":\"Thông báo nhân sự chủ chốt duy trì hoạt động kinh doanh thường nhật\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Kiểm tra các lệnh chi lương và nghĩa vụ thanh toán nhà cung cấp\",\"estimatedDuration\":\"45 phút\"}]", "TPL_BUSINESS_OPS", "Emergency Business Operations & Power of Attorney", "Ủy quyền Điều hành Khẩn cấp & Vận hành Doanh nghiệp", null },
                    { new Guid("a5555555-5555-5555-5555-555555555555"), "DOCUMENTS", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9097), "IMPORTANT", "FIRST_72_HOURS", null, "[\"Người giữ chìa khóa phụ\",\"Luật sư riêng\"]", "[{\"stepOrder\":1,\"instruction\":\"Tìm chìa khóa hoặc vị trí cất giữ tủ hồ sơ bảo mật\",\"estimatedDuration\":\"15 phút\"},{\"stepOrder\":2,\"instruction\":\"Kiểm tra danh mục sổ đỏ, giấy khai sinh, đăng ký kết hôn, giấy phép ĐKKD\",\"estimatedDuration\":\"30 phút\"},{\"stepOrder\":3,\"instruction\":\"Chụp lưu bản sao số hóa và niêm phong lại tài liệu gốc\",\"estimatedDuration\":\"20 phút\"}]", "TPL_LEGAL_DOCS", "Access Vital Legal Documents & Contracts", "Tiếp cận Tủ Hồ sơ Pháp lý & Hợp đồng Cốt tử", null },
                    { new Guid("a6666666-6666-6666-6666-666666666666"), "FAMILY", new DateTime(2026, 9, 5, 16, 24, 24, 181, DateTimeKind.Utc).AddTicks(9101), "IMPORTANT", "FIRST_7_DAYS", null, "[\"Người giám hộ tạm thời\",\"Giáo viên chủ nhiệm\",\"Bác sĩ gia đình\"]", "[{\"stepOrder\":1,\"instruction\":\"Kiểm tra hạn nộp học phí của con hoặc viện phí người cao tuổi\",\"estimatedDuration\":\"20 phút\"},{\"stepOrder\":2,\"instruction\":\"Thiết lập người phụ trách đưa đón và chăm sóc sinh hoạt hàng ngày\",\"estimatedDuration\":\"30 phút\"}]", "TPL_FAMILY_SUPPORT", "Maintain Family Support & Dependent Obligations", "Duy trì Nghĩa vụ & Chi phí Người phụ thuộc", null }
                });

            migrationBuilder.InsertData(
                table: "continuity_categories",
                columns: new[] { "id", "code", "created_at", "deleted_at", "icon", "name_en", "name_vi", "sort_order", "updated_at" },
                values: new object[,]
                {
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e01"), "FINANCIAL", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7003), null, "wallet", "Financial & Monetary Obligations", "Tài chính & Nghĩa vụ tiền tệ", 1, null },
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e02"), "PROPERTY", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7009), null, "home", "Property & Real Estate", "Tài sản & Bất động sản", 2, null },
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e03"), "INSURANCE", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7013), null, "shield", "Insurance & Health Policies", "Bảo hiểm & Quyền lợi sức khỏe", 3, null },
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e04"), "BUSINESS", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7017), null, "briefcase", "Business Operations & Partners", "Doanh nghiệp & Quan hệ đối tác", 4, null },
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e05"), "DOCUMENTS", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7021), null, "file-text", "Legal Documents & Records", "Hồ sơ & Giấy tờ pháp lý", 5, null },
                    { new Guid("018e6e5a-7341-789a-9e12-2d93e1104e06"), "FAMILY", new DateTime(2026, 9, 5, 16, 24, 24, 183, DateTimeKind.Utc).AddTicks(7035), null, "users", "Family & Personal Obligations", "Gia đình & Nghĩa vụ cá nhân", 6, null }
                });

            migrationBuilder.CreateIndex(
                name: "idx_action_card_contacts_card",
                table: "action_card_contacts",
                column: "action_card_id");

            migrationBuilder.CreateIndex(
                name: "idx_action_card_steps_card_order",
                table: "action_card_steps",
                columns: new[] { "action_card_id", "step_order" });

            migrationBuilder.CreateIndex(
                name: "IX_action_card_templates_template_code",
                table: "action_card_templates",
                column: "template_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_action_cards_continuity_item",
                table: "action_cards",
                column: "continuity_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_action_cards_owner_deleted",
                table: "action_cards",
                columns: new[] { "owner_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "idx_action_cards_owner_urgency_deleted",
                table: "action_cards",
                columns: new[] { "owner_id", "urgency_stage", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "idx_action_cards_urgency",
                table: "action_cards",
                column: "urgency_stage");

            migrationBuilder.CreateIndex(
                name: "IX_action_cards_category_id",
                table: "action_cards",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_activation_confirmations_activation_request_id_trusted_pers~",
                table: "activation_confirmations",
                columns: new[] { "activation_request_id", "trusted_person_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activation_requests_owner_id_status",
                table: "activation_requests",
                columns: new[] { "owner_id", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_continuity_assessment_history_owner",
                table: "continuity_assessment_history",
                columns: new[] { "owner_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "idx_continuity_audit_logs_owner_time",
                table: "continuity_audit_logs",
                columns: new[] { "owner_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_continuity_categories_code",
                table: "continuity_categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_continuity_items_action_card",
                table: "continuity_items",
                column: "action_card_id");

            migrationBuilder.CreateIndex(
                name: "idx_continuity_items_category",
                table: "continuity_items",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "idx_continuity_items_owner_deleted",
                table: "continuity_items",
                columns: new[] { "owner_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_owner_activation_configs_next_check_in_due_utc_status",
                table: "owner_activation_configs",
                columns: new[] { "next_check_in_due_utc", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_owner_activation_configs_owner_id",
                table: "owner_activation_configs",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trusted_people_owner_id_is_deleted",
                table: "trusted_people",
                columns: new[] { "owner_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_trusted_person_pairing_codes_trusted_person_id_is_used_expi~",
                table: "trusted_person_pairing_codes",
                columns: new[] { "trusted_person_id", "is_used", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_trusted_person_permissions_target_action_card_id",
                table: "trusted_person_permissions",
                column: "target_action_card_id");

            migrationBuilder.CreateIndex(
                name: "IX_trusted_person_permissions_target_category_id",
                table: "trusted_person_permissions",
                column: "target_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_trusted_person_permissions_trusted_person_id_target_action_~",
                table: "trusted_person_permissions",
                columns: new[] { "trusted_person_id", "target_action_card_id" });

            migrationBuilder.CreateIndex(
                name: "IX_trusted_person_permissions_trusted_person_id_target_categor~",
                table: "trusted_person_permissions",
                columns: new[] { "trusted_person_id", "target_category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_refresh_tokens_token_hash",
                table: "user_refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_refresh_tokens_user_id",
                table: "user_refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action_card_contacts");

            migrationBuilder.DropTable(
                name: "action_card_steps");

            migrationBuilder.DropTable(
                name: "action_card_templates");

            migrationBuilder.DropTable(
                name: "activation_confirmations");

            migrationBuilder.DropTable(
                name: "continuity_assessment_history");

            migrationBuilder.DropTable(
                name: "continuity_audit_logs");

            migrationBuilder.DropTable(
                name: "owner_activation_configs");

            migrationBuilder.DropTable(
                name: "trusted_person_pairing_codes");

            migrationBuilder.DropTable(
                name: "trusted_person_permissions");

            migrationBuilder.DropTable(
                name: "user_refresh_tokens");

            migrationBuilder.DropTable(
                name: "activation_requests");

            migrationBuilder.DropTable(
                name: "action_cards");

            migrationBuilder.DropTable(
                name: "trusted_people");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "continuity_items");

            migrationBuilder.DropTable(
                name: "continuity_categories");
        }
    }
}

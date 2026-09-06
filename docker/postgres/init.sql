-- Extension initialization for Asseta Vault & Identity
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Module 0: Users & Identity
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(150) NOT NULL,
    phone_number VARCHAR(30) NULL,
    master_key_verifier VARCHAR(64) NOT NULL,
    encryption_salt VARCHAR(64) NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'ACTIVE',
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);

CREATE TABLE IF NOT EXISTS user_refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL UNIQUE,
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_user_refresh_tokens_user ON user_refresh_tokens(user_id);

-- Seed Demo User: globalhelcurt14092005@gmail.com / Minhdz2005@
INSERT INTO users (
    id, email, password_hash, full_name, phone_number, 
    master_key_verifier, encryption_salt, status
)
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'globalhelcurt14092005@gmail.com',
    '$2a$06$gZwCxU/lgupfrXMc0d/eHeysZI.T4aLFXafXta1o59iWKBDaLI/X.',
    'Minh Asseta Demo',
    '0901234567',
    encode(digest('AK-DEMO-2026-ASSETA-VAULT', 'sha256'), 'hex'),
    'a1b2c3d4e5f60718293a4b5c6d7e8f90',
    'ACTIVE'
)
ON CONFLICT (email) DO UPDATE SET 
    password_hash = EXCLUDED.password_hash,
    full_name = EXCLUDED.full_name;

-- Module 1: Continuity Categories
CREATE TABLE IF NOT EXISTS continuity_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL UNIQUE,
    name_vi VARCHAR(100) NOT NULL,
    name_en VARCHAR(100) NOT NULL,
    icon VARCHAR(50) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

-- Seed Data: 6 Standard Categories
INSERT INTO continuity_categories (id, code, name_vi, name_en, icon, sort_order)
VALUES 
('11111111-1111-1111-1111-111111111111', 'FINANCIAL', 'Tài chính & Nghĩa vụ tiền tệ', 'Financial & Monetary Obligations', 'AccountBalance', 1),
('22222222-2222-2222-2222-222222222222', 'PROPERTY', 'Tài sản & Bất động sản', 'Real Estate & Tangible Assets', 'Home', 2),
('33333333-3333-3333-3333-333333333333', 'INSURANCE', 'Bảo hiểm & Quyền lợi sức khỏe', 'Insurance & Health Benefits', 'Security', 3),
('44444444-4444-4444-4444-444444444444', 'BUSINESS', 'Doanh nghiệp & Quan hệ đối tác', 'Business & Partnership Relations', 'BusinessCenter', 4),
('55555555-5555-5555-5555-555555555555', 'DOCUMENTS', 'Hồ sơ & Giấy tờ pháp lý', 'Legal Documents & Records', 'Description', 5),
('66666666-6666-6666-6666-666666666666', 'FAMILY', 'Gia đình & Nghĩa vụ cá nhân', 'Family & Personal Commitments', 'FamilyRestroom', 6)
ON CONFLICT (code) DO NOTHING;

-- Module 1: Continuity Items
CREATE TABLE IF NOT EXISTS continuity_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id UUID NOT NULL REFERENCES continuity_categories(id) ON DELETE RESTRICT,
    name VARCHAR(200) NOT NULL,
    priority VARCHAR(20) NOT NULL CHECK (priority IN ('CRITICAL', 'IMPORTANT', 'LOW')),
    document_location_hint VARCHAR(255) NULL,
    assigned_trusted_person_id UUID NULL,
    action_card_id UUID NULL,
    cipher_notes_blob TEXT NULL,
    cipher_nonce VARCHAR(64) NULL,
    cipher_auth_tag VARCHAR(64) NULL,
    sort_order INT NOT NULL DEFAULT 0,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    row_version INT NOT NULL DEFAULT 1,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_continuity_items_owner_deleted ON continuity_items(owner_id, is_deleted);
CREATE INDEX IF NOT EXISTS idx_continuity_items_category ON continuity_items(category_id);

-- Module 1: Continuity Audit Logs
CREATE TABLE IF NOT EXISTS continuity_audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    item_id UUID NULL,
    action VARCHAR(50) NOT NULL,
    payload_snapshot JSONB NOT NULL,
    ip_address VARCHAR(45) NULL,
    correlation_id UUID NOT NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_continuity_audit_logs_owner ON continuity_audit_logs(owner_id, created_at DESC);

-- Module 1: Continuity Assessment History
CREATE TABLE IF NOT EXISTS continuity_assessment_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    assessment_version VARCHAR(20) NOT NULL DEFAULT 'v1',
    raw_responses JSONB NOT NULL,
    items_generated_count INT NOT NULL DEFAULT 0,
    initial_readiness_score INT NOT NULL DEFAULT 0,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_continuity_assessment_history_owner ON continuity_assessment_history(owner_id, created_at DESC);

-- Module 2: Action Cards
CREATE TABLE IF NOT EXISTS action_cards (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    category_id UUID NOT NULL REFERENCES continuity_categories(id) ON DELETE RESTRICT,
    continuity_item_id UUID NULL REFERENCES continuity_items(id) ON DELETE SET NULL,
    title VARCHAR(200) NOT NULL,
    summary TEXT NULL,
    urgency_stage VARCHAR(30) NOT NULL CHECK (urgency_stage IN ('IMMEDIATE', 'FIRST_72_HOURS', 'FIRST_7_DAYS', 'LONGER_TERM')),
    priority VARCHAR(20) NOT NULL CHECK (priority IN ('CRITICAL', 'IMPORTANT', 'LOW')),
    assigned_trusted_person_id UUID NULL,
    document_location_hint VARCHAR(255) NULL,
    digital_storage_link VARCHAR(500) NULL,
    cipher_instructions_blob TEXT NULL,
    cipher_nonce VARCHAR(64) NULL,
    cipher_auth_tag VARCHAR(64) NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    row_version INT NOT NULL DEFAULT 1,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_action_cards_owner_deleted ON action_cards(owner_id, is_deleted);
CREATE INDEX IF NOT EXISTS idx_action_cards_urgency ON action_cards(urgency_stage);
CREATE INDEX IF NOT EXISTS idx_action_cards_continuity_item ON action_cards(continuity_item_id);

-- Module 2: Action Card Steps
CREATE TABLE IF NOT EXISTS action_card_steps (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    action_card_id UUID NOT NULL REFERENCES action_cards(id) ON DELETE CASCADE,
    step_order INT NOT NULL,
    instruction VARCHAR(500) NOT NULL,
    estimated_duration VARCHAR(50) NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_action_card_steps_card_order ON action_card_steps(action_card_id, step_order);

-- Module 2: Action Card Contacts
CREATE TABLE IF NOT EXISTS action_card_contacts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    action_card_id UUID NOT NULL REFERENCES action_cards(id) ON DELETE CASCADE,
    contact_name VARCHAR(150) NOT NULL,
    relationship_or_role VARCHAR(100) NOT NULL,
    phone_number VARCHAR(30) NULL,
    email VARCHAR(150) NULL,
    contact_notes VARCHAR(255) NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_action_card_contacts_card ON action_card_contacts(action_card_id);

-- Module 2: Action Card Templates
CREATE TABLE IF NOT EXISTS action_card_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    template_code VARCHAR(50) NOT NULL UNIQUE,
    category_code VARCHAR(50) NOT NULL,
    title_vi VARCHAR(200) NOT NULL,
    title_en VARCHAR(200) NOT NULL,
    default_urgency VARCHAR(30) NOT NULL,
    default_priority VARCHAR(20) NOT NULL,
    suggested_steps JSONB NOT NULL,
    suggested_roles JSONB NOT NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL,
    deleted_at TIMESTAMPTZ NULL
);

INSERT INTO action_card_templates (id, template_code, category_code, title_vi, title_en, default_urgency, default_priority, suggested_steps, suggested_roles)
VALUES
('a1111111-1111-1111-1111-111111111111', 'TPL_BANK_LOAN', 'FINANCIAL', 'Xử lý Khoản vay & Nghĩa vụ Trả nợ Ngân hàng', 'Handle Bank Loan & Debt Obligations', 'FIRST_72_HOURS', 'CRITICAL', '[{"stepOrder":1,"instruction":"Liên hệ cán bộ tín dụng phụ trách khoản vay để đối soát lịch trả nợ","estimatedDuration":"30 phút"},{"stepOrder":2,"instruction":"Kiểm tra hợp đồng tín dụng và khế ước nhận nợ gốc trong tủ tài liệu","estimatedDuration":"15 phút"},{"stepOrder":3,"instruction":"Đảm bảo tài khoản thanh toán tự động có đủ số dư cho kỳ trích nợ kế tiếp","estimatedDuration":"20 phút"}]', '["Cán bộ tín dụng ngân hàng","Kế toán phụ trách","Người đồng bảo lãnh"]'),
('a2222222-2222-2222-2222-222222222222', 'TPL_RENTAL_PROPERTY', 'PROPERTY', 'Quản lý Bất động sản Cho thuê & Khách thuê', 'Manage Rental Property & Tenants', 'FIRST_7_DAYS', 'IMPORTANT', '[{"stepOrder":1,"instruction":"Thông báo cho người thuê về đầu mối liên hệ tiếp nhận tiền thuê","estimatedDuration":"15 phút"},{"stepOrder":2,"instruction":"Kiểm tra hợp đồng thuê và hóa đơn phí quản lý/dịch vụ của tòa nhà","estimatedDuration":"30 phút"},{"stepOrder":3,"instruction":"Lưu giữ biên lai thanh toán định kỳ vào hồ sơ bất động sản","estimatedDuration":"10 phút"}]', '["Người thuê nhà","Ban quản lý tòa nhà","Môi giới quản lý"]'),
('a3333333-3333-3333-3333-333333333333', 'TPL_LIFE_INSURANCE', 'INSURANCE', 'Yêu cầu Quyền lợi Bồi thường Bảo hiểm', 'Claim Insurance Benefits & Coverage', 'IMMEDIATE', 'CRITICAL', '[{"stepOrder":1,"instruction":"Gọi hotline công ty bảo hiểm thông báo sự kiện bảo hiểm","estimatedDuration":"20 phút"},{"stepOrder":2,"instruction":"Tìm giấy chứng nhận bảo hiểm nhân thọ và phụ lục hợp đồng","estimatedDuration":"15 phút"},{"stepOrder":3,"instruction":"Thu thập hồ sơ bệnh án hoặc giấy tờ xác nhận y tế từ bệnh viện","estimatedDuration":"1-2 ngày"}]', '["Đại lý bảo hiểm phục vụ","Tổng đài bồi thường","Bác sĩ điều trị"]'),
('a4444444-4444-4444-4444-444444444444', 'TPL_BUSINESS_OPS', 'BUSINESS', 'Ủy quyền Điều hành Khẩn cấp & Vận hành Doanh nghiệp', 'Emergency Business Operations & Power of Attorney', 'IMMEDIATE', 'CRITICAL', '[{"stepOrder":1,"instruction":"Họp khẩn ban lãnh đạo/đồng sáng lập kích hoạt quy trình ủy quyền","estimatedDuration":"1 giờ"},{"stepOrder":2,"instruction":"Thông báo nhân sự chủ chốt duy trì hoạt động kinh doanh thường nhật","estimatedDuration":"30 phút"},{"stepOrder":3,"instruction":"Kiểm tra các lệnh chi lương và nghĩa vụ thanh toán nhà cung cấp","estimatedDuration":"45 phút"}]', '["Đồng sáng lập (Co-founder)","Kế toán trưởng","Luật sư doanh nghiệp"]'),
('a5555555-5555-5555-5555-555555555555', 'TPL_LEGAL_DOCS', 'DOCUMENTS', 'Tiếp cận Tủ Hồ sơ Pháp lý & Hợp đồng Cốt tử', 'Access Vital Legal Documents & Contracts', 'FIRST_72_HOURS', 'IMPORTANT', '[{"stepOrder":1,"instruction":"Tìm chìa khóa hoặc vị trí cất giữ tủ hồ sơ bảo mật","estimatedDuration":"15 phút"},{"stepOrder":2,"instruction":"Kiểm tra danh mục sổ đỏ, giấy khai sinh, đăng ký kết hôn, giấy phép ĐKKD","estimatedDuration":"30 phút"},{"stepOrder":3,"instruction":"Chụp lưu bản sao số hóa và niêm phong lại tài liệu gốc","estimatedDuration":"20 phút"}]', '["Người giữ chìa khóa phụ","Luật sư riêng"]'),
('a6666666-6666-6666-6666-666666666666', 'TPL_FAMILY_SUPPORT', 'FAMILY', 'Duy trì Nghĩa vụ & Chi phí Người phụ thuộc', 'Maintain Family Support & Dependent Obligations', 'FIRST_7_DAYS', 'IMPORTANT', '[{"stepOrder":1,"instruction":"Kiểm tra hạn nộp học phí của con hoặc viện phí người cao tuổi","estimatedDuration":"20 phút"},{"stepOrder":2,"instruction":"Thiết lập người phụ trách đưa đón và chăm sóc sinh hoạt hàng ngày","estimatedDuration":"30 phút"}]', '["Người giám hộ tạm thời","Giáo viên chủ nhiệm","Bác sĩ gia đình"]')
ON CONFLICT (template_code) DO NOTHING;

-- Backward compatibility with initial template
CREATE TABLE IF NOT EXISTS assets (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    type INT NOT NULL,
    estimated_value NUMERIC(18, 2) NOT NULL DEFAULT 0.0,
    encrypted_vault_data TEXT,
    owner_id UUID NOT NULL,
    created_at_utc TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at_utc TIMESTAMP WITH TIME ZONE,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    entity_name VARCHAR(100) NOT NULL,
    action VARCHAR(50) NOT NULL,
    performed_by UUID NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    details JSONB
);

CREATE INDEX IF NOT EXISTS idx_assets_owner ON assets(owner_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_audit_timestamp ON audit_logs(timestamp);

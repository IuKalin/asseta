# Đặc Tả Kỹ Thuật Chuẩn EARS: Module 2 – Action Cards (Thẻ Hành Động Tiếp Quản)

**Mã Module:** `module2` (Tương đương `feat-02-action-cards`)  
**Phiên bản:** v1.0.0 DRAFT  
**Phương pháp áp dụng:** Spec-Driven Development (SDD) & EARS Syntax  
**Cơ chế bảo mật:** Zero-Knowledge & Client-Side Encryption (AES-256-GCM)  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module2/CONTEXT.md)  

---

## 1. Bối Cảnh & Mục Tiêu Kỹ Thuật (Context & Goals)

### 1.1. Bối cảnh
Nếu **Continuity Map (Module 1)** cung cấp bức tranh toàn cảnh về những gì đang phụ thuộc vào người dùng, thì **Action Cards (Module 2)** là bộ công cụ chỉ dẫn thực thi chi tiết. Khi người nắm quyền đột ngột vắng mặt (tai nạn, nhập viện, mất năng lực hành vi, mất liên lạc khẩn cấp), người tiếp quản cần biết chính xác: **Cần làm việc gì trước → Gặp ai → Lấy tài liệu ở đâu → Trình tự các bước xử lý cụ thể ra sao**.

### 1.2. Mục tiêu kỹ thuật của Module 2
- Xây dựng mô hình dữ liệu và tập hợp API cho **Action Cards (Thẻ Hành Động)**, bao gồm: tiêu đề, mô tả tóm tắt, khung thời gian khẩn cấp (`UrgencyStage`), vị trí hồ sơ, đầu mối liên hệ khẩn cấp (`KeyContacts`), và danh sách các bước hành động cụ thể (`ActionSteps`).
- Hỗ trợ **Action Card Templates**: Các mẫu hướng dẫn dựng sẵn theo từng lĩnh vực chuyên môn (Khoản vay thế chấp, Vận hành công ty, Hợp đồng bảo hiểm, Bất động sản cho thuê, Hồ sơ pháp lý, Người phụ thuộc).
- Thiết lập **Liên kết hai chiều với Continuity Item (Module 1)**: Cho phép chuyển đổi 1-1 từ hạng mục tiếp quản sang thẻ hành động. Khi hoàn thành Action Card, hệ thống tự động giải tỏa trạng thái `ContinuityGap` và cập nhật tăng điểm Readiness Score của danh mục tương ứng.
- Tuân thủ nghiêm ngặt nguyên tắc **Zero-Knowledge**: Trường chỉ dẫn bí mật (`confidential_instructions`) được mã hóa hoàn toàn tại Client bằng `AES-256-GCM` trước khi truyền lên Server. Server chỉ lưu bản mã `cipher_instructions_blob`, `cipher_nonce`, `cipher_auth_tag`.

---

## 2. Tác Nhân & Vai Trò (Actors & Roles)

| Tác nhân (Actor) | Vai trò & Quyền hạn trong Module 2 |
| :--- | :--- |
| **Owner (Chủ tài sản)** | Toàn quyền tạo, chỉnh sửa, xóa mềm Action Cards của chính mình; gắn đầu mối liên hệ, tạo/sắp xếp thứ tự các bước hành động, và áp dụng các mẫu template sẵn có. |
| **Trusted Contact (Người ủy thác)** | Trong trạng thái vận hành bình thường, không thể xem Action Card. Sau khi quy trình Safe Activation (Module 5) được kích hoạt, chỉ được xem các Action Cards mà mình được phân công phụ trách. |
| **System Worker (Tiến trình hệ thống)** | Tự động đồng bộ trạng thái giữa Action Card và Continuity Item, tính toán lại điểm Readiness Score và xóa bỏ cờ Continuity Gap. |

---

## 3. Yêu Cầu Chức Năng (Functional Requirements - EARS Syntax)

### 3.1. Yêu Cầu Phổ Quát (Ubiquitous Requirements)

- `[REQ-CARD-001]`: Hệ thống SHALL luôn luôn cô lập dữ liệu Action Cards, danh sách các bước hành động (`action_card_steps`) và danh bạ liên hệ (`action_card_contacts`) theo mã định danh người dùng (`OwnerId`), nghiêm cấm truy xuất hoặc chỉnh sửa dữ liệu chéo giữa các tài khoản khác nhau.
- `[REQ-CARD-002]`: Hệ thống SHALL luôn luôn từ chối tiếp nhận và lưu trữ các trường dữ liệu chứa số dư tài khoản, mật khẩu đăng nhập, mã PIN ngân hàng, mã OTP, số CVV thẻ tín dụng, private key blockchain hoặc seed phrase ở tất cả các tầng API DTO và Database Entity của Action Card.
- `[REQ-CARD-003]`: Hệ thống SHALL luôn luôn ghi nhận nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác tạo mới, cập nhật các bước, thay đổi đầu mối liên hệ hoặc xóa mềm Action Card.
- `[REQ-CARD-004]`: Hệ thống SHALL luôn luôn áp dụng cơ chế Soft-Delete (`is_deleted = true`, `deleted_at = UtcNow`) khi người dùng thực hiện thao tác xóa Action Card, đồng thời áp dụng xóa mềm đồng bộ (cascade soft-delete) cho toàn bộ các bước hành động và đầu mối liên hệ trực thuộc thẻ đó.

### 3.2. Yêu Cầu Kích Hoạt Theo Sự Kiện (Event-Driven Requirements)

- `[REQ-CARD-005]`: WHEN người dùng chọn tạo một Action Card từ một Continuity Item có sẵn (`ContinuityItemId`), THE hệ thống SHALL tự động kế thừa thông tin danh mục (`category_id`), tên gọi (`title`), người phụ trách (`assigned_trusted_person_id`), vị trí tài liệu (`document_location_hint`), tự động liên kết khóa ngoại hai chiều qua `action_card_id` và cập nhật lại điểm Readiness Score của danh mục tương ứng.
- `[REQ-CARD-006]`: WHEN người dùng lựa chọn một Action Card Template mẫu (ví dụ: Vay thế chấp ngân hàng, Vận hành doanh nghiệp, Bảo hiểm nhân thọ, Nhà cho thuê, Hồ sơ pháp lý), THE hệ thống SHALL tự động khởi tạo sẵn danh sách các bước hành động chuẩn (`action_card_steps`) và các vai trò liên hệ gợi ý tương ứng với template đó.
- `[REQ-CARD-007]`: WHEN người dùng gửi yêu cầu tạo mới Action Card độc lập (không bắt nguồn từ Continuity Item), THE hệ thống SHALL kiểm tra tính hợp lệ qua FluentValidation, lưu trữ bản ghi vào PostgreSQL và gán khung thời gian khẩn cấp (`urgency_stage`) tương ứng.
- `[REQ-CARD-008]`: WHEN người dùng cập nhật thông tin Action Card (tiêu đề, khung thời gian khẩn cấp, vị trí tài liệu, người phụ trách hoặc bản mã chỉ dẫn mật mã), THE hệ thống SHALL kiểm tra quyền sở hữu, cập nhật dữ liệu, tăng `row_version` và phát sinh Audit Log.
- `[REQ-CARD-009]`: WHEN người dùng thêm, sửa, xóa hoặc sắp xếp lại thứ tự (`reorder`) các bước hành động trong checklist của Action Card, THE hệ thống SHALL cập nhật thứ tự mới (`step_order`) bắt đầu từ 1 theo chuỗi số nguyên liên tục không bị đứt đoạn.
- `[REQ-CARD-010]`: WHEN người dùng gắn hoặc cập nhật thông tin đầu mối liên hệ khẩn cấp (`KeyContact`), THE hệ thống SHALL lưu trữ họ tên, vai trò/tổ chức, số điện thoại, email và ghi chú quan hệ với giới hạn tối đa 5 đầu mối trên một Action Card.

### 3.3. Yêu Cầu Theo Trạng Thái (State-Driven Requirements)

- `[REQ-CARD-011]`: WHILE một Action Card có khung thời gian là `IMMEDIATE` hoặc `FIRST_72_HOURS` nhưng chưa có bất kỳ bước hành động nào (`step_count = 0`) hoặc chưa có người phụ trách (`assigned_trusted_person_id IS NULL`), THE hệ thống SHALL duy trì trạng thái cờ `is_incomplete = true` và hiển thị cảnh báo trực quan trên giao diện người dùng.
- `[REQ-CARD-012]`: WHILE ứng dụng di động (Flutter) ở trạng thái mất kết nối mạng (`Offline`), THE hệ thống phía Client (BLoC State) SHALL cho phép xem toàn bộ danh sách Action Cards đã cache cục bộ, xem chi tiết các bước checklist và lọc theo khung thời gian khẩn cấp (`UrgencyStage`).

### 3.4. Yêu Cầu Xử Lý Bất Thường & Ngoại Lệ (Unwanted Behavior Requirements)

- `[REQ-CARD-013]`: IF dữ liệu yêu cầu tạo mới hoặc cập nhật Action Card bị thiếu tiêu đề (rỗng hoặc > 200 ký tự), danh mục không hợp lệ, hoặc khung thời gian khẩn cấp không thuộc 4 giá trị quy chuẩn (`IMMEDIATE`, `FIRST_72_HOURS`, `FIRST_7_DAYS`, `LONGER_TERM`), THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi nghiệp vụ `VALIDATION_FAILED`.
- `[REQ-CARD-014]`: IF dữ liệu gửi lên trong các trường văn bản mở (tiêu đề, mô tả bước hành động, ghi chú liên hệ) chứa chuỗi nhận diện số thẻ tín dụng hoặc private key (kiểm tra Regex tầng Application), THEN hệ thống SHALL chặn đứng giao dịch, ghi log cảnh báo an toàn và trả về mã lỗi HTTP 422 Unprocessable Entity với mã `SENSITIVE_DATA_DETECTED`.
- `[REQ-CARD-015]`: IF người dùng cố gắng truy xuất, cập nhật hoặc xóa Action Card không thuộc quyền sở hữu của mình, THEN hệ thống SHALL từ chối và trả về mã lỗi HTTP 403 Forbidden với mã `UNAUTHORIZED_RESOURCE_ACCESS`.
- `[REQ-CARD-016]`: IF client gửi yêu cầu có `Idempotency-Key` đã được thực thi thành công trong vòng 24 giờ qua, THEN hệ thống SHALL trả về kết quả đã cache mà không tạo Action Card hoặc bước hành động trùng lặp trong cơ sở dữ liệu.
- `[REQ-CARD-017]`: IF xảy ra xung đột đồng thời khi hai thiết bị cùng cập nhật một Action Card (sai lệch `row_version`), THEN hệ thống SHALL từ chối bản ghi đến sau và trả về mã lỗi HTTP 409 Conflict với mã `CONCURRENT_STATE_MUTATION`.

### 3.5. Yêu Cầu Tùy Chọn (Optional Requirements)

- `[REQ-CARD-018]`: WHERE người dùng nhập trường chỉ dẫn khẩn cấp nhạy cảm (`ConfidentialInstructions`), THE hệ thống SHALL chỉ chấp nhận dữ liệu này dưới dạng chuỗi đã được mã hóa Client-Side (`cipher_instructions_blob`) kèm vector khởi tạo (`cipher_nonce`) và thẻ xác thực (`cipher_auth_tag`).
- `[REQ-CARD-019]`: WHERE một Action Card có chứa liên kết tài liệu số tham chiếu (`digital_storage_link`), THE hệ thống SHALL kiểm tra tính hợp lệ của cú pháp URL (HTTP/HTTPS) và lưu trữ liên kết an toàn mà không tải tệp nhị phân lên máy chủ.

### 3.6. Yêu Cầu Phức Hợp (Complex Requirements)

- `[REQ-CARD-020]`: WHEN người dùng hoàn thành việc điền đầy đủ cả Người phụ trách, Vị trí hồ sơ và ít nhất 1 Bước hành động cho một Action Card WHILE Action Card này đang liên kết với một Continuity Item có cờ Continuity Gap, IF Continuity Item đó có mức ưu tiên `Critical` hoặc `Important`, THEN hệ thống SHALL tự động giải phóng cờ `has_continuity_gap = false` trên Continuity Item, tính toán nâng điểm Readiness Score của danh mục tương ứng và cập nhật đồng bộ trạng thái hiển thị trên giao diện người dùng.

### 3.7. Yêu Cầu Xử Lý Trường Hợp Biên (Boundary & Edge Cases)

- `[REQ-CARD-021]`: IF người dùng thêm số lượng bước hành động vượt quá giới hạn tối đa cho phép (> 20 bước) hoặc số lượng đầu mối liên hệ vượt quá giới hạn (> 5 đầu mối) trên một Action Card, THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 400 Bad Request kèm thông báo chi tiết vi phạm ngưỡng giới hạn.
- `[REQ-CARD-022]`: IF người dùng truy vấn danh sách Action Cards theo khung thời gian khẩn cấp (`urgency_stage`) hoặc theo danh mục (`category_id`), THEN API SHALL hỗ trợ lọc, tìm kiếm theo từ khóa tiêu đề, và phân trang chuẩn với kích thước trang mặc định là 20 items.

---

## 4. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)

| Mã NFR | Tiêu chí | Số đo & Chỉ số chấp nhận (Target Metric) |
| :--- | :--- | :--- |
| **NFR-SEC-01** | Zero-Knowledge Confidentiality | 100% các chỉ dẫn nhạy cảm được mã hóa tại Client bằng AES-256-GCM. Không lưu trữ Master Key hay plaintext chỉ dẫn trên Server. |
| **NFR-PERF-01** | API Response Latency | Thời gian phản hồi API lấy chi tiết Action Card và danh sách các bước $\le 100\text{ ms}$ tại p95 với 1.000 concurrent users. |
| **NFR-AVAIL-01** | High Availability | SLA dịch vụ API đạt $\ge 99.9\%$ thời gian hoạt động. |
| **NFR-REL-01** | Mutation Idempotency | 100% các API tạo/sửa/xóa hỗ trợ Header `Idempotency-Key` với TTL 24 giờ trên Redis. |
| **NFR-ARCH-01** | Clean Architecture Compliance | 100% mã nguồn Backend triển khai đúng phân lớp Clean Architecture (.NET 8 LTS) và Mobile triển khai BLoC pattern (Flutter 3.x). |

---

## 5. Mô Hình Dữ Liệu Dự Kiến (Data Model Schema - PostgreSQL 16)

### 5.1. Bảng `action_cards` (Thẻ Hành Động Cốt Lõi)

```sql
CREATE TABLE action_cards (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL,
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

CREATE INDEX idx_action_cards_owner_deleted ON action_cards(owner_id, is_deleted);
CREATE INDEX idx_action_cards_urgency ON action_cards(urgency_stage);
CREATE INDEX idx_action_cards_continuity_item ON action_cards(continuity_item_id);
```

### 5.2. Bảng `action_card_steps` (Checklist Các Bước Hành Động)

```sql
CREATE TABLE action_card_steps (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    action_card_id UUID NOT NULL REFERENCES action_cards(id) ON DELETE CASCADE,
    step_order INT NOT NULL,
    instruction VARCHAR(500) NOT NULL,
    estimated_duration VARCHAR(50) NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL
);

CREATE INDEX idx_action_card_steps_card_order ON action_card_steps(action_card_id, step_order);
```

### 5.3. Bảng `action_card_contacts` (Đầu Mối Liên Hệ Khẩn Cấp)

```sql
CREATE TABLE action_card_contacts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    action_card_id UUID NOT NULL REFERENCES action_cards(id) ON DELETE CASCADE,
    contact_name VARCHAR(150) NOT NULL,
    relationship_or_role VARCHAR(100) NOT NULL,
    phone_number VARCHAR(30) NULL,
    email VARCHAR(150) NULL,
    contact_notes VARCHAR(255) NULL,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NULL
);

CREATE INDEX idx_action_card_contacts_card ON action_card_contacts(action_card_id);
```

### 5.4. Bảng `action_card_templates` (Mẫu Thẻ Dựng Sẵn)

```sql
CREATE TABLE action_card_templates (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    template_code VARCHAR(50) NOT NULL UNIQUE,
    category_code VARCHAR(50) NOT NULL,
    title_vi VARCHAR(200) NOT NULL,
    title_en VARCHAR(200) NOT NULL,
    default_urgency VARCHAR(30) NOT NULL,
    default_priority VARCHAR(20) NOT NULL,
    suggested_steps JSONB NOT NULL,
    suggested_roles JSONB NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

---

## 6. Xử Lý Lỗi & Chuẩn Phản Hồi (Error Handling)

### Mã Lỗi Nghiệp Vụ
| Mã Lỗi (Code) | HTTP Status | Kịch bản phát sinh |
| :--- | :--- | :--- |
| `VALIDATION_FAILED` | 400 Bad Request | Thiếu tiêu đề, danh mục sai, khung thời gian khẩn cấp không hợp lệ, vượt quá số bước hoặc số đầu mối liên hệ cho phép. |
| `UNAUTHORIZED_RESOURCE_ACCESS` | 403 Forbidden | Người dùng truy cập hoặc sửa đổi Action Card thuộc quyền sở hữu của tài khoản khác. |
| `ACTION_CARD_NOT_FOUND` | 404 Not Found | Không tìm thấy Action Card theo ID chỉ định hoặc thẻ đã bị xóa mềm. |
| `CONCURRENT_STATE_MUTATION` | 409 Conflict | Xung đột phiên bản cập nhật đồng thời (`row_version` không khớp). |
| `SENSITIVE_DATA_DETECTED` | 422 Unprocessable Entity | Phát hiện số thẻ tín dụng hoặc private key gửi dạng plaintext trong tiêu đề, mô tả hoặc ghi chú. |

---

## 7. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Gherkin Format)

### Scenario 1: Tạo Action Card từ Continuity Item và tự động giải phóng Continuity Gap
```gherkin
Given Người dùng sở hữu một Continuity Item "Khoản vay thế chấp mua nhà Vietcombank" đang có Continuity Gap
When Người dùng chọn "Tạo Thẻ Hành Động" từ item này
And Nhập các bước hành động: "1. Liên hệ cán bộ tín dụng", "2. Chuẩn bị giấy tờ chứng minh thu nhập"
And Gán người phụ trách "Vợ" và vị trí tài liệu "Tủ hồ sơ phòng ngủ"
Then Hệ thống lưu Action Card với mã 201 Created
And Cập nhật liên kết action_card_id trên Continuity Item
And Trạng thái has_continuity_gap của item chuyển thành false
And Điểm Readiness Score của danh mục "FINANCIAL" được cập nhật tăng tương ứng
```

### Scenario 2: Áp dụng Template mẫu có sẵn tự động khởi tạo checklist
```gherkin
Given Người dùng đang ở màn hình tạo mới Action Card
When Người dùng chọn Template mẫu "Mẫu Xử lý Khoản Vay Ngân Hàng"
Then Hệ thống tự động điền sẵn tiêu đề và gợi ý 4 bước hành động chuẩn
And Đặt khung thời gian mặc định là "FIRST_72_HOURS"
And Gợi ý vai trò liên hệ cần thiết là "Cán bộ tín dụng ngân hàng"
```

### Scenario 3: Bảo vệ chỉ dẫn mật mã bằng Zero-Knowledge Encryption
```gherkin
Given Người dùng nhập ghi chú chỉ dẫn khẩn cấp: "Mã số két sắt văn phòng là 889922, chìa khóa phụ giấu sau đồng hồ treo tường"
When Ứng dụng Client (Web/Mobile) gửi dữ liệu lên máy chủ
Then Trường ConfidentialInstructions phải được mã hóa qua AES-256-GCM tại máy khách
And Dữ liệu gửi lên API và lưu trong PostgreSQL chỉ bao gồm cipher_instructions_blob, cipher_nonce, cipher_auth_tag
And Cơ sở dữ liệu tuyệt đối không chứa chuỗi văn bản gốc "889922"
```

### Scenario 4: Chặn lưu trữ chuỗi nhạy cảm unencrypted (Sensitive Data Regex)
```gherkin
Given Người dùng nhập tiêu đề hoặc ghi chú liên hệ chứa chuỗi số thẻ tín dụng hoặc private key
When Người dùng bấm Lưu mà không qua mã hóa Client-Side
Then Hệ thống phát hiện vi phạm bảo mật
And Trả về mã lỗi HTTP 422 Unprocessable Entity với mã lỗi "SENSITIVE_DATA_DETECTED"
And Không có dữ liệu nào được ghi nhận vào cơ sở dữ liệu
```

### Scenario 5: Bảo vệ Idempotency chống trùng lặp Action Card
```gherkin
Given Client gửi yêu cầu POST tạo Action Card kèm Header Idempotency-Key "e812d4a1-..."
When Mạng bị ngắt quãng và Client tự động retry gửi lại cùng Idempotency-Key
Then Hệ thống nhận diện khóa đã tồn tại trong cache Redis
And Trả về phản hồi 201 Created của lần gọi đầu tiên
And Không tạo thêm bản ghi thẻ hành động thứ hai trong PostgreSQL
```

### Scenario 6: Xóa mềm Action Card và các bước liên quan (Cascade Soft-Delete)
```gherkin
Given Người dùng sở hữu một Action Card đang có 3 bước hành động và 2 đầu mối liên hệ
When Người dùng gửi yêu cầu DELETE cho Action Card này
Then Hệ thống cập nhật is_deleted = true cho Action Card
And Tự động cập nhật is_deleted = true cho cả 3 bước hành động và 2 đầu mối liên hệ
And Ghi nhận sự kiện vào continuity_audit_logs
And Thẻ này không còn xuất hiện trong danh sách hiển thị
```

---

## 8. Ngoài Phạm Vi (Out of Scope)

Theo **Decision Framework (Step 5.5.4)**, các tính năng sau được xác định rõ ràng là **Nằm ngoài phạm vi của Module 2 (MVP)**:

1. **Thực thi giao dịch tài chính tự động**: Không tích hợp lệnh thanh toán ngân hàng, chuyển khoản hoặc ủy thác tài chính tự động.
2. **Hệ thống tổng đài tự động gọi điện hoặc gửi SMS cho đầu mối liên hệ**: Không tự động liên hệ hay gửi tin nhắn ra bên ngoài khi chưa có kích hoạt chính thức từ Module 5 (Safe Activation).
3. **Lưu trữ tệp tài liệu số nguyên vẹn (Full Document Storage / Cloud Drive)**: Không xây dựng kho lưu trữ tài liệu đám mây (Google Drive / Dropbox clone); chỉ lưu trữ vị trí hồ sơ và đường dẫn tham chiếu.
4. **Ký số điện tử hoặc Hợp đồng thông minh (Smart Contracts)**: Không cung cấp chức năng ký số văn bản pháp lý.
5. **Định giá tài sản liên quan**: Không tính toán lại giá trị tài sản gắn với thẻ hành động.

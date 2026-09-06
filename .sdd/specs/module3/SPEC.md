# Đặc Tả Kỹ Thuật Chuẩn EARS: Module 3 – Trusted People (Mạng Lưới Người Ủy Thác & Ma Trận Phân Quyền)

**Mã Module:** `module3` (Tương đương `feat-03-trusted-people`)  
**Phiên bản:** v1.0.0 APPROVED  
**Phương pháp áp dụng:** Spec-Driven Development (SDD) & EARS Syntax  
**Cơ chế bảo mật:** Zero-Knowledge, Zero-Disclosure in Normal State & Ephemeral Pairing Code  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module3/CONTEXT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  

---

## 1. Bối Cảnh & Mục Tiêu Kỹ Thuật (Context & Goals)

### 1.1. Bối cảnh
Trong hệ thống quản trị rủi ro và tiếp quản cá nhân Asseta, việc nhận diện các hạng mục quan trọng (Module 1) và chuẩn bị các thẻ hành động khẩn cấp (Module 2) chỉ phát huy giá trị nếu có người phụ trách cụ thể khi xảy ra biến cố. Tuy nhiên, người dùng luôn có tâm lý ngần ngại chia sẻ toàn bộ đời sống tài chính và pháp lý cho một người duy nhất, đồng thời lo sợ dữ liệu bị rò rỉ trước thời hạn khi họ còn đang khỏe mạnh và tự vận hành bình thường.

### 1.2. Mục tiêu kỹ thuật của Module 3
- Quản lý **Mạng lưới Người Ủy Thác (Trusted People Network)**: Cho phép chủ tài sản (`Owner`) thiết lập từ 1 đến tối đa 5 người tin cậy (khuyến nghị 1–3 người trong MVP).
- Thiết lập **Ma Trận Phân Quyền (Scoped Access Matrix)**: Phân quyền tiếp cận theo từng Danh mục (`ContinuityCategory`) hoặc từng Thẻ hành động cụ thể (`ActionCard`) theo nguyên tắc tối thiểu thông tin (Need-to-Know Principle).
- Cơ chế **Ghép Đôi Danh Tính An Toàn (Identity Handshake & Pairing Code)**: Sinh mã ghép đôi 6 ký tự bảo mật (TTL 48h) có cơ chế mã băm một chiều (HMAC-SHA256) và chống tấn công dò quét (Rate-limiting).
- Tuân thủ nguyên tắc **Zero-Disclosure in Normal State**: Trong điều kiện bình thường, Người Ủy Thác chỉ thấy vai trò của mình; tuyệt đối không thể xem trước nội dung chi tiết hay giải mã ghi chú nhạy cảm cho đến khi Module 5 (Safe Activation) được kích hoạt.
- Tích hợp chặt chẽ với **Module 1 & Module 2**: Gán người phụ trách cho `ContinuityItem` và `ActionCard`, tự động giải tỏa các lỗ hổng tiếp quản (`ContinuityGap`) và cập nhật điểm sẵn sàng tiếp quản (`ReadinessScore`).

---

## 2. Tác Nhân & Vai Trò (Actors & Roles)

| Tác nhân (Actor) | Vai trò & Quyền hạn trong Module 3 |
| :--- | :--- |
| **Owner (Chủ tài sản)** | Toàn quyền tạo, chỉnh sửa thông tin hồ sơ, phân quyền trong Ma Trận Phân Quyền, sinh lại mã ghép đôi, tạm ngưng hoặc thu hồi (Revoke/Soft-delete) Người Ủy Thác trong mạng lưới của mình. |
| **Trusted Person (Người ủy thác)** | Cài đặt ứng dụng di động (Flutter) hoặc truy cập Web, nhập mã Pairing Code để liên kết danh tính, đăng ký Public Key thiết bị. Ở trạng thái bình thường chỉ xem được danh sách vai trò mình đảm nhận. Khi có kích hoạt khẩn cấp (Module 5), được cấp quyền xem các danh mục/thẻ đã được phân quyền. |
| **System Worker (Tiến trình nền)** | Quét và hủy bỏ các mã Pairing Code quá hạn 48h, dọn dẹp các yêu cầu ghép đôi thất bại, và tự động đồng bộ hóa trạng thái Continuity Gap khi Người Ủy Thác bị thu hồi hoặc xóa bỏ. |

---

## 3. Yêu Cầu Chức Năng (Functional Requirements - EARS Syntax)

### 3.1. Yêu Cầu Phổ Quát (Ubiquitous Requirements)

- `[REQ-TRUST-001]`: Hệ thống SHALL luôn luôn cô lập dữ liệu Người Ủy Thác, mã ghép đôi và ma trận phân quyền theo mã định danh chủ sở hữu (`OwnerId`), nghiêm cấm tuyệt đối việc truy xuất hoặc chỉnh sửa chéo dữ liệu giữa các tài khoản khác nhau.
- `[REQ-TRUST-002]`: Hệ thống SHALL luôn luôn từ chối tiếp nhận và lưu trữ các trường dữ liệu chứa mật khẩu đăng nhập, mã PIN ngân hàng, mã OTP, số CVV thẻ tín dụng, private key blockchain hoặc seed phrase ở tất cả các tầng API DTO và Database Entity của Module 3.
- `[REQ-TRUST-003]`: Hệ thống SHALL luôn luôn ghi nhận nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác thêm mới, cập nhật hồ sơ, thay đổi quyền hạn ma trận, sinh mã pairing, ghép đôi thành công, tạm ngưng hoặc thu hồi Người Ủy Thác.
- `[REQ-TRUST-004]`: Hệ thống SHALL luôn luôn áp dụng cơ chế Xóa Mềm (`is_deleted = true`, `deleted_at = UtcNow`) khi Owner thực hiện thao tác xóa Người Ủy Thác, đồng thời tự động hủy hiệu lực của bất kỳ mã Pairing Code nào đang tồn tại.

### 3.2. Yêu Cầu Kích Hoạt Theo Sự Kiện (Event-Driven Requirements)

- `[REQ-TRUST-005]`: WHEN Owner tạo mới một Người Ủy Thác (gồm họ tên, số điện thoại, email, mối quan hệ và cấp bậc tin cậy `TrustLevel`), THE hệ thống SHALL kiểm tra tính hợp lệ dữ liệu qua FluentValidation, lưu vào cơ sở dữ liệu với trạng thái `Invited`, sinh mã Pairing Code ngẫu nhiên 6 ký tự bảo mật (có hiệu lực trong 48 giờ) và lưu bản băm HMAC-SHA256 của mã đó vào PostgreSQL.
- `[REQ-TRUST-006]`: WHEN Người Ủy Thác nhập mã Pairing Code hợp lệ trên ứng dụng di động hoặc web, THE hệ thống SHALL đối chiếu mã băm, liên kết mã định danh tài khoản (`DelegateUserId`) với bản ghi `TrustedPerson`, chuyển trạng thái sang `Active`, hủy hiệu lực mã Pairing Code vừa dùng và phát sinh Audit Log `TRUSTED_PERSON_PAIRING_COMPLETED`.
- `[REQ-TRUST-007]`: WHEN Owner cập nhật thông tin Người Ủy Thác (họ tên, số điện thoại, email, vai trò, cấp bậc tin cậy), THE hệ thống SHALL kiểm tra quyền sở hữu, cập nhật dữ liệu, tăng `row_version` và ghi nhận nhật ký kiểm toán.
- `[REQ-TRUST-008]`: WHEN Owner thiết lập hoặc cập nhật Ma Trận Phân Quyền (`ScopedAccessMatrix`) cho một Người Ủy Thác theo danh mục (`category_permissions`) hoặc theo thẻ hành động (`action_card_permissions`), THE hệ thống SHALL lưu trữ danh sách quyền hạn tương ứng trong bảng `trusted_person_permissions`.
- `[REQ-TRUST-009]`: WHEN Owner gửi yêu cầu cấp lại mã ghép đôi mới (Regenerate Pairing Code), THE hệ thống SHALL vô hiệu hóa mã cũ ngay lập tức và sinh mã mới gồm 6 ký tự với thời gian sống (TTL) 48 giờ.
- `[REQ-TRUST-010]`: WHEN Owner thực hiện thao tác thu hồi quyền ủy thác (`Revoke`) hoặc xóa mềm Người Ủy Thác, THE hệ thống SHALL chuyển trạng thái thành `Revoked`, xóa bỏ các phân quyền hiện thời trong `trusted_person_permissions`, đồng thời tự động gán `AssignedTrustedPersonId = NULL` trên các `ContinuityItem` và `ActionCard` liên quan, kích hoạt lại cờ `has_continuity_gap = true` và tính toán giảm điểm `ReadinessScore` tương ứng.

### 3.3. Yêu Cầu Theo Trạng Thái (State-Driven Requirements)

- `[REQ-TRUST-011]`: WHILE Người Ủy Thác ở trạng thái `Invited` (chưa hoàn tất nhập mã ghép đôi), THE hệ thống SHALL không cho phép tài khoản này truy cập bất kỳ dữ liệu nào của Owner.
- `[REQ-TRUST-012]`: WHILE hệ thống ở trạng thái vận hành bình thường (quy trình kích hoạt an toàn Safe Activation chưa kích hoạt), THE hệ thống SHALL duy trì cơ chế bảo vệ Không Tiết Lộ (Zero-Disclosure): Người Ủy Thác chỉ có thể thấy tên vai trò của mình và trạng thái liên kết, tuyệt đối không được phép đọc nội dung chi tiết của Action Cards hay giải mã ghi chú nhạy cảm.
- `[REQ-TRUST-013]`: WHILE ứng dụng di động (Flutter) ở trạng thái ngoại tuyến (`Offline`), THE hệ thống phía Client SHALL cho phép Owner xem danh sách Trusted People và cấu hình phân quyền đã được cache cục bộ từ Local Database bảo mật.

### 3.4. Yêu Cầu Xử Lý Bất Thường & Ngoại Lệ (Unwanted Behavior Requirements)

- `[REQ-TRUST-014]`: IF dữ liệu yêu cầu tạo mới hoặc cập nhật Người Ủy Thác bị thiếu trường bắt buộc (họ tên rỗng hoặc > 150 ký tự, định dạng email không hợp lệ, số điện thoại không hợp lệ, hoặc cấp bậc tin cậy `TrustLevel` nằm ngoài khoảng 1 đến 3), THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi nghiệp vụ `VALIDATION_FAILED`.
- `[REQ-TRUST-015]`: IF một người dùng hoặc thiết bị nhập sai mã Pairing Code quá 3 lần liên tiếp, THEN hệ thống SHALL khóa tạm thời khả năng nhập mã trong vòng 15 phút và trả về mã lỗi HTTP 429 Too Many Requests kèm mã lỗi `PAIRING_ATTEMPTS_EXCEEDED`.
- `[REQ-TRUST-016]`: IF mã Pairing Code đã hết hạn sử dụng (quá 48 giờ) hoặc đã từng được sử dụng trước đó, THEN hệ thống SHALL từ chối ghép đôi và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi `PAIRING_CODE_EXPIRED_OR_INVALID`.
- `[REQ-TRUST-017]`: IF một tài khoản cố gắng ghép đôi bằng chính tài khoản của Owner (`DelegateUserId == OwnerId`), THEN hệ thống SHALL chặn đứng giao dịch và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi `SELF_DELEGATION_PROHIBITED`.
- `[REQ-TRUST-018]`: IF Owner cố gắng thêm mới Người Ủy Thác khi số lượng Người Ủy Thác đang hoạt động (`is_deleted = false`) đã đạt tới giới hạn tối đa 5 người, THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi `MAX_TRUSTED_PEOPLE_EXCEEDED`.
- `[REQ-TRUST-019]`: IF xảy ra xung đột đồng thời khi hai phiên làm việc cùng cập nhật hồ sơ hoặc phân quyền của một Người Ủy Thác (sai lệch `row_version`), THEN hệ thống SHALL từ chối bản ghi đến sau và trả về mã lỗi HTTP 409 Conflict kèm mã lỗi `CONCURRENT_STATE_MUTATION`.
- `[REQ-TRUST-020]`: IF client gửi yêu cầu tạo/sửa với `Idempotency-Key` đã được thực thi thành công trong vòng 24 giờ qua, THEN hệ thống SHALL trả về kết quả đã cache trên Redis mà không tạo bản ghi Người Ủy Thác hoặc mã ghép đôi trùng lặp.

### 3.5. Yêu Cầu Tùy Chọn (Optional Requirements)

- `[REQ-TRUST-021]`: WHERE Người Ủy Thác đăng ký Public Key mật mã từ thiết bị cá nhân (`client_public_key`), THE hệ thống SHALL lưu trữ Public Key này trong bảng `trusted_person_keys` để sẵn sàng tạo bao thư mã hóa E2EE khi bàn giao dữ liệu trong tương lai.

### 3.6. Yêu Cầu Phức Hợp (Complex Requirements)

- `[REQ-TRUST-022]`: WHEN một Trusted Person được gán vào một Continuity Item có mức độ ưu tiên `Critical` hoặc `Important` WHILE Item đó đã có thông tin vị trí tài liệu (`document_location_hint != NULL`), IF Item đó đã có Action Card tương ứng với ít nhất một bước hành động, THEN hệ thống SHALL tự động giải phóng cờ `has_continuity_gap = false` trên Continuity Item, tính toán nâng điểm Readiness Score của danh mục tương ứng và cập nhật đồng bộ trạng thái hiển thị trên giao diện người dùng.

### 3.7. Yêu Cầu Xử Lý Trường Hợp Biên (Boundary & Edge Cases)

- `[REQ-TRUST-023]`: IF Owner cố gắng tạo hai Người Ủy Thác có cùng số điện thoại hoặc cùng địa chỉ email trong cùng một tài khoản, THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 409 Conflict kèm mã lỗi `DUPLICATE_TRUSTED_PERSON_CONTACT`.
- `[REQ-TRUST-024]`: IF Owner hạ cấp bậc tin cậy của một Người Ủy Thác từ `Level 3 (Primary Delegate)` xuống `Level 1 (Notice Only)`, THEN hệ thống SHALL tự động thu hồi toàn bộ các phân quyền truy cập danh mục chi tiết trong bảng `trusted_person_permissions` để bảo đảm tuân thủ nguyên tắc quyền tối thiểu (Least Privilege).

---

## 4. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)

| Mã NFR | Tiêu chí | Số đo & Chỉ số chấp nhận (Target Metric) |
| :--- | :--- | :--- |
| **NFR-SEC-01** | Zero-Knowledge & Zero-Disclosure | 100% dữ liệu nhạy cảm của Action Cards không thể bị đọc hoặc giải mã bởi Người Ủy Thác trước khi có sự kiện kích hoạt khẩn cấp (Module 5). |
| **NFR-SEC-02** | Pairing Code Security | Mã ghép đôi có độ dài tối thiểu 6 ký tự số/chữ (entropy > 30 bits), lưu trữ dưới dạng băm HMAC-SHA256, tự động vô hiệu hóa sau 48h hoặc 3 lần thử sai. |
| **NFR-PERF-01** | API Response Latency | Thời gian phản hồi API lấy danh sách Trusted People và Ma Trận Phân Quyền $\le 100\text{ ms}$ tại p95 với 1.000 concurrent users. |
| **NFR-REL-01** | Mutation Idempotency | 100% các API tạo/sửa/thu hồi Người Ủy Thác hỗ trợ Header `Idempotency-Key` với thời gian tồn tại cache (TTL) là 24 giờ trên Redis. |
| **NFR-ARCH-01** | Clean Architecture Compliance | 100% mã nguồn Backend triển khai đúng phân lớp Clean Architecture (.NET 8 LTS) và Mobile triển khai BLoC pattern (Flutter 3.x). |

---

## 5. Mô Hình Dữ Liệu Chi Tiết (Data Model & Schema)

Cơ sở dữ liệu: **PostgreSQL 16**. Toàn bộ khóa chính sử dụng kiểu `UUIDv4`.

### 5.1. Bảng `trusted_people`
```sql
CREATE TABLE IF NOT EXISTS trusted_people (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    delegate_user_id UUID NULL REFERENCES users(id) ON DELETE SET NULL,
    full_name VARCHAR(150) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone_number VARCHAR(50) NOT NULL,
    relationship VARCHAR(100) NOT NULL,
    role_description VARCHAR(255) NULL,
    trust_level INT NOT NULL DEFAULT 1 CHECK (trust_level BETWEEN 1 AND 3),
    status VARCHAR(50) NOT NULL DEFAULT 'Invited', -- 'Invited', 'Active', 'Suspended', 'Revoked'
    row_version INT NOT NULL DEFAULT 1,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX IF NOT EXISTS idx_trusted_people_owner_status 
ON trusted_people(owner_id, status) WHERE is_deleted = FALSE;

CREATE UNIQUE INDEX IF NOT EXISTS idx_trusted_people_owner_email_unique 
ON trusted_people(owner_id, LOWER(email)) WHERE is_deleted = FALSE;

CREATE UNIQUE INDEX IF NOT EXISTS idx_trusted_people_owner_phone_unique 
ON trusted_people(owner_id, phone_number) WHERE is_deleted = FALSE;
```

### 5.2. Bảng `trusted_person_pairing_codes`
```sql
CREATE TABLE IF NOT EXISTS trusted_person_pairing_codes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trusted_person_id UUID NOT NULL REFERENCES trusted_people(id) ON DELETE CASCADE,
    code_hash VARCHAR(255) NOT NULL,
    salt VARCHAR(64) NOT NULL,
    failed_attempts INT NOT NULL DEFAULT 0,
    lockout_until TIMESTAMPTZ NULL,
    expires_at TIMESTAMPTZ NOT NULL,
    is_used BOOLEAN NOT NULL DEFAULT FALSE,
    used_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_pairing_codes_lookup 
ON trusted_person_pairing_codes(trusted_person_id, is_used, expires_at);
```

### 5.3. Bảng `trusted_person_permissions` (Ma Trận Phân Quyền)
```sql
CREATE TABLE IF NOT EXISTS trusted_person_permissions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trusted_person_id UUID NOT NULL REFERENCES trusted_people(id) ON DELETE CASCADE,
    permission_type VARCHAR(50) NOT NULL, -- 'Category', 'ActionCard'
    target_category_id UUID NULL REFERENCES continuity_categories(id) ON DELETE CASCADE,
    target_action_card_id UUID NULL REFERENCES action_cards(id) ON DELETE CASCADE,
    can_view BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_permission_target CHECK (
        (permission_type = 'Category' AND target_category_id IS NOT NULL AND target_action_card_id IS NULL) OR
        (permission_type = 'ActionCard' AND target_action_card_id IS NOT NULL AND target_category_id IS NULL)
    )
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_category_permission 
ON trusted_person_permissions(trusted_person_id, target_category_id) 
WHERE target_category_id IS NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_card_permission 
ON trusted_person_permissions(trusted_person_id, target_action_card_id) 
WHERE target_action_card_id IS NOT NULL;
```

### 5.4. Bảng `trusted_person_keys` (Đăng Ký Khóa Thiết Bị)
```sql
CREATE TABLE IF NOT EXISTS trusted_person_keys (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trusted_person_id UUID NOT NULL REFERENCES trusted_people(id) ON DELETE CASCADE,
    device_id VARCHAR(100) NOT NULL,
    public_key_spki TEXT NOT NULL,
    algorithm VARCHAR(50) NOT NULL DEFAULT 'ECDH-P256',
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

---

## 6. Xử Lý Lỗi & Định Dạng Envelope Chuẩn (Error Handling)

Tất cả các API tuân thủ định dạng thống nhất theo [shared_context.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/shared_context.md):

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "PAIRING_ATTEMPTS_EXCEEDED",
    "message": "Bạn đã nhập sai mã ghép đôi quá 3 lần. Vui lòng thử lại sau 15 phút.",
    "details": []
  },
  "meta": {
    "timestamp": "2026-09-05T14:30:00Z",
    "correlationId": "8f3b2a1c-..."
  }
}
```

### Bảng Mã Lỗi Nghiệp Vụ Chuẩn Hóa
| HTTP Status | Mã Lỗi (Error Code) | Tình huống phát sinh |
| :--- | :--- | :--- |
| **400** | `VALIDATION_FAILED` | Thiếu họ tên, sai định dạng email, sai SĐT hoặc TrustLevel không hợp lệ. |
| **400** | `PAIRING_CODE_EXPIRED_OR_INVALID` | Mã ghép đôi không khớp, đã dùng hoặc quá hạn 48h. |
| **400** | `SELF_DELEGATION_PROHIBITED` | Owner dùng tài khoản của chính mình để ghép đôi mã ủy thác. |
| **400** | `MAX_TRUSTED_PEOPLE_EXCEEDED` | Cố gắng thêm người thứ 6 trong cùng một tài khoản. |
| **403** | `UNAUTHORIZED_RESOURCE_ACCESS` | Truy xuất hoặc sửa đổi Người Ủy Thác của tài khoản khác. |
| **409** | `DUPLICATE_TRUSTED_PERSON_CONTACT` | Email hoặc SĐT đã tồn tại trong danh sách Người Ủy Thác của Owner. |
| **409** | `CONCURRENT_STATE_MUTATION` | Xung đột phiên bản đồng thời (`row_version`). |
| **429** | `PAIRING_ATTEMPTS_EXCEEDED` | Nhập sai quá 3 lần liên tiếp, khóa 15 phút. |

---

## 7. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Gherkin Checklist)

### Kịch bản 1: Tạo mới Người Ủy Thác & Sinh Mã Ghép Đôi Thành Công
- **Given**: Chủ tài sản `Owner A` đã đăng nhập và đang có 2 Người Ủy Thác trong danh sách.
- **When**: `Owner A` gửi yêu cầu tạo Người Ủy Thác mới với tên "Nguyễn Văn B", email "b@example.com", SĐT "0901234567", quan hệ "Luật sư", TrustLevel = 2.
- **Then**: Hệ thống tạo bản ghi mới với trạng thái `Invited`, sinh mã ghép đôi 6 ký tự viết hoa (ví dụ: `AS7K9P`), lưu bản băm vào DB với thời hạn 48 giờ, và trả về HTTP 201 Created kèm mã ghép đôi.

### Kịch bản 2: Người Ủy Thác Ghép Đôi Danh Tính Bằng Mã Pairing Code
- **Given**: Người Ủy Thác `B` đã cài app Flutter và đăng nhập tài khoản `User B`. Bản ghi ủy thác đang ở trạng thái `Invited`.
- **When**: `User B` nhập đúng mã `AS7K9P` trên giao diện ghép đôi.
- **Then**: Hệ thống xác thực thành công mã băm, cập nhật `delegate_user_id = User B.Id`, chuyển trạng thái thành `Active`, đánh dấu `is_used = true`, ghi Audit Log, và trả về HTTP 200 OK.

### Kịch bản 3: Thiết Lập Ma Trận Phân Quyền Theo Danh Mục
- **Given**: Người Ủy Thác `B` đang ở trạng thái `Active`.
- **When**: `Owner A` phân quyền cho `B` quyền xem 2 danh mục: "Doanh nghiệp" và "Hồ sơ".
- **Then**: Hệ thống cập nhật bảng `trusted_person_permissions` lưu 2 bản ghi tương ứng, trả về HTTP 200 OK kèm danh sách quyền hạn đã cập nhật.

### Kịch bản 4: Thu Hồi Quyền Ủy Thác & Tự Động Kích Hoạt Lại Lỗ Hổng Tiếp Quản
- **Given**: Người Ủy Thác `B` đang được gán phụ trách cho Action Card "Vận hành Công ty" và Continuity Item "Hồ sơ Doanh nghiệp".
- **When**: `Owner A` gửi yêu cầu thu hồi (`Revoke`) Người Ủy Thác `B`.
- **Then**: Trạng thái của `B` chuyển sang `Revoked`, toàn bộ quyền hạn trong ma trận phân quyền bị xóa bỏ, trường `assigned_trusted_person_id` trên Action Card và Continuity Item chuyển về `NULL`, cờ `has_continuity_gap` chuyển thành `true`, điểm `ReadinessScore` tự động giảm tương ứng.

### Kịch bản 5: Chống Tấn Công Dò Quét Mã Ghép Đôi (Rate Limiting)
- **Given**: Bản ghi Pairing Code đang có hiệu lực.
- **When**: Một người dùng nhập sai mã Pairing Code 3 lần liên tiếp.
- **Then**: Hệ thống thiết lập `lockout_until = UtcNow + 15 phút`, từ chối lần thử thứ 4 với HTTP 429 Too Many Requests kèm mã lỗi `PAIRING_ATTEMPTS_EXCEEDED`.

---

## 8. Ngoài Phạm Vi (Out of Scope)

1. **KHÔNG** thực hiện định danh điện tử cấp chính phủ (eKYC, CCCD gắn chip).
2. **KHÔNG** ký số văn bản pháp lý ủy quyền tài sản tự động.
3. **KHÔNG** cung cấp tính năng chat trực tuyến P2P giữa Owner và Người Ủy Thác.
4. **KHÔNG** hỗ trợ tính năng ủy thác đa cấp (Người Ủy Thác chuyển quyền cho người khác).
5. **KHÔNG** hiển thị nội dung chi tiết của Action Cards cho Người Ủy Thác ở trạng thái bình thường (việc này thuộc thẩm quyền của Module 5 – Safe Activation).

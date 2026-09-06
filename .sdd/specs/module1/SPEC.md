# Đặc Tả Kỹ Thuật Chuẩn EARS: Module 1 – Continuity Map (Bản Đồ Tiếp Quản)

**Mã Module:** `module1` (Tương đương `feat-01-continuity-map`)  
**Phiên bản:** v1.0.0 APPROVED  
**Phương pháp áp dụng:** Spec-Driven Development (SDD) & EARS Syntax  
**Cơ chế bảo mật:** Zero-Knowledge & Client-Side Encryption (AES-256-GCM)  

---

## 1. Bối Cảnh & Mục Tiêu (Context & Goal)

### 1.1. Bối cảnh
Người dùng trưởng thành (chủ hộ kinh doanh, chủ doanh nghiệp, người trụ cột tài chính 35–55 tuổi) thường nắm giữ nhiều nghĩa vụ, tài sản, hợp đồng nhưng không được hệ thống hóa. Khi người dùng gặp rủi ro sức khỏe hoặc vắng mặt khẩn cấp, người thân và cộng sự không biết bắt đầu từ đâu.

### 1.2. Mục tiêu kỹ thuật của Module 1
- Cung cấp mô hình dữ liệu và tập hợp API để xây dựng **Bản đồ Tiếp quản (Continuity Map)** bao gồm 6 danh mục chuẩn.
- Hỗ trợ Onboarding Assessment (10–15 câu hỏi) để tự động sinh bản đồ tiếp quản ban đầu.
- Cho phép quản lý danh mục và các hạng mục tiếp quản (Continuity Items) với nhãn định danh gợi nhớ, mức độ ưu tiên, vị trí tài liệu và liên kết người phụ trách.
- Tự động tính toán Chỉ số Sẵn sàng Tiếp quản (Continuity Readiness Score) cấp danh mục và cấp hệ thống theo trọng số toán học xác định.
- Tự động phát hiện các lỗ hổng tiếp quản (Continuity Gaps).
- Tuân thủ nghiêm ngặt nguyên tắc **Zero-Knowledge**: Server không lưu trữ mật khẩu, số dư hay private key. Trường ghi chú mật mã được mã hóa tại Client trước khi gửi lên API.

---

## 2. Tác Nhân & Vai Trò (Actors & Roles)

| Tác nhân (Actor) | Vai trò & Quyền hạn trong Module 1 |
| :--- | :--- |
| **Owner (Chủ tài sản)** | Toàn quyền tạo, đọc, sửa, sắp xếp thứ tự và soft-delete các Continuity Items của chính mình. Toàn quyền kích hoạt tính toán lại Readiness Score. |
| **Trusted Contact (Người ủy thác)** | Không có quyền truy cập trực tiếp vào Continuity Map trong trạng thái vận hành bình thường. Chỉ được tiếp cận từng phần sau khi quy trình Safe Activation (Module 5) kích hoạt thành công. |
| **System Worker (Tiến trình nền)** | Quét và phát hiện các lỗ hổng tiếp quản (Continuity Gaps), tính toán các chỉ số phân tích bất đồng bộ và kiểm toán tính toàn vẹn của dữ liệu. |

---

## 3. Yêu Cầu Chức Năng (Functional Requirements - EARS Syntax)

### 3.1. Yêu Cầu Phổ Quát (Ubiquitous Requirements)

- `[REQ-MAP-001]`: Hệ thống SHALL luôn luôn cô lập dữ liệu Continuity Map theo mã định danh người dùng (`OwnerId`), nghiêm cấm truy xuất dữ liệu chéo giữa các tài khoản khác nhau.
- `[REQ-MAP-002]`: Hệ thống SHALL luôn luôn từ chối tiếp nhận và lưu trữ các trường dữ liệu chứa số dư tài khoản, mã PIN ngân hàng, mã OTP, số CVV thẻ tín dụng, private key blockchain hoặc seed phrase ở tất cả các tầng API DTO và Database Entity.
- `[REQ-MAP-003]`: Hệ thống SHALL luôn luôn ghi nhận nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác tạo mới, cập nhật, thay đổi mức độ ưu tiên hoặc xóa mềm hạng mục tiếp quản.
- `[REQ-MAP-004]`: Hệ thống SHALL luôn luôn áp dụng cơ chế Soft-Delete (`is_deleted = true`, `deleted_at = UtcNow`) khi người dùng thực hiện thao tác xóa Continuity Item, tuyệt đối không xóa cứng khỏi cơ sở dữ liệu.

### 3.2. Yêu Cầu Kích Hoạt Theo Sự Kiện (Event-Driven Requirements)

- `[REQ-MAP-005]`: WHEN người dùng hoàn thành bộ câu hỏi khảo sát khởi tạo (Continuity Assessment Onboarding), THE hệ thống SHALL khởi tạo cấu trúc Continuity Map gồm 6 danh mục mặc định và tạo các Continuity Item gợi ý tương ứng với câu trả lời.
- `[REQ-MAP-006]`: WHEN người dùng gửi yêu cầu tạo mới một Continuity Item, THE hệ thống SHALL kiểm tra tính hợp lệ dữ liệu qua FluentValidation, lưu trữ bản ghi vào PostgreSQL và tính toán cập nhật lại điểm Readiness Score của danh mục tương ứng.
- `[REQ-MAP-007]`: WHEN người dùng cập nhật thông tin một Continuity Item (tên gợi nhớ, mức ưu tiên, vị trí lưu trữ tài liệu, người tiếp quản hoặc bản mã ghi chú), THE hệ thống SHALL kiểm tra quyền sở hữu, cập nhật dữ liệu và phát sinh Audit Log.
- `[REQ-MAP-008]`: WHEN người dùng gửi lệnh xóa một Continuity Item, THE hệ thống SHALL chuyển cờ `is_deleted = true`, loại trừ hạng mục này khỏi cây tính điểm và tính toán lại điểm Readiness Score tổng thể.
- `[REQ-MAP-009]`: WHEN người dùng truy vấn cây thông tin Continuity Map, THE hệ thống SHALL trả về 6 danh mục chuẩn, danh sách các Continuity Item đang hoạt động (`is_deleted = false`), điểm số Readiness Score của từng danh mục và danh sách cảnh báo Continuity Gaps.

### 3.3. Yêu Cầu Theo Trạng Thái (State-Driven Requirements)

- `[REQ-MAP-010]`: WHILE tài khoản người dùng ở trạng thái hoạt động bình thường (`Active`), THE hệ thống SHALL cho phép người dùng thực hiện toàn quyền CRUD và sắp xếp lại vị trí hiển thị (`sort_order`) của các Continuity Items.
- `[REQ-MAP-011]`: WHILE một Continuity Item có mức độ ưu tiên là `Critical` hoặc `Important` nhưng chưa được gán người tiếp quản (`assigned_trusted_person_id IS NULL`) hoặc chưa có vị trí tài liệu (`document_location_hint IS NULL`), THE hệ thống SHALL duy trì trạng thái cờ `has_continuity_gap = true`.
- `[REQ-MAP-012]`: WHILE ứng dụng di động ở trạng thái mất kết nối mạng (`Offline`), THE hệ thống phía Client (Flutter BLoC) SHALL cho phép xem dữ liệu đã cache từ local database và tự động tính toán điểm Readiness Score tức thì trên giao diện.

### 3.4. Yêu Cầu Xử Lý Bất Thường & Ngoại Lệ (Unwanted Behavior Requirements)

- `[REQ-MAP-013]`: IF dữ liệu yêu cầu tạo mới hoặc cập nhật Continuity Item bị thiếu trường bắt buộc (như tên rỗng, danh mục không hợp lệ, mức ưu tiên không đúng quy chuẩn), THEN hệ thống SHALL từ chối lưu và trả về mã lỗi HTTP 400 Bad Request kèm mã lỗi nghiệp vụ `VALIDATION_FAILED`.
- `[REQ-MAP-014]`: IF dữ liệu gửi lên chứa chuỗi văn bản không được mã hóa có dấu hiệu nhận dạng số thẻ tín dụng hoặc private key (kiểm tra Regex tầng Application), THEN hệ thống SHALL chặn đứng giao dịch, ghi log vi phạm an toàn và trả về mã lỗi HTTP 422 Unprocessable Entity với mã `SENSITIVE_DATA_DETECTED`.
- `[REQ-MAP-015]`: IF người dùng cố gắng truy xuất hoặc thao tác trên Continuity Item không thuộc quyền sở hữu của mình, THEN hệ thống SHALL từ chối và trả về mã lỗi HTTP 403 Forbidden với mã `UNAUTHORIZED_RESOURCE_ACCESS`.
- `[REQ-MAP-016]`: IF client gửi yêu cầu có `Idempotency-Key` đã được thực thi thành công trong vòng 24 giờ qua, THEN hệ thống SHALL trả về kết quả đã cache mà không tạo bản ghi trùng lặp trong cơ sở dữ liệu.
- `[REQ-MAP-017]`: IF xảy ra xung đột đồng thời khi hai thiết bị cùng cập nhật một Continuity Item (sai lệch `row_version`), THEN hệ thống SHALL từ chối bản ghi đến sau và trả về mã lỗi HTTP 409 Conflict với mã `CONCURRENT_STATE_MUTATION`.

### 3.5. Yêu Cầu Tùy Chọn (Optional Requirements)

- `[REQ-MAP-018]`: WHERE người dùng nhập trường ghi chú khẩn cấp nhạy cảm (`ConfidentialNote`), THE hệ thống SHALL chỉ chấp nhận dữ liệu này dưới dạng chuỗi đã được mã hóa Client-Side (`cipher_notes_blob`) kèm theo thông số khởi tạo (`cipher_nonce`) và thẻ xác thực (`cipher_auth_tag`).
- `[REQ-MAP-019]`: WHERE một Continuity Item được người dùng lựa chọn chuyển đổi thành Thẻ Hành Động Khẩn Cấp (Action Card - Module 2), THE hệ thống SHALL liên kết khóa ngoại tham chiếu `action_card_id` và duy trì tính toàn vẹn hai chiều.

### 3.6. Yêu Cầu Phức Hợp (Complex Requirements)

- `[REQ-MAP-020]`: WHEN người dùng nâng mức độ ưu tiên của một Continuity Item từ `Low` lên `Critical` WHILE danh mục tương ứng chưa có thông tin người tiếp quản, IF hạng mục này chưa được gán tài liệu chứng minh, THEN hệ thống SHALL tự động tái tính toán giảm điểm Readiness Score của danh mục đó và kích hoạt thông báo cảnh báo Continuity Gap cấp độ cao trên giao diện người dùng.

### 3.7. Yêu Cầu Xử Lý Trường Hợp Biên (Boundary & Edge Cases)

- `[REQ-MAP-021]`: IF một danh mục không chứa bất kỳ Continuity Item nào, THEN hệ thống SHALL gán điểm Readiness Score của danh mục đó bằng 0% và tuyệt đối không làm phát sinh lỗi chia cho 0 (`DivideByZeroException`).
- `[REQ-MAP-022]`: IF người dùng sở hữu số lượng lớn Continuity Items (> 50 items trong một danh mục), THEN API SHALL hỗ trợ phân trang chuẩn với kích thước trang mặc định là 20 items và hỗ trợ lọc theo mức độ ưu tiên (`priority`).

---

## 4. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)

| Mã NFR | Tiêu chí | Số đo & Chỉ số chấp nhận (Target Metric) |
| :--- | :--- | :--- |
| **NFR-SEC-01** | Zero-Knowledge Confidentiality | 100% các ghi chú nhạy cảm phải được mã hóa tại Client bằng AES-256-GCM. Không có khóa giải mã (Master Key) nào được lưu trên Server. |
| **NFR-PERF-01** | API Response Latency | Thời gian phản hồi API lấy toàn bộ Map và tính toán Readiness Score $\le 150\text{ ms}$ tại p95 và $\le 200\text{ ms}$ tại p99 với 1.000 concurrent users. |
| **NFR-AVAIL-01** | High Availability | SLA của API dịch vụ đạt $\ge 99.9\%$ thời gian hoạt động. |
| **NFR-REL-01** | Mutation Idempotency | 100% các API tạo/sửa/xóa hỗ trợ Header `Idempotency-Key` với thời gian tồn tại cache (TTL) là 24 giờ trên Redis. |
| **NFR-ARCH-01** | Clean Architecture Compliance | 100% mã nguồn Backend triển khai đúng phân lớp Clean Architecture (.NET 8 LTS) và Mobile triển khai BLoC pattern (Flutter 3.x). |

---

## 5. Mô Hình Dữ Liệu (Data Model & Schema)

Cơ sở dữ liệu: **PostgreSQL 16**. Toàn bộ khóa chính sử dụng kiểu `UUIDv4`.

### 5.1. Bảng `continuity_categories` (Danh mục Cố định)

```sql
CREATE TABLE continuity_categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) NOT NULL UNIQUE,
    name_vi VARCHAR(100) NOT NULL,
    name_en VARCHAR(100) NOT NULL,
    icon VARCHAR(50) NOT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

*Dữ liệu hạt giống (Seed Data):*
- `FINANCIAL`: Tài chính & Nghĩa vụ tiền tệ
- `PROPERTY`: Tài sản & Bất động sản
- `INSURANCE`: Bảo hiểm & Quyền lợi sức khỏe
- `BUSINESS`: Doanh nghiệp & Quan hệ đối tác
- `DOCUMENTS`: Hồ sơ & Giấy tờ pháp lý
- `FAMILY`: Gia đình & Nghĩa vụ cá nhân

### 5.2. Bảng `continuity_items` (Hạng mục Tiếp quản)

```sql
CREATE TABLE continuity_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL,
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
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ NULL
);

CREATE INDEX idx_continuity_items_owner_deleted ON continuity_items(owner_id, is_deleted);
CREATE INDEX idx_continuity_items_category ON continuity_items(category_id);
```

### 5.3. Bảng `continuity_audit_logs` (Nhật ký Kiểm toán Bất biến)

```sql
CREATE TABLE continuity_audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL,
    item_id UUID NULL,
    action VARCHAR(50) NOT NULL,
    payload_snapshot JSONB NOT NULL,
    ip_address VARCHAR(45) NULL,
    correlation_id UUID NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_continuity_audit_logs_owner ON continuity_audit_logs(owner_id, created_at DESC);
```

### 5.4. Bảng `continuity_assessment_history` (Lịch sử Khảo sát Tiếp quản)

```sql
CREATE TABLE continuity_assessment_history (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    owner_id UUID NOT NULL,
    assessment_version VARCHAR(20) NOT NULL DEFAULT 'v1',
    raw_responses JSONB NOT NULL,
    items_generated_count INT NOT NULL DEFAULT 0,
    initial_readiness_score INT NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_continuity_assessment_history_owner ON continuity_assessment_history(owner_id, created_at DESC);
```

---

## 6. Xử Lý Lỗi & Chuẩn Phản Hồi (Error Handling)

### 6.1. Cấu Trúc Envelope Phản Hồi Thống Nhất

Tất cả các API tuân thủ cấu trúc tại `shared_context.md`:

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "VALIDATION_FAILED",
    "message": "Dữ liệu đầu vào không hợp lệ",
    "details": [
      {
        "field": "name",
        "issue": "Tên hạng mục tiếp quản không được để trống"
      }
    ]
  },
  "meta": {
    "timestamp": "2026-09-05T10:00:00Z",
    "correlationId": "8f3b2075-8b89-436f-b258-89c09c253db1"
  }
}
```

### 6.2. Bảng Mã Lỗi Nghiệp Vụ

| Mã Lỗi (Code) | HTTP Status | Kịch bản phát sinh |
| :--- | :--- | :--- |
| `VALIDATION_FAILED` | 400 Bad Request | Thiếu tên, danh mục không tồn tại, sai định dạng mức ưu tiên. |
| `UNAUTHORIZED_RESOURCE_ACCESS` | 403 Forbidden | Người dùng truy cập hoặc sửa đổi hạng mục không thuộc quyền sở hữu. |
| `CONTINUITY_ITEM_NOT_FOUND` | 404 Not Found | Không tìm thấy hạng mục theo ID chỉ định hoặc item đã bị xóa mềm. |
| `CONCURRENT_STATE_MUTATION` | 409 Conflict | Xung đột phiên bản cập nhật đồng thời (`row_version` không khớp). |
| `SENSITIVE_DATA_DETECTED` | 422 Unprocessable Entity | Phát hiện chuỗi nhạy cảm (số thẻ tín dụng, private key) trong plaintext. |
| `RATE_LIMIT_EXCEEDED` | 429 Too Many Requests | Vượt quá 100 requests/phút trên các endpoint mutation. |

---

## 7. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Gherkin Format)

### Scenario 1: Khởi tạo Continuity Map thành công sau Onboarding Assessment

```gherkin
Given Người dùng mới hoàn thành đăng ký tài khoản và đang ở màn hình Onboarding
When Người dùng trả lời xong 12 câu hỏi khảo sát tiếp quản và bấm "Tạo Bản Đồ"
Then Hệ thống phản hồi mã trạng thái 201 Created
And Trả về danh sách 6 danh mục chuẩn kèm các Continuity Items gợi ý tương ứng
And Điểm Readiness Score ban đầu được tính toán chính xác theo dữ liệu đã nhập
```

### Scenario 2: Thêm mới một Continuity Item quan trọng và phát hiện Continuity Gap

```gherkin
Given Người dùng đã đăng nhập vào hệ thống
When Người dùng gửi yêu cầu thêm mới Continuity Item:
  | Name | "Khoản vay thế chấp mua nhà Vietcombank" |
  | Category | "FINANCIAL" |
  | Priority | "CRITICAL" |
  | DocumentLocation | NULL |
  | AssignedTrustedPerson | NULL |
Then Hệ thống lưu bản ghi mới với mã 201 Created
And Điểm Readiness Score của danh mục "FINANCIAL" được cập nhật
And Hệ thống đánh dấu mục này có Continuity Gap do thiếu hồ sơ và người phụ trách
```

### Scenario 3: Cập nhật vị trí giấy tờ và gán người phụ trách giúp giải tỏa Continuity Gap

```gherkin
Given Người dùng có một Continuity Item đang ở trạng thái có Continuity Gap
When Người dùng cập nhật trường DocumentLocation thành "Tủ hồ sơ phòng làm việc, ngăn 2"
And Gán AssignedTrustedPerson thành một Trusted Contact hợp lệ
Then Hệ thống cập nhật bản ghi với mã 200 OK
And Cờ Continuity Gap của item này chuyển thành false
And Điểm Readiness Score của danh mục tăng lên tương ứng
```

### Scenario 4: Chặn lưu trữ dữ liệu nhạy cảm chưa được mã hóa (Zero-Knowledge Rule)

```gherkin
Given Người dùng đang nhập ghi chú cho Continuity Item
When Người dùng nhập một chuỗi chứa số thẻ tín dụng hoặc private key vào trường plaintext
And Gửi yêu cầu lên API mà không qua mã hóa Client-Side
Then Hệ thống phát hiện vi phạm qua Regex Validation
And Trả về mã lỗi 422 Unprocessable Entity với mã lỗi "SENSITIVE_DATA_DETECTED"
And Không có dữ liệu nào được lưu vào cơ sở dữ liệu
```

### Scenario 5: Cơ chế bảo vệ Idempotency chống trùng lặp dữ liệu

```gherkin
Given Client gửi yêu cầu POST tạo Continuity Item kèm Header Idempotency-Key "d7b27fc2-..."
When Mạng của Client bị ngắt quãng và Client tự động retry gửi lại cùng Idempotency-Key
Then Hệ thống nhận diện khóa đã tồn tại trong bộ đệm Redis
And Trả về kết quả 201 Created của lần gọi đầu tiên
And Không tạo thêm bản ghi thứ hai trong cơ sở dữ liệu
```

### Scenario 6: Xóa mềm Continuity Item và ghi Audit Log

```gherkin
Given Người dùng sở hữu một Continuity Item đang hoạt động
When Người dùng gửi yêu cầu DELETE cho item này
Then Hệ thống cập nhật trường is_deleted thành true
And Ghi nhận bản ghi tương ứng vào bảng continuity_audit_logs
And Hạng mục này không còn xuất hiện trong danh sách hiển thị thông thường
```

---

## 8. Ngoài Phạm Vi (Out of Scope)

Theo **Decision Framework (Step 5.5.4)**, các tính năng sau được xác định rõ ràng là **Nằm ngoài phạm vi của Module 1 (MVP)**:

1. **Kết nối API Ngân hàng / Open Banking**: Không đồng bộ số dư hoặc lịch sử giao dịch thời gian thực.
2. **Quản lý Danh mục Đầu tư & Tỷ suất Sinh lời (P&L)**: Không vẽ biểu đồ nến, không tính toán lãi lỗ chứng khoán hoặc tiền số.
3. **Trình Quản lý Mật khẩu (Password Manager)**: Tuyệt đối không lưu trữ hoặc quản lý mật khẩu tài khoản, OTP, CVV, private key.
4. **Tự Động Định Giá Tài Sản**: Không tích hợp mô hình định giá nhà đất, xe cộ.
5. **Soạn Thảo Di Chúc Pháp Lý Tự Động**: Không xuất bản hoặc tư vấn các văn bản di chúc pháp lý ràng buộc dân sự.
6. **Thực Hiện Giao Dịch Tài Chính Thay Thế**: Không cung cấp chức năng chuyển tiền, ủy quyền thanh toán tự động thay cho người dùng.
7. **Tạo Danh mục Tùy Biến (Custom Category)**: Tạm thời chỉ hỗ trợ 6 danh mục chuẩn trong MVP; tính năng thêm danh mục riêng được chuyển vào Backlog tương lai.

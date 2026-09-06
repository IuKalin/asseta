# Bối Cảnh Nghiệp Vụ & Phạm Vi: Module 2 – Action Cards (Thẻ Hành Động Tiếp Quản)

**Mã Module:** `module2` (Tương đương `feat-02-action-cards`)  
**Pha phát triển:** Pha 0 – Context Discovery  
**Vai trò đảm trách:** Business Analyst & Spec Architect  
**Tài liệu căn cứ:** [ASSETA – MVP PRODUCT CONCEPT.md](file:///c:/DevFlutter/asseta-monorepo/docs/project/ASSETA%20%E2%80%93%20MVP%20PRODUCT%20CONCEPT.md)  
**Trạng thái:** DRAFT / PENDING SPEC REVIEW  

---

## 1. Tuyên Bố Vấn Đề (Problem Statement)

Nếu **Continuity Map (Module 1)** giải quyết bài toán trả lời câu hỏi:
> *"Tôi đang nắm giữ những lĩnh vực, tài sản, nghĩa vụ và mối quan hệ nào có thể bị gián đoạn nếu tôi vắng mặt?"*

thì một danh sách các hạng mục tĩnh là hoàn toàn **chưa đủ** để người tiếp quản có thể hành động trong thực tế. 

Khi một biến cố bất ngờ xảy ra (tai nạn, nằm viện dài ngày, mất năng lực hành vi, mất liên lạc khẩn cấp):
- Người thân hoặc cộng sự có thể nhìn thấy tên một hạng mục như *"Khoản vay thế chấp mua nhà Vietcombank"* hay *"Công ty TNHH Vận tải Minh Phát"*, nhưng họ sẽ rơi vào trạng thái bối rối, hoảng loạn: **Phải bắt đầu từ bước nào? Gặp ai? Hồ sơ gốc nằm ở ngăn kéo nào? Việc gì phải làm ngay trong 24 giờ đầu để tránh bị phạt hợp đồng hay phong tỏa tài khoản?**
- Những thông tin hướng dẫn chi tiết này thường chỉ tồn tại trong thói quen và trí nhớ của người chủ sở hữu. Không có một văn bản bàn giao quy trình nào được lập sẵn một cách ngắn gọn, mạch lạc và dễ tiếp cận.

**Module 2 – Action Cards (Thẻ Hành Động Tiếp Quản)** được định vị là **"Trái tim thực thi" (Execution Core)** của nền tảng Asseta. 

Mỗi Action Card chuyển đổi một vấn đề tiếp quản từ một danh từ tĩnh thành một kịch bản hành động năng động, trả lời chuẩn xác **5 câu hỏi cốt tử**:
1. **Đây là việc gì?** (*What is this?*)
2. **Ai nên xử lý nếu tôi không thể xử lý?** (*Who handles this?*)
3. **Người đó cần làm gì?** (*What should they do? – Thứ tự các bước hành động cụ thể*)
4. **Thông tin hoặc hồ sơ nằm ở đâu?** (*Where is the information? – Vị trí lưu trữ tài liệu vật lý hoặc số*)
5. **Cần liên hệ với ai?** (*Who should they contact? – Danh bạ đầu mối liên hệ khẩn cấp*)

Đồng thời, mỗi Action Card được phân loại vào một **Khung thời gian thực thi (Execution Timeline / Urgency Stage)**:
- `IMMEDIATE`: Cần xử lý ngay trong 0 – 24 giờ.
- `FIRST_72_HOURS`: Cần xử lý trong 72 giờ đầu.
- `FIRST_7_DAYS`: Cần giải quyết trong 7 ngày đầu.
- `LONGER_TERM`: Cần xử lý trong giai đoạn duy trì dài hạn (> 30 ngày).

---

## 2. Tri Thức Miền & Thuật Ngữ Nghiệp Vụ (Domain Knowledge)

| Thuật ngữ | Tiếng Anh | Định nghĩa nghiệp vụ chuẩn xác |
| :--- | :--- | :--- |
| **Thẻ Hành Động** | Action Card | Đơn vị chỉ dẫn tiếp quản độc lập và hoàn chỉnh, chứa toàn bộ quy trình các bước, người phụ trách, đầu mối liên hệ và vị trí tài liệu cho một vấn đề cụ thể. |
| **Bước Hành Động** | Action Step / Checklist Item | Một thao tác đơn lẻ, có thứ tự tuần tự (`step_order`), có thể đánh dấu hoàn thành (`is_completed`) bởi người tiếp quản khi kế hoạch được kích hoạt. |
| **Đầu Mối Liên Hệ** | Key Contact | Thông tin người/tổ chức cần liên hệ để giải quyết vấn đề (Họ tên, vai trò/tổ chức, số điện thoại, email, ghi chú liên hệ). |
| **Giai Đoạn Khẩn Cấp** | Urgency Stage / Timeline | Khung thời gian cần thực hiện: `IMMEDIATE` (0-24h), `FIRST_72_HOURS` (24-72h), `FIRST_7_DAYS` (3-7 ngày), `LONGER_TERM` (>30 ngày). |
| **Mẫu Thẻ Hành Động** | Action Card Template | Kịch bản mẫu dựng sẵn theo từng nhóm nghiệp vụ phổ biến (Vay thế chấp, Doanh nghiệp, Bảo hiểm nhân thọ, Nhà cho thuê, Pháp lý, Nghĩa vụ gia đình). |
| **Chỉ Dẫn Mật Mã** | Confidential Instructions | Các hướng dẫn đặc biệt nhạy cảm (mã khóa két sắt, vị trí cất giữ chìa khóa, lời dặn riêng cho người tiếp quản) bắt buộc phải được mã hóa đầu cuối phía Client. |
| **Liên Kết Tiếp Quản** | Continuity Item Link | Khóa ngoại liên kết 1-1 giữa `ContinuityItem` trong Module 1 và `ActionCard` trong Module 2. |

---

## 3. Ràng Buộc Kỹ Thuật & Kiến Trúc (Constraints)

### 3.1. Ràng buộc Bảo mật & Quyền riêng tư (Zero-Knowledge & Non-Possession)

- **Nguyên tắc Non-Possession**: Tuyệt đối KHÔNG lưu mật khẩu tài khoản, mã PIN thẻ, mã OTP, số thẻ tín dụng CVV, private key ví tiền mã hóa trong Action Card.
- **Mã hóa Phía Client (Client-Side Encryption)**: Nếu người dùng điền trường **Chỉ dẫn mật mã khẩn cấp** (`confidential_instructions`), trường này PHẢI được mã hóa tại Client (Web Crypto API trên React hoặc Dart Cryptography trên Flutter) bằng thuật toán `AES-256-GCM` với khóa Master Key của người dùng. Server chỉ lưu bản mã:
  - `cipher_instructions_blob` (Nội dung mã hóa)
  - `cipher_nonce` (Vector khởi tạo ngẫu nhiên 12 bytes)
  - `cipher_auth_tag` (Thẻ xác thực tính toàn vẹn 16 bytes)
- **Kiểm soát nội dung văn bản mở**: Tầng Application & Middleware tiếp tục kiểm tra Regex (Luhn algorithm và private key pattern) để từ chối các chuỗi nhạy cảm nếu người dùng vô tình nhập vào các trường plaintext (tiêu đề, các bước hành động, đầu mối liên hệ) với mã lỗi HTTP 422 `SENSITIVE_DATA_DETECTED`.

### 3.2. Ràng buộc Kiến trúc & Công nghệ (Tech Stack)

- **Backend (.NET 8 Clean Architecture)**:
  - Triển khai CQRS với MediatR, FluentValidation, EF Core 8 trên PostgreSQL 16.
  - Sử dụng khóa chính UUIDv4 cho toàn bộ các bảng: `action_cards`, `action_card_steps`, `action_card_contacts`, `action_card_templates`.
  - Hỗ trợ Optimistic Concurrency qua trường `row_version (int)` chống xung đột ghi đè.
- **Frontend Web (React 18 + Vite + TypeScript)**:
  - Component thẻ hành động dạng tương tác (Kanban/Timeline view và Card detail modal).
  - Tích hợp Web Crypto API để mã hóa/giải mã trường chỉ dẫn bí mật.
  - Form tạo Action Card thông minh với khả năng tải template mẫu theo danh mục.
- **Mobile App (Flutter 3.x + BLoC)**:
  - BLoC quản lý trạng thái tải, tạo, sửa, sắp xếp thứ tự bước hành động (reorder steps).
  - Khả năng lưu trữ cục bộ (Offline-first) và mã hóa an toàn qua gói `cryptography`.

### 3.3. Ràng buộc Vận hành & Hiệu năng (SLA & Resilience)

- **Idempotency**: 100% các thao tác tạo/sửa/xóa Action Card phải hỗ trợ Header `Idempotency-Key` với thời gian lưu cache 24h trên Redis.
- **Audit Immutability**: Ghi nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác thêm, cập nhật các bước, thay đổi người phụ trách hoặc xóa mềm Action Card.
- **Soft Delete**: Áp dụng cơ chế `is_deleted = true`, `deleted_at = UtcNow` cho Action Card và các bước con liên quan.

---

## 4. Giả Định Nghiệp Vụ & Quyết Định Thiết Kế (Assumptions & Decisions)

Áp dụng nghiêm ngặt theo **Decision Framework (Step 5.5.4)**:

1. **Quan hệ giữa Continuity Item (Module 1) và Action Card (Module 2)**:
   - *Logic gap xử lý*: Người dùng có thể tạo một Action Card từ một Continuity Item có sẵn, hoặc tạo một Action Card độc lập mới.
   - *Quy tắc chuẩn*:
     - Nếu tạo từ Continuity Item: Tự động đồng bộ `category_id`, tên gợi nhớ, `assigned_trusted_person_id`, `document_location_hint`, và cập nhật trường `continuity_items.action_card_id = action_cards.id`.
     - Khi Action Card có đầy đủ: Người phụ trách + Vị trí hồ sơ + Ít nhất 1 bước hành động $\rightarrow$ Cờ `has_continuity_gap` của `ContinuityItem` liên kết sẽ tự động chuyển thành `false`, và tăng Readiness Score.
2. **Cấu trúc Checklist các bước hành động (Action Steps)**:
   - Số lượng bước hành động linh hoạt từ 1 đến 20 bước cho mỗi thẻ.
   - Mỗi bước có trường `step_order` (thứ tự 1, 2, 3...) và nội dung chỉ dẫn ngắn gọn (`instruction`), kèm trường tùy chọn `estimated_duration` (ví dụ: "30 phút", "1 buổi sáng").
3. **Đầu mối liên hệ khẩn cấp (Key Contacts)**:
   - Một Action Card có thể gắn từ 0 đến 5 đầu mối liên hệ cụ thể (ví dụ: Luật sư riêng, Kế toán trưởng, Môi giới bất động sản, Cán bộ tín dụng ngân hàng).
4. **Không lưu trữ tệp đính kèm nặng trong MVP (No Heavy File Storage)**:
   - *Scope Decision*: MVP chỉ lưu trữ chuỗi hướng dẫn vị trí tài liệu vật lý (ví dụ: *"Ngăn kéo tủ hồ sơ bàn làm việc"*) hoặc đường dẫn lưu trữ đám mây số (`digital_storage_link`). Việc upload và lưu trữ file PDF scan hợp đồng nặng trên máy chủ Asseta nằm ngoài phạm vi MVP để tối ưu bảo mật và chi phí lưu trữ.

---

## 5. Ngoài Phạm Vi (Out of Scope)

Theo **Decision Framework (Step 5.5.4)**, các tính năng sau được xác định rõ ràng là **Nằm ngoài phạm vi của Module 2 (MVP)**:

1. **Thực thi giao dịch tài chính tự động**: Không tích hợp lệnh thanh toán ngân hàng, chuyển khoản hoặc ủy thác tài chính tự động.
2. **Hệ thống tổng đài tự động gọi điện hoặc gửi SMS cho đầu mối liên hệ**: Không tự động liên hệ hay gửi tin nhắn ra bên ngoài khi chưa có kích hoạt chính thức từ Module 5 (Safe Activation).
3. **Lưu trữ tệp tài liệu số nguyên vẹn (Full Document Storage / Cloud Drive)**: Không xây dựng kho lưu trữ tài liệu đám mây (Google Drive / Dropbox clone); chỉ lưu trữ vị trí hồ sơ và đường dẫn tham chiếu.
4. **Ký số điện tử hoặc Hợp đồng thông minh (Smart Contracts)**: Không cung cấp chức năng ký số văn bản pháp lý.
5. **Định giá tài sản liên quan**: Không tính toán lại giá trị tài sản gắn với thẻ hành động.

# Bối Cảnh Nghiệp Vụ & Phạm Vi: Module 3 – Trusted People (Mạng Lưới Người Ủy Thác & Ma Trận Phân Quyền)

**Mã Module:** `module3` (Tương đương `feat-03-trusted-people`)  
**Pha phát triển:** Pha 0 – Context Discovery  
**Vai trò đảm trách:** Business Analyst & Spec Architect  
**Tài liệu căn cứ:** [ASSETA – MVP PRODUCT CONCEPT.md](file:///c:/DevFlutter/asseta-monorepo/docs/project/ASSETA%20%E2%80%93%20MVP%20PRODUCT%20CONCEPT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  
**Trạng thái:** DRAFT / READY FOR SPEC REVIEW  

---

## 1. Tuyên Bố Vấn Đề (Problem Statement)

Sau khi hoàn thiện **Continuity Map (Module 1)** và **Action Cards (Module 2)**, người dùng đã nhận diện được các mắt xích phụ thuộc và có hướng dẫn hành động cụ thể cho từng hạng mục quan trọng. Tuy nhiên, một hệ thống tiếp quản cá nhân sẽ trở nên vô nghĩa nếu:

> **"Không có ai được chỉ định nhận trách nhiệm, hoặc người được chỉ định không biết mình có trách nhiệm và không có quyền hạn hợp lệ để tiếp cận thông tin khi biến cố xảy ra."**

Trong thực tế quản trị rủi ro cá nhân và gia đình, người dùng đối mặt với 3 rào cản tâm lý và kỹ thuật cốt tử:

1. **Nỗi sợ "Biết quá nhiều" (All-or-Nothing Dilemma):**  
   Người dùng thường không muốn chia sẻ toàn bộ cuộc sống và tài sản của mình cho một người duy nhất. Vợ/chồng có thể lo việc gia đình và bảo hiểm nhưng không am hiểu cách xử lý công nợ doanh nghiệp; người đồng sáng lập hiểu vận hành công ty nhưng không nên biết các vấn đề tài chính riêng tư của gia đình; kế toán hoặc luật sư chỉ cần tiếp cận đúng hồ sơ thuế và hợp đồng. Nếu chỉ có lựa chọn "cho xem tất cả" hoặc "không cho xem gì", người dùng sẽ chọn **không chia sẻ gì cả**.
2. **Nỗi sợ rò rỉ trước thời hạn (Premature Disclosure Risk):**  
   Người dùng muốn chuẩn bị trước kế hoạch, nhưng tuyệt đối không muốn người được ủy thác nhìn thấy danh sách tài sản, khoản vay, hay vị trí hồ sơ trong khi bản thân vẫn hoàn toàn khỏe mạnh và tự điều hành bình thường.
3. **Sự phức tạp khi kết nối danh tính (Friction in Identity Handshake):**  
   Việc yêu cầu người thân (bố mẹ, vợ/chồng, cộng sự lớn tuổi) phải tạo tài khoản phức tạp, nhớ mật khẩu phụ hoặc thực hiện các thao tác xác thực rườm rà sẽ dẫn đến tỷ lệ kích hoạt thất bại rất cao trong tình huống khẩn cấp.

**Module 3 – Trusted People (Mạng Lưới Người Ủy Thác & Ma Trận Phân Quyền)** ra đời để giải quyết triệt để 3 vấn đề trên theo nguyên tắc **"Need-to-Know & Zero-Knowledge"**:
- Cho phép người dùng thiết lập mạng lưới từ 1 đến 5 người tin cậy (khuyến nghị 1–3 người trong MVP).
- Thiết lập **Ma trận phân quyền tối thiểu (Scoped Access Matrix)**: Mỗi người chỉ được cấp quyền xem các Danh mục hoặc Action Cards liên quan trực tiếp đến vai trò của họ.
- Cơ chế **Ghép đôi danh tính an toàn (Identity Handshake & Pairing Code)**: Kết nối tài khoản ứng dụng di động của người được ủy thác thông qua mã Pairing ngắn hạn (TTL 48h) mà không làm lộ bất kỳ dữ liệu tiếp quản nào trước khi quy trình Safe Activation (Module 5) được kích hoạt.

---

## 2. Tri Thức Miền & Thuật Ngữ Nghiệp Vụ (Domain Knowledge)

| Thuật ngữ | Tiếng Anh | Định nghĩa nghiệp vụ chuẩn xác |
| :--- | :--- | :--- |
| **Người Ủy Thác** | Trusted Person (Delegate) | Cá nhân đáng tin cậy được chủ tài sản (`Owner`) chỉ định trong hệ thống để tiếp nhận hướng dẫn và xử lý một hoặc nhiều phần việc khi xảy ra biến cố. |
| **Mạng Lưới Ủy Thác** | Trusted Network | Tập hợp các cá nhân được chỉ định của một chủ tài sản (MVP giới hạn 1–5 người, tối ưu 1–3 người). |
| **Ma Trận Phân Quyền** | Scoped Access Matrix | Bảng thiết lập phân quyền chi tiết quy định một Người Ủy Thác cụ thể được phép tiếp cận những Danh mục tiếp quản (`ContinuityCategory`) hoặc Thẻ hành động (`ActionCard`) nào. |
| **Cấp Bậc Tin Cậy** | Trust Level / Access Scope Tier | 3 cấp bậc quyền hạn tiếp cận: `Level 1 - Notice Only` (Chỉ nhận cảnh báo khẩn cấp, không xem dữ liệu); `Level 2 - Scoped Delegate` (Chỉ xem các danh mục/thẻ được phân quyền); `Level 3 - Primary Delegate` (Đại diện tiếp quản chính, được phân quyền toàn bộ các hạng mục khi kích hoạt). |
| **Mã Ghép Đôi Danh Tính** | Pairing Code | Mã số định danh bảo mật gồm 6 ký tự chữ/số viết hoa (hoặc Magic Link/QR Code) có hiệu lực 48 giờ (TTL), dùng để liên kết thiết bị/tài khoản của Người Ủy Thác vào hệ thống của Owner. |
| **Vòng Đời Ủy Thác** | Delegate Lifecycle Status | Trạng thái của Người Ủy Thác: `Invited` (Đã gửi mã ghép đôi, chờ nhập), `Active` (Đã xác thực và liên kết danh tính), `Suspended` (Tạm ngưng quyền truy cập), `Revoked` (Thu hồi quyền hoàn toàn). |
| **Nguyên Tắc Tối Thiểu Thông Tin** | Need-to-Know Principle | Quy tắc an ninh thông tin: Không một ai có quyền biết nhiều hơn những gì họ thực sự cần biết để hoàn thành nghĩa vụ tiếp quản của mình. |
| **Bao Thư Mật Mã Ủy Thác** | Delegate Encrypted Envelope | Cấu trúc lưu trữ bản mã của khóa phiên (Wrapped Session Key / Ephemeral Public Key) chỉ có thể được giải mã bởi Private Key của Người Ủy Thác khi giao thức kích hoạt an toàn (Module 5) hoàn tất. |

---

## 3. Ràng Buộc Kỹ Thuật & Kiến Trúc (Constraints)

### 3.1. Ràng buộc Bảo mật & Quyền riêng tư (Zero-Knowledge & Non-Possession)
- **Tuyệt đối Không Tiết Lộ Dữ Liệu Ở Trạng Thái Bình Thường (Zero-Disclosure in Normal State)**:
  - Khi chủ tài sản đang ở trạng thái hoạt động bình thường, Người Ủy Thác chỉ có thể thấy trạng thái liên kết (`Active`) và danh sách các vai trò họ được ủy thác (ví dụ: "Người phụ trách Tài chính Gia đình").
  - Người Ủy Thác **HOÀN TOÀN KHÔNG THỂ** đọc nội dung chi tiết của Action Cards, vị trí hồ sơ vật lý, hay giải mã trường chỉ dẫn bí mật (`confidential_instructions`) cho đến khi Module 5 (Safe Activation) kích hoạt thành công.
- **Bảo Mật Ghép Đôi Danh Tính (Pairing Security)**:
  - Mã Pairing Code gồm 6 ký tự số/chữ ngẫu nhiên tạo bởi `RandomNumberGenerator` chuẩn mật mã (Cryptographically Secure PRNG).
  - Thời hạn hiệu lực: 48 giờ. Mã bị hủy ngay sau khi sử dụng thành công (One-time use).
  - Chống Brute-force: Khóa tạm thời 15 phút nếu nhập sai quá 3 lần liên tiếp từ một địa chỉ IP hoặc thiết bị.
  - Lưu trữ dưới dạng mã băm (HMAC-SHA256 hoặc BCrypt/Argon2) trong cơ sở dữ liệu PostgreSQL; server không lưu plaintext của Pairing Code.
- **Mã Hóa Đầu Cuối (E2EE) & Trao Đổi Khóa Bất Đối Xứng**:
  - Mỗi Người Ủy Thác khi cài đặt ứng dụng (Mobile/Web) sinh một cặp khóa bất đối xứng cục bộ (X25519 hoặc ECDH P-256).
  - Public Key của Người Ủy Thác được đăng ký lên Server; Private Key lưu trong Secure Storage của thiết bị di động (Flutter Keychain/Keystore).
  - Server không bao giờ sở hữu Private Key của Người Ủy Thác.

### 3.2. Ràng buộc Kiến trúc Monorepo & Công nghệ
- **Backend**: ASP.NET Core 8 Web API, MediatR CQRS, Entity Framework Core 8, PostgreSQL 16 (`asseta_db`), Redis 7 cho caching và rate-limiting/idempotency.
- **Frontend Web**: React 18+, Vite, TypeScript, TailwindCSS, WebCrypto API cho mã hóa khóa, TanStack Query.
- **Mobile App**: Flutter 3.x, Clean Architecture + BLoC State Management, `flutter_secure_storage` cho lưu trữ khóa bảo mật.
- **Audit Logging Bất Biến**: Mọi hành vi mời, chấp nhận mã, chỉnh sửa phân quyền, tạm ngưng hoặc thu hồi ủy thác bắt buộc phải ghi log vào bảng `continuity_audit_logs`.

---

## 4. Giả Định Nghiệp Vụ & Câu Hỏi Mở (Assumptions & Open Questions)

### 4.1. Giả định kỹ thuật (Assumptions)
1. **Quy mô mạng lưới**: Trong phạm vi MVP, mỗi chủ tài sản chỉ có nhu cầu ủy thác từ 1 đến 3 người (tối đa 5 người để giữ giao diện trực quan và dễ kiểm soát).
2. **Kênh truyền mã**: Trong MVP, chủ tài sản có thể sao chép mã Pairing Code hoặc chia sẻ đường link mời qua Zalo/SMS/Email cá nhân một cách chủ động (Manual Share) để giảm thiểu chi phí tích hợp SMS Gateway/SendGrid bên thứ ba.
3. **Quan hệ phân quyền**: Một Trusted Person có thể được phân quyền trên:
   - Một hoặc nhiều `ContinuityCategory` (Toàn quyền trong danh mục đó khi kích hoạt).
   - Hoặc từng `ActionCard` cụ thể (Quyền hạn hạt nhân - Granular Scoping).
4. **Tương tác với Module 1 & 2**: Khi một Trusted Person được gán vào `ContinuityItem` hoặc `ActionCard`, cờ `has_continuity_gap` sẽ được giải phóng nếu các điều kiện khác (hồ sơ, bước thực hiện) đã đáp ứng.

### 4.2. Câu hỏi mở cần thẩm định (Open Questions)
- *Câu hỏi 1:* Khi Owner xóa hoặc thu hồi một Trusted Person đang được gán làm người phụ trách trong các Continuity Item / Action Card, hệ thống nên xử lý thế nào?
  - *Quyết định kỹ thuật:* Hệ thống sẽ gán `assigned_trusted_person_id = NULL` trên các items/cards liên quan, đồng thời tự động kích hoạt lại cờ `has_continuity_gap = true` và tính toán giảm điểm `ReadinessScore` tương ứng (Đã tuân thủ Decision Framework: Edge case quan trọng).
- *Câu hỏi 2:* Người Ủy Thác có cần tạo tài khoản mật khẩu đầy đủ trước khi nhập Pairing Code không?
  - *Quyết định kỹ thuật:* Người Ủy Thác chỉ cần cài đặt ứng dụng Flutter/truy cập Web, xác thực số điện thoại/email (OTP cơ bản) và nhập Pairing Code để ghép đôi danh tính.

---

## 5. Ngoài Phạm Vi (Out of Scope)

Nhằm đảm bảo tôn chỉ Hiến pháp Asseta "No Feature Creep" và giữ vững phạm vi MVP:

1. **KHÔNG** triển khai xác minh danh tính cấp độ chính phủ (eKYC, quét chip CCCD, xác thực sinh trắc học công dân).
2. **KHÔNG** triển khai ký số pháp lý hoặc lập văn bản ủy quyền có công chứng tự động.
3. **KHÔNG** triển khai hệ thống nhắn tin trò chuyện trực tiếp (In-app P2P Chat) giữa Owner và Trusted Person.
4. **KHÔNG** hỗ trợ ủy thác đa cấp (Trusted Person ủy quyền tiếp cho người thứ ba).
5. **KHÔNG** tích hợp hệ thống thanh toán thù lao hoặc phân chia tài sản cho người thừa kế.

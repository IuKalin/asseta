# Bối Cảnh Nghiệp Vụ & Phạm Vi: Module 1 – Continuity Map (Bản Đồ Tiếp Quản)

## 1. Tuyên Bố Vấn Đề (Problem Statement)

Trong xã hội hiện đại, một người trưởng thành — đặc biệt là chủ doanh nghiệp nhỏ, chủ hộ kinh doanh, founder hoặc người từ 35–55 tuổi — thường là "điểm nghẽn thông tin" duy nhất nắm giữ toàn bộ huyết mạch vận hành của gia đình và tổ chức:

- Thông tin về các tài khoản thanh toán, nghĩa vụ vay ngân hàng, hợp đồng bảo hiểm, vị trí hồ sơ pháp lý, các khoản chi phí định kỳ và quy trình phê duyệt khẩn cấp thường phân mảnh rải rác: một phần trong trí nhớ, một phần trong điện thoại, một phần trong email cá nhân hoặc chỉ một vài cộng sự thân cận nắm được.
- Khi người trụ cột gặp rủi ro đột ngột (tai nạn bất ngờ, nhập viện điều trị dài ngày, mất năng lực hành vi dân sự tạm thời hoặc vĩnh viễn, mất liên lạc trong tình huống khẩn cấp), toàn bộ hệ thống rơi vào tình trạng đóng băng và hỗn loạn. Người thân và cộng sự không biết việc gì cần ưu tiên xử lý, nghĩa vụ tài chính nào sắp quá hạn, hồ sơ chứng từ lưu trữ ở đâu và cần liên hệ ai.

**Module 1 – Continuity Map (Bản đồ Tiếp quản)** được thiết kế để giải quyết căn nguyên của vấn đề:
Hệ thống không đóng vai trò là một ứng dụng kiểm kê tài sản (Asset Inventory) đòi hỏi số dư tài chính nhạy cảm. Thay vào đó, Continuity Map cung cấp một cấu trúc trực quan phản ánh toàn bộ các mắt xích phụ thuộc vào người dùng, giúp chủ tài sản định hình rõ:

> "Nếu ngày mai tôi không thể trực tiếp xử lý công việc trong 30–60 ngày, những lĩnh vực, tài sản, hồ sơ và nghĩa vụ nào sẽ gặp sự cố?"

Qua đó, hệ thống đo lường Chỉ số Sẵn sàng Tiếp quản (Continuity Readiness Score) và chỉ ra các lỗ hổng tiếp quản (Continuity Gaps) để người dùng chủ động xây dựng kế hoạch ứng phó.

---

## 2. Tri Thức Miền & Thuật Ngữ Nghiệp Vụ (Domain Knowledge)

| Thuật ngữ | Tiếng Anh | Định nghĩa nghiệp vụ chuẩn xác |
| :--- | :--- | :--- |
| **Bản đồ Tiếp quản** | Continuity Map | Cấu trúc phân cấp trực quan mô tả các lĩnh vực, tài sản, nghĩa vụ, đối tác và hồ sơ đang chịu sự điều hành trực tiếp từ chủ tài sản. |
| **Danh mục Tiếp quản** | Continuity Category | 6 nhóm nghiệp vụ cốt lõi cố định trong MVP: Tài chính (Financial), Tài sản (Property), Bảo hiểm (Insurance), Doanh nghiệp (Business), Hồ sơ (Documents), Gia đình (Family). |
| **Hạng mục Tiếp quản** | Continuity Item | Một thực thể cụ thể thuộc Danh mục Tiếp quản (ví dụ: "Khoản vay thế chấp Vietcombank", "Căn hộ chung cư cho thuê", "Hợp đồng nhân thọ Prudential"). |
| **Khảo sát Khởi tạo** | Continuity Assessment | Bộ 10–15 câu hỏi tình huống dạng hội thoại onboarding giúp người dùng nhận diện nhanh các điểm phụ thuộc mà không cần tự điền form phức tạp. |
| **Chỉ số Sẵn sàng** | Continuity Readiness Score | Điểm số phần trăm (0% – 100%) biểu thị mức độ chuẩn bị sẵn sàng tiếp quản của từng danh mục và toàn bộ hồ sơ của người dùng. |
| **Lỗ hổng Tiếp quản** | Continuity Gap | Trạng thái của một hạng mục quan trọng (Critical/Important) nhưng chưa có đầy đủ thông tin bàn giao (thiếu người phụ trách, thiếu vị trí giấy tờ hoặc thiếu hướng dẫn xử lý). |
| **Mức độ Ưu tiên** | Priority Level | Mức độ cấp thiết khi tiếp quản: `Critical` (Sống còn / Ngay lập tức), `Important` (Quan trọng / 72 giờ đầu), `Low Priority` (Ưu tiên thấp / Dài hạn). |
| **Chỉ báo Zero-Knowledge** | Zero-Knowledge Indicator | Cơ chế cam kết không lưu giữ dữ liệu bí mật nguyên bản (số dư, mật khẩu, private key). Mọi ghi chú mô tả chi tiết nhạy cảm đều được mã hóa đầu cuối phía Client trước khi truyền lên Server. |

---

## 3. Ràng Buộc Kỹ Thuật & Kiến Trúc (Constraints)

### 3.1. Ràng buộc Bảo mật & Quyền riêng tư (Zero-Knowledge & Non-Possession)

- **Nguyên tắc Không Sở Hữu Dữ Liệu Nhạy Cảm (Non-Possession)**: Tuyệt đối KHÔNG thu thập, không truyền tải và không lưu trữ trên máy chủ các thông tin: số dư tài khoản ngân hàng, mật khẩu internet banking, mã PIN, mã OTP, số CVV thẻ tín dụng, private key blockchain, hoặc seed phrase.
- **Mã hóa Phía Client (Client-Side Encryption)**: Trường ghi chú bổ sung (`ConfidentialNote`) hoặc gợi ý định danh bắt buộc phải được mã hóa tại thiết bị người dùng (Web/Mobile) bằng thuật toán AES-256-GCM trước khi gửi qua API. Server ASP.NET Core và cơ sở dữ liệu PostgreSQL chỉ lưu trữ dạng `CipherBlob`. Khóa mã hóa dẫn xuất từ Master Key do người dùng sở hữu.
- **Định danh Tối giản**: Tên hạng mục chỉ lưu dưới dạng nhãn nhận diện gợi nhớ (ví dụ: "Khoản vay mua nhà số đuôi *1234", tuyệt đối không lưu hợp đồng tín dụng chứa mã số bảo mật trừ khi đã được mã hóa client-side).

### 3.2. Ràng buộc Kiến trúc & Công nghệ (Tech Stack)

- **Backend**:
  - ASP.NET Core 8 Web API tuân thủ Clean Architecture (Domain, Application, Infrastructure, Api).
  - Triển khai CQRS pattern sử dụng MediatR.
  - Validation nghiệp vụ bằng FluentValidation trước khi qua Command Handler.
  - Cơ sở dữ liệu: PostgreSQL 16 quản lý qua Entity Framework Core 8.
  - Định danh khóa chính toàn bộ bằng UUIDv4.
- **Frontend Web**:
  - ReactJS 18+ với TypeScript, Vite, TailwindCSS.
  - Quản lý State & Server Cache với TanStack Query.
- **Mobile App**:
  - Flutter 3.x với Dart 3.x, Clean Architecture + BLoC State Management.
  - Thư viện HTTP Dio có interceptor đính kèm Idempotency-Key.

### 3.3. Ràng buộc Vận hành & Hiệu năng (SLA & Resilience)

- **Idempotency**: Mọi thao tác ghi dữ liệu (POST, PUT, DELETE) đối với Continuity Item phải truyền Header `Idempotency-Key` (UUIDv4). Nếu nhận trùng key trong 24 giờ, hệ thống trả về kết quả đã lưu mà không ghi đè dữ liệu.
- **Audit Immutability**: Không xóa cứng dữ liệu (Hard Delete). Áp dụng Soft-Delete (`is_deleted = true`) và ghi nhận Audit Log bất biến cho mọi thao tác thêm, cập nhật hoặc đánh dấu hoàn tất hạng mục.
- **Thời gian phản hồi**: API tính toán Continuity Readiness Score và hiển thị bản đồ phải phản hồi < 200ms (p99).

---

## 4. Giả Định Nghiệp Vụ & Quyết Định Thiết Kế (Assumptions & Decisions)

Áp dụng nghiêm ngặt theo **Decision Framework (Step 5.5.4)**:

1. **Công thức tính Continuity Readiness Score**:
   - *Logic gap xử lý*: Readiness Score không được tính ngẫu nhiên hay phụ thuộc bên ngoài.
   - *Quy tắc chuẩn*:
     $$\text{Readiness Score} = \sum (\text{Trọng số của Item} \times \text{Tỷ lệ hoàn thiện của Item})$$
     Trong đó hạng mục có độ ưu tiên `Critical` chiếm 50% tổng điểm danh mục, `Important` chiếm 35%, `Low Priority` chiếm 15%. Một Item được tính 100% hoàn thiện khi có: Tên gợi nhớ + Mức ưu tiên + Vị trí hồ sơ + Người tiếp quản được phân công.
2. **Khả năng hoạt động Ngoại tuyến (Offline-First Calculation)**:
   - *Ambiguity xử lý*: Khi người dùng di động không có mạng, ứng dụng Flutter tự động tính toán Readiness Score trên Local BLoC State theo cùng công thức của Backend, sau đó tự động hòa giải (reconciliation) khi trực tuyến.
3. **Cố định 6 Danh mục Chuẩn trong MVP**:
   - *Scope Decision*: Không cho phép người dùng tự tạo danh mục tùy biến (Custom Category) trong giai đoạn MVP nhằm đảm bảo chuẩn hóa thuật toán tính điểm và trải nghiệm onboarding đồng nhất. Tính năng Custom Category được đưa vào Backlog tương lai.
4. **Bảo toàn tính nhất quán khi chuyển đổi sang Action Card**:
   - Mỗi Continuity Item chứa trường tham chiếu tùy chọn `ActionCardId (UUID nullable)`. Khi người dùng tạo Action Card từ Continuity Item (Module 2), quan hệ 1-1 sẽ được thiết lập tự động.

---

## 5. Ngoài Phạm Vi (Out of Scope)

Các tính năng sau tuyệt đối **KHÔNG** thuộc phạm vi của Module 1 trong giai đoạn MVP:

1. **Kết nối Open Banking & Đồng bộ Số dư Thực tế**: Không tích hợp API ngân hàng, không hiển thị số dư biến động thời gian thực.
2. **Quản lý Danh mục Đầu tư & Lãi Lỗ**: Không cung cấp biểu đồ nến, tỷ suất sinh lời (P&L), quản lý danh mục cổ phiếu hay tiền mã hóa.
3. **Lưu trữ Mật khẩu & Bí mật Truy cập**: Không đóng vai trò là trình quản lý mật khẩu (Password Manager); không lưu OTP, CVV, private key, seed phrase.
4. **Định giá Tự động Tài sản**: Không tích hợp AI hoặc API định giá nhà đất, xe cộ, cổ phần.
5. **Soạn thảo Di chúc Pháp lý & Phân chia Di sản**: Không thay thế văn bản pháp lý di chúc hoặc các dịch vụ tư vấn luật thừa kế.
6. **Thanh toán Hóa đơn hoặc Ủy nhiệm Chi Tự động**: Không thực hiện giao dịch tài chính thay cho người dùng.

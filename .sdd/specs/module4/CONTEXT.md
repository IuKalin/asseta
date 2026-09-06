# Bối Cảnh Nghiệp Vụ & Phạm Vi: Module 4 – Continuity Plan (Kế Hoạch Tiếp Quản Tổng Thể Theo Trục Thời Gian)

**Mã Module:** `module4` (Tương đương `feat-04-continuity-plan`)  
**Pha phát triển:** Pha 0 – Context Discovery  
**Vai trò đảm trách:** Business Analyst & System Spec Architect  
**Tài liệu căn cứ:** [ASSETA – MVP PRODUCT CONCEPT.md](file:///c:/DevFlutter/asseta-monorepo/docs/project/ASSETA%20%E2%80%93%20MVP%20PRODUCT%20CONCEPT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  
**Trạng thái:** DRAFT / READY FOR SPEC REVIEW  

---

## 1. Tuyên Bố Vấn Đề (Problem Statement)

Sau khi hoàn tất việc lập bản đồ tiếp quản (**Continuity Map - Module 1**), soạn thảo các hướng dẫn tiếp quản chi tiết (**Action Cards - Module 2**), và chỉ định mạng lưới người tin cậy kèm phân quyền tối thiểu (**Trusted People - Module 3**), người dùng đã có đầy đủ các mảnh ghép dữ liệu.

Tuy nhiên, trong một tình huống biến cố đột ngột (chủ tài sản nằm viện, tai nạn, mất liên lạc khẩn cấp), việc cung cấp cho người tiếp quản một danh sách rời rạc gồm hàng chục thẻ hành động được phân chia theo danh mục (Tài chính, Bất động sản, Doanh nghiệp...) sẽ tạo ra sự **tê liệt vì quá tải thông tin (Information Overload & Paralysis)**:

> **"Trong cơn khủng hoảng, người tiếp quản không thể và không nên ngồi lọc từng danh mục tài sản để đoán xem việc gì phải làm trước, việc gì có thể chờ. Họ cần biết chính xác: NGAY BÂY GIỜ phải làm gì, 72 GIỜ TỚI cần xử lý việc gì, 7 NGÀY ĐẦU cần gặp ai, và 30 NGÀY TIẾP THEO cần duy trì những gì."**

Một kế hoạch tiếp quản thực sự có giá trị không phải là một bảng kiểm kê tài sản tĩnh, mà là một **Kế hoạch hành động theo trục thời gian (Time-Sequenced Action Plan)**:

1. **Khủng hoảng thứ tự ưu tiên (Chronological Priority Void):**  
   Một khoản lãi vay ngân hàng đến hạn thanh toán trong 24h hoặc một hợp đồng kinh doanh cần chữ ký ủy quyền trong 48h có mức độ khẩn cấp vượt trội so với việc kiểm tra quy hoạch một mảnh đất cho thuê (vốn có thể chờ 30 ngày). Nếu không tổ chức theo thời gian, người tiếp quản rất dễ xử lý sai thứ tự, dẫn đến phạt chậm trả, nợ xấu hoặc phá vỡ hợp đồng.
2. **Lỗ hổng điều phối thời gian thực (Execution Timeline Gaps):**  
   Nhiều thẻ hành động thuộc giai đoạn tối khẩn cấp ("Immediate") nhưng bị bỏ quên: chưa gán người phụ trách, chưa có chỉ dẫn vị trí hợp đồng hoặc thiếu thông tin liên hệ của đầu mối khẩn cấp.
3. **Rủi ro rò rỉ khi xem tổng thể (Summary Exposure Risk):**  
   Giao diện kế hoạch tổng thể phải tuân thủ nghiêm ngặt nguyên tắc **Zero-Knowledge & Zero-Disclosure**: Bản tóm tắt kế hoạch phải cho phép chủ tài sản (Owner) kiểm soát toàn diện, đồng thời cho phép tạo ra bản tóm lược khẩn cấp (Emergency Brief / Offline Summary) an toàn mà không làm lộ các thông tin giải mật hoặc vị trí nhạy cảm khi chưa kích hoạt khẩn cấp (Module 5).

**Module 4 – Continuity Plan** là trái tim điều phối của Asseta, chuyển đổi các thẻ hành động tĩnh thành một **Kế Hoạch Tiếp Quản Cá Nhân Tổng Thể** được tổ chức theo 4 mốc thời gian thực thi then chốt:
- **Giai đoạn 1: Immediate Actions (NOW)** – Xử lý tức thì (0 – 24 giờ).
- **Giai đoạn 2: First 72 Hours** – Ổn định và giải quyết nghĩa vụ cấp thiết (24 – 72 giờ).
- **Giai đoạn 3: First 7 Days** – Xử lý các tổ chức tài chính, đối tác và pháp lý (3 – 7 ngày).
- **Giai đoạn 4: Longer-Term Continuity (Next 30 Days & Beyond)** – Vận hành và bàn giao dài hạn (sau 7 ngày đến 30 ngày).

---

## 2. Tri Thức Miền & Thuật Ngữ Nghiệp Vụ (Domain Knowledge)

| Thuật ngữ | Tiếng Anh | Định nghĩa nghiệp vụ chuẩn xác |
| :--- | :--- | :--- |
| **Kế Hoạch Tiếp Quản Tổng Thể** | Personal Continuity Plan | Bức tranh toàn cảnh tổng hợp toàn bộ các Thẻ hành động (`ActionCard`) của chủ tài sản, được sắp xếp và điều phối theo 4 giai đoạn thời gian thực thi khẩn cấp. |
| **Giai Đoạn Khẩn Cấp (Trục Thời Gian)** | Urgency Stage / Timeline Stage | Phân đoạn thời gian phản ứng khi xảy ra tình huống tiếp quản, bao gồm: `IMMEDIATE` (Ngay lập tức), `FIRST_72_HOURS` (72 giờ đầu), `FIRST_7_DAYS` (7 ngày đầu), `LONGER_TERM` (Dài hạn 30 ngày). |
| **Lỗ Hổng Giai Đoạn** | Stage Continuity Gap | Tình trạng một Thẻ hành động trong giai đoạn (đặc biệt là Immediate hoặc 72 Hours) bị thiếu người phụ trách (`AssignedTrustedPersonId == null`) hoặc thiếu chỉ dẫn vị trí giấy tờ (`DocumentLocationHint == null`). |
| **Chỉ Số Sẵn Sàng Kế Hoạch** | Plan Readiness & Coverage Index | Bộ chỉ số đánh giá mức độ hoàn thiện của kế hoạch: Tỷ lệ thẻ có người phụ trách (Delegate Coverage), Tỷ lệ thẻ có vị trí giấy tờ (Document Readiness), và Điểm sẵn sàng theo từng giai đoạn thời gian. |
| **Bản Tóm Lược Khẩn Cấp Ngoại Tuyến** | Offline Emergency Brief | Bản trích xuất tổng hợp dạng tóm tắt bảo mật (Zero-Knowledge Safe Summary) liệt kê thứ tự các việc cần làm, đầu mối liên hệ khẩn cấp và tên người phụ trách theo từng mốc thời gian, có thể lưu trữ offline hoặc in ra giấy an toàn mà không chứa nội dung giải mật. |
| **Điểm Nghẽn Trách Nhiệm Đơn Lẻ** | Single Point of Failure (SPoF) Risk | Rủi ro phát hiện trong kế hoạch khi một Người Ủy Thác duy nhất bị gán quá nhiều thẻ hành động khẩn cấp (> 70% tổng số thẻ của giai đoạn Immediate/72h), tạo nguy cơ quá tải nếu người đó cũng gặp sự cố. |
| **Điều Chỉnh Giai Đoạn Thẻ** | Stage Reassignment / Timeline Tuning | Khả năng của Chủ tài sản chuyển đổi giai đoạn khẩn cấp của một thẻ (ví dụ đưa thẻ từ `FIRST_72_HOURS` lên `IMMEDIATE` hoặc ngược lại) để tối ưu hóa kịch bản tiếp quản. |

---

## 3. Ràng Buộc Kỹ Thuật & Kiến Trúc (Constraints)

### 3.1. Ràng buộc Bảo mật & Quyền riêng tư (Zero-Knowledge & Zero-Disclosure)
- **Tôn Trọng Tuyệt Đối Nguyên Tắc Zero-Knowledge**:
  - Bản kế hoạch Continuity Plan khi hiển thị tổng quan không bao giờ giải mã hoặc làm lộ trường bí mật (`CipherInstructions`).
  - Bản tóm lược ngoại tuyến (Offline Emergency Brief) chỉ bao gồm: Tiêu đề thẻ, Tên giai đoạn, Mức độ ưu tiên, Tên người phụ trách, Đầu mối liên hệ ngoại vi (Action Card Contacts), và Gợi ý vị trí vật lý chung (`DocumentLocationHint`). Tuyệt đối không trích xuất ciphertext hoặc yêu cầu nhập master key để xem tổng quan timeline.
- **Cô lập dữ liệu đa người dùng (Multi-tenant Isolation)**:
  - Mọi truy vấn kế hoạch bắt buộc lọc theo `OwnerId` từ ClaimsPrincipal của JWT Token đã chứng thực.
  - Người ủy thác (Delegate) ở trạng thái bình thường (Normal State) khi truy cập chỉ xem được danh sách thẻ họ được phân quyền trực tiếp, được nhóm theo 4 giai đoạn thời gian (Role-Scoped Timeline View).

### 3.2. Ràng buộc Kiến trúc Clean Architecture & CQRS
- **Backend (.NET 8 LTS)**:
  - Tận dụng triệt để Entity `ActionCard` với `UrgencyStage` đã có từ Module 2 và quan hệ với `TrustedPerson` từ Module 3.
  - Tạo Query chuyên biệt `GetContinuityPlanQuery` để tổng hợp, phân nhóm, tính toán chỉ số bao phủ (Coverage Metrics) và phát hiện Stage Gaps với hiệu năng cao (Single round-trip database query qua EF Core projection).
  - Cung cấp Command `UpdateActionCardStageCommand` cho phép kéo-thả hoặc chuyển đổi giai đoạn nhanh chóng.
  - Cung cấp Query `GetPlanReadinessAuditQuery` phân tích rủi ro SPoF và thống kê lỗ hổng theo từng mốc thời gian.
- **Frontend Web (React 18 + Vite + TypeScript)**:
  - Giao diện Timeline View và Kanban Stage Columns với 4 cột/tab trực quan (`NOW`, `FIRST 72 HOURS`, `FIRST 7 DAYS`, `NEXT 30 DAYS`).
  - Thẻ thông số KPI tổng quan (Plan Coverage Score, Gaps Identified, SPoF Alert).
  - Bộ lọc theo Người phụ trách (Filter by Assignee) và Danh mục (Category).
- **Mobile App (Flutter 3.x + BLoC)**:
  - Màn hình `ContinuityPlanPage` tổ chức theo Timeline cuộn dọc hoặc Segmented Tabs với huy hiệu màu sắc tương ứng từng mốc thời gian.
  - Cảnh báo trực quan nếu giai đoạn `NOW` có việc chưa được phân công.
  - Hỗ trợ xem offline hoàn toàn thông qua Local Cache (SQLite / HydratedBloc).

---

## 4. Giả Định Nghiệp Vụ & Quyết Định Thiết Kế (Assumptions & Decisions)

### 4.1. Giả định nghiệp vụ (Assumptions)
1. **Phân loại 4 giai đoạn**: Bốn mốc thời gian `IMMEDIATE`, `FIRST_72_HOURS`, `FIRST_7_DAYS`, `LONGER_TERM` phản ánh chính xác chuẩn mực ứng phó sự cố quốc tế và tâm lý học tiếp quản khẩn cấp (Emergency Response Protocols).
2. **Nguồn gốc dữ liệu**: Continuity Plan không tạo thêm bảng dữ liệu độc lập lưu thẻ mới, mà tổng hợp và điều phối trực tiếp các `ActionCard` đã được tạo từ Module 2 và gán `TrustedPerson` từ Module 3.
3. **Mức độ ưu tiên trong từng giai đoạn**: Các thẻ trong mỗi giai đoạn được tự động sắp xếp theo thứ tự ưu tiên giảm dần: `CRITICAL` &rarr; `HIGH` &rarr; `MEDIUM` &rarr; `LOW`.

### 4.2. Quyết định kỹ thuật theo Decision Framework (Step 5.5.4)
- **Logic gap: Thẻ chưa gán người hoặc chưa có tài liệu trong giai đoạn Immediate**:
  - *Quyết định:* Hệ thống tự động đánh dấu cờ `HasStageGap = true` và hiển thị cảnh báo đỏ trên UI để hướng dẫn chủ tài sản xử lý dứt điểm trước khi kích hoạt.
- **Edge case: Người ủy thác bị thu hồi (Revoked) khiến thẻ trong Plan mất người phụ trách**:
  - *Quyết định:* Do Module 3 đã tự động unassign `AssignedTrustedPersonId = null`, Continuity Plan sẽ ngay lập tức phản ánh lỗ hổng này vào báo cáo Audit và giảm điểm bao phủ `DelegateCoveragePercentage`.
- **Ambiguity: Kế hoạch có thể in ra PDF hoặc lưu offline không?**:
  - *Quyết định:* Cung cấp endpoint và UI cho `Emergency Brief` (dạng Markdown/JSON sạch để Web/Mobile render thành chế độ in ấn Print/PDF View tiện lợi).

---

## 5. Ngoài Phạm Vi (Out of Scope)

Nhằm tuân thủ nguyên tắc "Zero Feature Creep" của Hiến pháp Asseta:

1. **KHÔNG** tích hợp dịch vụ SMS/Zalo tự động gửi tin nhắn theo từng mốc thời gian (Tính năng gửi cảnh báo tự động thuộc về Module 5: Safe Activation).
2. **KHÔNG** triển khai tích hợp AI sinh lịch biểu tự động với Google Calendar / Outlook Calendar trong MVP.
3. **KHÔNG** hỗ trợ tính toán dòng tiền chi tiết hoặc dự báo tài chính theo từng giai đoạn (Asseta không phải ứng dụng quản lý ngân sách).
4. **KHÔNG** cho phép người ủy thác tự ý đổi giai đoạn khẩn cấp của thẻ (Chỉ Chủ tài sản `Owner` có quyền điều phối kế hoạch).

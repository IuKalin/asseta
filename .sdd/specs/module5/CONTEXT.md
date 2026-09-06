# Bối Cảnh Nghiệp Vụ & Phạm Vi: Module 5 – Safe Activation & Dead Man's Switch Protocol

**Mã Module:** `module5` (Tương đương `feat-05-safe-activation`)  
**Pha phát triển:** Pha 0 – Context Discovery  
**Vai trò đảm trách:** Business Analyst & Security Protocol Architect  
**Tài liệu căn cứ:** [ASSETA – MVP PRODUCT CONCEPT.md](file:///c:/DevFlutter/asseta-monorepo/docs/project/ASSETA%20%E2%80%93%20MVP%20PRODUCT%20CONCEPT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  
**Trạng thái:** DRAFT / READY FOR SPEC REVIEW  

---

## 1. Tuyên Bố Vấn Đề (Problem Statement)

Sau khi hoàn tất cả 4 Module nền tảng:
- **Module 1 (Continuity Map)**: Nhận diện các lĩnh vực tài sản/nghĩa vụ sống còn.
- **Module 2 (Action Cards)**: Soạn thảo hướng dẫn tiếp quản chi tiết kèm mã hóa bí mật Client-Side.
- **Module 3 (Trusted People)**: Thiết lập mạng lưới ủy thác, phân cấp Level 1–3 và ma trận phân quyền tối thiểu (Scoped Access Matrix).
- **Module 4 (Continuity Plan)**: Tổ chức và điều phối hành động theo 4 giai đoạn khẩn cấp (24h, 72h, 7d, 30d) và phân tích điểm nghẽn SPoF.

Hệ thống Asseta đứng trước bài toán sinh tử và nhạy cảm nhất của toàn bộ giải pháp:
> **"Chính xác khi nào và bằng cơ chế nào, Kế Hoạch Tiếp Quản được mở khóa cho Người Ủy Thác mà không xảy ra báo động giả, không bị chiếm quyền đoạt tài sản, và không làm lộ bí mật khi Chủ tài sản vẫn khỏe mạnh?"**

### 1.1. Sự thất bại của các cơ chế Dead Man's Switch ngây thơ (Naive Dead Man's Switch)
Một số giải pháp truyền thống áp dụng nguyên tắc: *"Nếu người dùng không đăng nhập trong 30 ngày &rarr; Tự động gửi toàn bộ tài liệu cho người thân."*
Cơ chế này tiềm ẩn những thảm họa thực tế:
1. **Báo động giả thường xuyên (False Positives):** Chủ tài sản đi công tác vùng sâu vùng xa mất sóng, đi du lịch nghỉ dưỡng không dùng điện thoại, hoặc đơn giản là quên đăng nhập trong 30 ngày. Toàn bộ bí mật kinh doanh, di chúc, và tài sản bị bung bét gửi đến người thân khi chủ tài sản vẫn hoàn toàn bình thường.
2. **Chiếm đoạt tài sản và áp lực tống tiền (Malicious Takeover & Hostile Activation):** Người ủy thác có thể tìm cách lừa gạt hệ thống hoặc kích hoạt cưỡng bức khi chủ tài sản gặp trục trặc tạm thời.
3. **Mở khóa ồ ạt toàn bộ (All-or-Nothing Exposure):** Ngay khi kích hoạt, toàn bộ thông tin tài sản nhạy cảm bị phơi bày cho tất cả mọi người, vi phạm triệt để nguyên tắc Phân quyền tối thiểu (Least Privilege).

### 1.2. Giải pháp của Asseta: Safe Activation Protocol (Giao Thức Kích Hoạt An Toàn)
Asseta MVP giải quyết triệt để vấn đề này bằng một **Quy trình kích hoạt đa tầng (Multi-Step Time-Locked Verification Protocol)**:
1. **Kiểm tra sức sống định kỳ (Vitality Check-in / Heartbeat):** Định kỳ (ví dụ 30 ngày), hệ thống gửi thông báo nhẹ nhàng *"Everything okay?"*. Chủ tài sản chỉ cần 1 chạm để xác nhận còn sống và khỏe mạnh.
2. **Kích hoạt kép (Dual Triggering Modes):** Kế hoạch có thể được kích hoạt bởi:
   - **Tự động theo thời gian (Heartbeat Timeout):** Quá hạn check-in + thời gian gia hạn nhắc nhở mà không có phản hồi.
   - **Thủ công từ người ủy thác (Emergency Request):** Người ủy thác cấp cao (Level 2/3) gửi yêu cầu kích hoạt khi biết Chủ tài sản gặp tai nạn hoặc nằm viện khẩn cấp.
3. **Cửa sổ đệm thời gian an toàn (Time-Lock Grace Period):** Khi có kích hoạt, hệ thống **chưa bao giờ mở khóa ngay lập tức**. Một khoảng đếm ngược bảo vệ (ví dụ 48 giờ) bắt đầu. Cảnh báo khẩn cấp được gửi đến Chủ tài sản qua SMS, Email, và In-App Push.
4. **Quyền Hủy Khẩn Cấp 1-Chạm Của Chủ Tài Sản (1-Tap Owner Cancellation):** Trong thời gian đếm ngược 48h, Chủ tài sản có thể hủy bỏ yêu cầu bất kỳ lúc nào chỉ bằng 1 chạm, đưa hệ thống về trạng thái an toàn tuyệt đối và chặn đứng mọi mưu toan kích hoạt bất hợp pháp.
5. **Xác nhận đa bên (Quorum / Multi-Party Verification):** Yêu cầu sự xác nhận độc lập từ các Người Ủy Thác khác trước khi lệnh kích hoạt có hiệu lực.
6. **Mở khóa theo phạm vi ủy thác (Scoped Emergency Unlock):** Sau khi kích hoạt thành công, Người ủy thác chỉ được truy cập vào đúng các thẻ hành động mình được phân quyền phụ trách từ Module 3.
7. **Khôi phục và Thu hồi tình trạng khẩn cấp (Vitality Recovery & Rollback):** Chủ tài sản có thể phục hồi quyền kiểm soát, chứng minh sự hiện diện của mình và tắt chế độ khẩn cấp bất kỳ lúc nào.

---

## 2. Tri Thức Miền & Thuật Ngữ Nghiệp Vụ (Domain Knowledge)

| Thuật ngữ | Tiếng Anh | Định nghĩa nghiệp vụ chuẩn xác |
| :--- | :--- | :--- |
| **Giao Thức Kích Hoạt An Toàn** | Safe Activation Protocol | Quy trình xác minh đa tầng có trễ thời gian (Time-lock) và phân quyền chặt chẽ nhằm chuyển trạng thái kế hoạch từ Bình thường sang Khẩn cấp. |
| **Điểm Danh Định Kỳ (Nhịp Tim)** | Vitality Check-in / Heartbeat | Thao tác 1-chạm của Chủ tài sản nhằm xác nhận bản thân vẫn an toàn và đang quản lý các công việc bình thường. |
| **Khoảng Thời Gian Điểm Danh** | Check-in Interval | Chu kỳ yêu cầu điểm danh định kỳ được cấu hình bởi Chủ tài sản (ví dụ: 15 ngày, 30 ngày, 60 ngày, 90 ngày; mặc định: 30 ngày). |
| **Thời Gian Đệm Bảo Vệ (Khóa Thời Gian)** | Time-Lock Grace Period | Khoảng thời gian trì hoãn bắt buộc (ví dụ: 48 giờ hoặc 72 giờ) từ khi phát sinh yêu cầu kích hoạt đến khi kế hoạch thực sự mở khóa. |
| **Yêu Cầu Kích Hoạt Khẩn Cấp** | Emergency Activation Request | Hồ sơ yêu cầu kích hoạt kế hoạch do hệ thống phát động (khi timeout) hoặc do Người ủy thác cấp 2/3 tạo lập kèm lý do. |
| **Hủy Bỏ Kích Hoạt 1-Chạm** | 1-Tap Cancellation | Quyền tối thượng của Chủ tài sản nhằm hủy ngay lập tức một yêu cầu kích hoạt đang trong thời gian đệm, ngăn chặn báo động giả. |
| **Ngưỡng Xác Nhận Đa Bên** | Quorum / Multi-Party Confirmation | Quy định cần tối thiểu $M$-trên-$N$ Người Ủy Thác độc lập đồng thuận xác nhận rằng Chủ tài sản thực sự gặp biến cố. |
| **Trạng Thái Khẩn Cấp (Kế Hoạch Mở)** | Emergency Active State | Trạng thái của hệ thống sau khi vượt qua thời gian đệm và đủ điều kiện xác thực, cho phép Người Ủy Thác truy xuất thẻ được phân quyền. |
| **Khôi Phục Quyền Kiểm Soát** | Vitality Recovery & Deactivation | Quy trình Chủ tài sản đăng nhập, chứng minh quyền sở hữu, chấm dứt trạng thái khẩn cấp và cắt quyền truy cập khẩn cấp của Người Ủy Thác. |

---

## 3. Ràng Buộc Kỹ Thuật & Kiến Trúc (Constraints)

### 3.1. Ràng buộc Bảo Mật & Hiến Pháp Asseta
1. **Nguyên Tắc Zero-Knowledge:**
   - Server tuyệt đối không lưu trữ khóa giải mã Master Key của người dùng.
   - Khi kế hoạch chuyển sang trạng thái `EmergencyActive`, Server chỉ cấp quyền truy xuất các bản mã `CipherInstructions` hoặc bao thư khóa E2EE (`KeyEnvelope`) cho những Người Ủy Thác đã ghép đôi hợp lệ. Việc giải mã chỉ diễn ra trên thiết bị đầu cuối của Người Ủy Thác.
2. **Cô lập Đa Người Dùng (Multi-Tenant Isolation):**
   - Mọi cấu hình Heartbeat và yêu cầu kích hoạt thuộc quyền sở hữu độc quyền của `OwnerId`.
   - Người ủy thác chỉ được tạo yêu cầu hoặc bỏ phiếu xác nhận cho tài khoản Chủ tài sản mà mình đã ghép đôi thành công (`TrustedPersonStatus.Active`).
3. **Toàn Vẹn Dữ Liệu & Khóa Lạc Quan (`RowVersion`):**
   - Trạng thái yêu cầu kích hoạt (`ActivationRequest`) và cấu hình kích hoạt (`OwnerActivationConfig`) phải được bảo vệ chống tranh chấp đồng thời bằng `RowVersion`.
4. **Nhật Ký Kiểm Toán Bất Biến (Immutable Audit Trail):**
   - 100% các hành vi: Điểm danh, yêu cầu kích hoạt, hủy yêu cầu, xác nhận của người ủy thác, mở khóa khẩn cấp, khôi phục trạng thái... đều phải ghi nhận vào `continuity_audit_logs`.

---

## 4. Giả Định Kỹ Thuật & Các Câu Hỏi Đã Làm Rõ (Assumptions & Resolved Questions)

- **Cấu hình chu kỳ mặc định:**
  - `CheckInIntervalDays`: 30 ngày (cho phép cấu hình 15, 30, 60, 90 ngày).
  - `GracePeriodHours`: 48 giờ (cho phép cấu hình 24h, 48h, 72h, 168h).
  - `MinConfirmationsRequired`: Mặc định 1 người (nếu có 1-2 delegates) hoặc 2 người (nếu có $\ge 3$ delegates).
- **Hành vi khi Người Ủy Thác bấm kích hoạt:**
  - Hệ thống gửi SMS/Email và Push notification cho Chủ tài sản ngay lập tức, chuyển trạng thái yêu cầu sang `PENDING_GRACE_PERIOD`.
  - Bộ đếm thời gian 48 giờ bắt đầu đếm ngược.
- **Hành vi khi Chủ tài sản bấm "Tôi Vẫn Ổn" (Cancel/Check-in):**
  - Trạng thái yêu cầu chuyển thành `CANCELLED_BY_OWNER`.
  - `LastCheckInAtUtc` được cập nhật về thời điểm hiện tại.
  - Người ủy thác gửi yêu cầu nhận được thông báo rằng yêu cầu đã bị hủy vì Chủ tài sản vẫn an toàn.

---

## 5. Những Gì Nằm Ngoài Phạm Vi (Out of Scope)

1. Tự động kết nối với cơ sở dữ liệu khai tử của chính phủ hoặc bệnh viện (strictly out of scope cho MVP).
2. Kiểm tra sinh trắc học liveness nhận diện khuôn mặt qua bên thứ ba (chỉ sử dụng xác thực tài khoản chuẩn JWT/Biometrics của thiết bị).
3. Tự động thực hiện các giao dịch chuyển nhượng tài sản ngân hàng, chứng khoán hoặc lập di chúc pháp lý.
4. Trò chuyện P2P trực tiếp giữa các người ủy thác trong ứng dụng.

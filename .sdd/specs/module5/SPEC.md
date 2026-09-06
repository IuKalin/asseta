# Đặc Tả Kỹ Thuật Chuẩn EARS: Module 5 – Safe Activation & Dead Man's Switch Protocol

**Mã Module:** `module5` (Tương đương `feat-05-safe-activation`)  
**Phiên bản:** v1.0.0 DRAFT / READY FOR APPROVAL  
**Phương pháp áp dụng:** Spec-Driven Development (SDD) & EARS Syntax  
**Cơ chế bảo mật:** Zero-Knowledge, Time-Locked Grace Period, Multi-Party Quorum & 1-Tap Cancellation  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module5/CONTEXT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  

---

## 1. Bối Cảnh & Mục Tiêu Kỹ Thuật (Context & Goals)

Module 5 hoàn thiện chặng cuối cùng của chu trình kế thừa Asseta: Thiết lập cơ chế kiểm tra sức sống định kỳ (Vitality Check-in), phát hiện sự cố khẩn cấp, bảo vệ chủ tài sản bằng cửa sổ đệm an toàn (Time-lock Grace Period), ngăn chặn kích hoạt giả mạo bằng ngưỡng xác nhận đa bên (Quorum), và mở quyền truy xuất theo đúng phạm vi ủy thác (Scoped Emergency Access).

---

## 2. Tác Nhân & Vai Trò (Actors & Roles)

| Tác nhân (Actor) | Vai trò & Quyền hạn trong Module 5 |
| :--- | :--- |
| **Owner (Chủ tài sản)** | Cấu hình chu kỳ điểm danh (`CheckInIntervalDays`), cấu hình thời gian đệm (`GracePeriodHours`), thực hiện điểm danh định kỳ (`VitalityCheckIn`), hủy yêu cầu kích hoạt 1-chạm (`CancelActivationRequest`), khôi phục quyền kiểm soát và tắt trạng thái khẩn cấp (`DeactivateEmergencyPlan`). |
| **Trusted Person (Người ủy thác)** | Người ủy thác cấp 2 hoặc 3 (`Level 2` Scoped Delegate hoặc `Level 3` Primary Delegate) có quyền gửi yêu cầu kích hoạt khẩn cấp (`InitiateActivationRequest`), tham gia biểu quyết xác nhận (`ConfirmActivationRequest`), và truy xuất các thẻ được phân quyền khi trạng thái chuyển sang `ACTIVATED`. Người ủy thác cấp 1 (`Level 1` Notice Only) không có quyền kích hoạt. |
| **System Heartbeat Worker** | Tiến trình nền kiểm tra thời hạn điểm danh, gửi cảnh báo nhắc nhở khi đến hạn, và tự động phát động yêu cầu kích hoạt timeout nếu chủ tài sản vắng mặt quá hạn quy định. |

---

## 3. Yêu Cầu Chức Năng (Functional Requirements - EARS Syntax)

### 3.1. Yêu Cầu Phổ Quát (Ubiquitous Requirements)

- `[REQ-ACT-001]`: Hệ thống SHALL luôn luôn cô lập dữ liệu cấu hình kích hoạt (`OwnerActivationConfig`) và các yêu cầu kích hoạt (`ActivationRequest`) theo mã định danh `OwnerId` của tài khoản đã xác thực qua JWT Claims, nghiêm cấm truy xuất chéo giữa các người dùng.
- `[REQ-ACT-002]`: Hệ thống SHALL luôn luôn tuân thủ nguyên tắc Zero-Knowledge: Server không bao giờ lưu trữ khóa giải mã Master Key; việc giải mã chỉ diễn ra trên client của Người Ủy Thác khi kế hoạch đã chuyển sang trạng thái `ACTIVATED`.
- `[REQ-ACT-003]`: Hệ thống SHALL luôn luôn ghi nhận nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác check-in, sửa cấu hình, phát động yêu cầu, biểu quyết xác nhận, hủy bỏ, kích hoạt thành công và khôi phục trạng thái.
- `[REQ-ACT-004]`: Hệ thống SHALL luôn luôn duy trì tính toàn vẹn dữ liệu và kiểm soát tương tranh lạc quan thông qua thuộc tính `RowVersion` trên cả Entity `OwnerActivationConfig` và `ActivationRequest`.

### 3.2. Yêu Cầu Kích Hoạt Theo Sự Kiện (Event-Driven Requirements)

- `[REQ-ACT-005]`: WHEN Chủ tài sản gửi yêu cầu điểm danh định kỳ (`VitalityCheckInCommand`), THE hệ thống SHALL cập nhật `LastCheckInAtUtc = UtcNow`, tính toán lại `NextCheckInDueUtc = UtcNow + CheckInIntervalDays`, chuyển trạng thái Heartbeat về `ACTIVE`, tăng `RowVersion` và ghi audit log `VITALITY_CHECK_IN_RECORDED`.
- `[REQ-ACT-006]`: WHEN Chủ tài sản cập nhật cấu hình kích hoạt an toàn (`UpdateActivationConfigCommand`), THE hệ thống SHALL kiểm tra hợp lệ các tham số (`CheckInIntervalDays` $\in [15, 90]$, `GracePeriodHours` $\in [24, 168]$, `MinConfirmationsRequired` $\in [1, 5]$), cập nhật vào cơ sở dữ liệu và trả về cấu hình mới.
- `[REQ-ACT-007]`: WHEN một Người Ủy Thác ở trạng thái `Active` (thuộc Trust Level 2 hoặc 3) gửi yêu cầu kích hoạt khẩn cấp (`InitiateActivationRequestCommand`), THE hệ thống SHALL kiểm tra tính hợp lệ, tạo một `ActivationRequest` mới ở trạng thái `PENDING_GRACE_PERIOD`, thiết lập `GracePeriodExpiresAtUtc = UtcNow + GracePeriodHours`, phát thông báo khẩn cấp đến Chủ tài sản qua Email/Push, và tự động ghi nhận phiếu xác nhận đầu tiên từ người yêu cầu.
- `[REQ-ACT-008]`: WHEN Chủ tài sản thực hiện lệnh hủy yêu cầu kích hoạt (`CancelActivationRequestCommand`), THE hệ thống SHALL ngay lập tức chuyển trạng thái `ActivationRequest` thành `CANCELLED_BY_OWNER`, giữ trạng thái hệ thống ở mức an toàn bình thường (`NORMAL`), cập nhật `LastCheckInAtUtc = UtcNow`, và gửi thông báo hủy cho các bên liên quan.
- `[REQ-ACT-009]`: WHEN một Người Ủy Thác khác thực hiện biểu quyết xác nhận (`ConfirmActivationRequestCommand`), THE hệ thống SHALL lưu phiếu xác nhận, kiểm tra nếu số lượng xác nhận đạt ngưỡng `MinConfirmationsRequired` và thời gian đệm đã hết (`GracePeriodExpiresAtUtc <= UtcNow`) mà không bị hủy, THE hệ thống SHALL chuyển trạng thái sang `ACTIVATED`.
- `[REQ-ACT-010]`: WHEN thời gian đệm `GracePeriodHours` kết thúc VÀ số xác nhận đạt ngưỡng `MinConfirmationsRequired` VÀ không có lệnh hủy từ Chủ tài sản, THE hệ thống SHALL chuyển trạng thái yêu cầu sang `ACTIVATED`, chuyển trạng thái kế hoạch sang `EmergencyActive`, và mở quyền truy xuất thẻ hành động Scoped Access cho các Người Ủy Thác.
- `[REQ-ACT-011]`: WHEN Chủ tài sản thực hiện khôi phục quyền kiểm soát và tắt trạng thái khẩn cấp (`DeactivateEmergencyPlanCommand`), THE hệ thống SHALL xác thực quyền Chủ tài sản, chuyển trạng thái kế hoạch trở lại `NORMAL`, thu hồi toàn bộ quyền truy xuất khẩn cấp của Người Ủy Thác, cập nhật `LastCheckInAtUtc = UtcNow`, và ghi audit log `PLAN_EMERGENCY_DEACTIVATED`.

### 3.3. Yêu Cầu Theo Trạng Thái (State-Driven Requirements)

- `[REQ-ACT-012]`: WHILE hệ thống ở trạng thái `ACTIVATED` (Khẩn cấp), THE hệ thống SHALL cho phép Người Ủy Thác đã ghép đôi truy xuất các Thẻ hành động được phân quyền, và hiển thị banner trạng thái khẩn cấp màu đỏ trên cả giao diện Web và Mobile.
- `[REQ-ACT-013]`: WHILE hệ thống ở trạng thái `PENDING_GRACE_PERIOD`, THE hệ thống SHALL hiển thị đồng hồ đếm ngược thời gian đệm (Countdown Timer), nút hủy khẩn cấp 1-chạm ("Tôi Vẫn Ổn") nổi bật cho Chủ tài sản, và tuyệt đối KHÔNG cho phép Người Ủy Thác xem nội dung thẻ nhạy cảm.
- `[REQ-ACT-014]`: WHILE ứng dụng di động hoặc Web ở trạng thái ngoại tuyến (`Offline`), THE hệ thống Client SHALL cho phép xem trạng thái kích hoạt gần nhất và lưu trữ bản ghi điểm danh cục bộ để đồng bộ ngay khi có mạng trở lại.

### 3.4. Yêu Cầu Xử Lý Bất Thường & Ngoại Lệ (Unwanted Behavior Requirements)

- `[REQ-ACT-015]`: IF một Người Ủy Thác cấp 1 (`Level 1 - Notice Only`) hoặc người chưa ghép đôi gửi yêu cầu kích hoạt, THEN hệ thống SHALL từ chối và trả về HTTP 403 Forbidden kèm mã lỗi `INSUFFICIENT_TRUST_LEVEL_FOR_ACTIVATION`.
- `[REQ-ACT-016]`: IF đã có một yêu cầu kích hoạt đang ở trạng thái `PENDING_GRACE_PERIOD` cho cùng một Chủ tài sản, THEN hệ thống SHALL từ chối tạo yêu cầu kích hoạt mới và trả về HTTP 409 Conflict kèm mã lỗi `ACTIVE_REQUEST_ALREADY_EXISTS`.
- `[REQ-ACT-017]`: IF Chủ tài sản cố gắng hủy một yêu cầu kích hoạt đã ở trạng thái `CANCELLED_BY_OWNER`, `REJECTED` hoặc `EXPIRED`, THEN hệ thống SHALL trả về HTTP 400 Bad Request kèm mã lỗi `INVALID_ACTIVATION_REQUEST_STATE`.
- `[REQ-ACT-018]`: IF xảy ra xung đột phiên bản dữ liệu đồng thời (`RowVersion` không khớp), THEN hệ thống SHALL từ chối ghi đè và trả về HTTP 409 Conflict kèm mã lỗi `CONCURRENT_STATE_MUTATION`.

### 3.5. Yêu Cầu Phức Hợp & Đồng Bộ Liên Module (Complex Requirements)

- `[REQ-ACT-019]`: WHEN tiến trình nền (Background Heartbeat Worker) phát hiện `UtcNow > NextCheckInDueUtc`, THE hệ thống SHALL chuyển trạng thái sang `WARNING`. Nếu sau thời gian gia hạn nhắc nhở (Grace Reminder 7 ngày) mà Chủ tài sản vẫn không điểm danh, hệ thống SHALL tự động tạo một `ActivationRequest` có nguồn gốc `SYSTEM_TIMEOUT` và khởi động thời gian đệm `GracePeriodHours`.
- `[REQ-ACT-020]`: THE hệ thống SHALL cung cấp API truy vấn trạng thái kích hoạt tổng thể (`GetActivationStatusQuery`), trả về: Cấu hình hiện tại, Thời điểm điểm danh cuối, Hạn điểm danh tiếp theo, Trạng thái hoạt động (`NORMAL`, `WARNING`, `PENDING_GRACE_PERIOD`, `ACTIVATED`), danh sách xác nhận và thời gian đếm ngược còn lại.

---

## 4. Yêu Cầu Phi Chức Năng Có Đo Lường (Non-Functional Requirements)

| Tiêu chí | Chỉ số đo lường (SLA) | Phương pháp kiểm chứng |
| :--- | :--- | :--- |
| **Độ trễ Check-in API** | Thao tác `VitalityCheckInCommand` hoàn tất trong **< 80ms**. | Integration Test đo thời gian phản hồi với DB in-memory/Postgres. |
| **Độ chính xác Time-Lock** | Thời gian đếm ngược chính xác theo UTC, không bị sai lệch múi giờ người dùng. | Unit Test kiểm tra các mốc DateTimeOffset/DateTime UTC. |
| **Bảo mật Zero-Knowledge** | Server không sở hữu Master Key và không bao giờ lưu trữ mật khẩu/OTP plaintext. | Code Review & Audit Log Verification. |
| **Độ Phủ Kiểm Thử (Coverage)** | Đạt tối thiểu **90% code coverage** trên toàn bộ Command/Query Handlers của Module 5. | `dotnet test --collect:"XPlat Code Coverage"`. |

---

## 5. Mô Hình Dữ Liệu & Hợp Đồng DTO (Data Model)

### 5.1. Entity `OwnerActivationConfig`
```csharp
public class OwnerActivationConfig : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public int CheckInIntervalDays { get; private set; } = 30;
    public int GracePeriodHours { get; private set; } = 48;
    public int MinConfirmationsRequired { get; private set; } = 1;
    public DateTime LastCheckInAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime NextCheckInDueUtc { get; private set; }
    public HeartbeatStatus Status { get; private set; } = HeartbeatStatus.Active;
    public int RowVersion { get; private set; } = 1;
}
```

### 5.2. Entity `ActivationRequest`
```csharp
public class ActivationRequest : BaseEntity
{
    public Guid OwnerId { get; private set; }
    public ActivationTriggerSource TriggerSource { get; private set; } // TrustedPersonRequest or SystemTimeout
    public Guid? InitiatedByTrustedPersonId { get; private set; }
    public string? Reason { get; private set; }
    public ActivationRequestStatus Status { get; private set; } // PendingGracePeriod, CancelledByOwner, Activated, Rejected
    public DateTime GracePeriodExpiresAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public DateTime? ActivatedAtUtc { get; private set; }
    public int RowVersion { get; private set; } = 1;
    public ICollection<ActivationConfirmation> Confirmations { get; private set; } = new List<ActivationConfirmation>();
}
```

### 5.3. Entity `ActivationConfirmation`
```csharp
public class ActivationConfirmation : BaseEntity
{
    public Guid ActivationRequestId { get; private set; }
    public Guid TrustedPersonId { get; private set; }
    public bool IsConfirmed { get; private set; }
    public string? Note { get; private set; }
    public DateTime ConfirmedAtUtc { get; private set; }
}
```

---

## 6. Tiêu Chuẩn Nghiệm Thu (Acceptance Criteria - Given-When-Then)

- **Kịch bản 1: Điểm danh định kỳ thành công**
  - GIVEN: Chủ tài sản có hạn điểm danh tiếp theo là ngày 10/09/2026.
  - WHEN: Chủ tài sản gửi lệnh `VitalityCheckIn`.
  - THEN: `LastCheckInAtUtc` cập nhật thời điểm hiện tại, `NextCheckInDueUtc` cộng thêm 30 ngày, trạng thái là `ACTIVE`.
- **Kịch bản 2: Người ủy thác kích hoạt & Chủ tài sản hủy 1-chạm trong thời gian đệm**
  - GIVEN: Người ủy thác Level 2 gửi yêu cầu kích hoạt vì nghi ngờ sự cố.
  - WHEN: Yêu cầu được tạo với thời gian đệm 48h (`PENDING_GRACE_PERIOD`). Chủ tài sản nhận cảnh báo và bấm nút "Tôi Vẫn Ổn" (Cancel).
  - THEN: Yêu cầu chuyển thành `CANCELLED_BY_OWNER`, kế hoạch duy trì trạng thái `NORMAL`, người ủy thác không xem được thông tin nhạy cảm.
- **Kịch bản 3: Hết thời gian đệm & Đạt ngưỡng xác nhận &rarr; Mở khóa khẩn cấp**
  - GIVEN: Yêu cầu kích hoạt đã có đủ số xác nhận và hết 48h mà không bị hủy.
  - WHEN: Hệ thống kiểm tra điều kiện chuyển trạng thái.
  - THEN: Trạng thái chuyển thành `ACTIVATED`, kế hoạch chuyển sang chế độ `EmergencyActive`, các Người Ủy Thác được xem thẻ trong phạm vi phân quyền.

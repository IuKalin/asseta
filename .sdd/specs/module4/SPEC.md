# Đặc Tả Kỹ Thuật Chuẩn EARS: Module 4 – Continuity Plan (Kế Hoạch Tiếp Quản Tổng Thể Theo Trục Thời Gian)

**Mã Module:** `module4` (Tương đương `feat-04-continuity-plan`)  
**Phiên bản:** v1.0.0 APPROVED  
**Phương pháp áp dụng:** Spec-Driven Development (SDD) & EARS Syntax  
**Cơ chế bảo mật:** Zero-Knowledge, Multi-Tenant Isolation & Time-Locked Execution View  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module4/CONTEXT.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  

---

## 1. Bối Cảnh & Mục Tiêu Kỹ Thuật (Context & Goals)

### 1.1. Bối cảnh
Trong hệ thống Asseta, người dùng đã nhận diện danh mục tài sản/nghĩa vụ (Module 1: Continuity Map), soạn thảo hướng dẫn tiếp quản chi tiết (Module 2: Action Cards), và chỉ định mạng lưới người tin cậy kèm phân quyền (Module 3: Trusted People). Tuy nhiên, khi đối mặt với khủng hoảng khẩn cấp thực tế, một danh sách tĩnh chia theo lĩnh vực (tài chính, pháp lý, bất động sản...) gây ra tình trạng bối rối và tê liệt xử lý. Người tiếp quản cần một lộ trình hành động có cấu trúc theo trục thời gian thực thi: việc gì giải quyết tức thì, việc gì giải quyết trong 72 giờ, việc gì giải quyết trong 7 ngày, và việc gì duy trì trong 30 ngày tiếp theo.

### 1.2. Mục tiêu kỹ thuật của Module 4
- **Tổ chức Kế Hoạch Theo 4 Mốc Thời Gian (4-Stage Execution Timeline)**: Tự động tổng hợp và phân nhóm toàn bộ các `ActionCard` theo 4 giai đoạn khẩn cấp:
  1. `IMMEDIATE` (Giai đoạn 1: Xử lý tức thì trong 24 giờ đầu).
  2. `FIRST_72_HOURS` (Giai đoạn 2: Ổn định và nghĩa vụ cấp thiết trong 24–72 giờ).
  3. `FIRST_7_DAYS` (Giai đoạn 3: Làm việc với ngân hàng, bảo hiểm, đối tác trong 3–7 ngày).
  4. `LONGER_TERM` (Giai đoạn 4: Quản lý và chuyển giao dài hạn sau 7 ngày đến 30 ngày).
- **Phát Hiện & Cảnh Báo Lỗ Hổng Kế Hoạch (Stage Gap Detection)**: Tự động phát hiện các thẻ trong các giai đoạn nhạy cảm (đặc biệt `IMMEDIATE` và `FIRST_72_HOURS`) bị thiếu người phụ trách hoặc thiếu gợi ý vị trí giấy tờ, kích hoạt cờ cảnh báo `HasStageGap = true`.
- **Đánh Giá Chỉ Số Sẵn Sàng Kế Hoạch (Plan Readiness & SPoF Analysis)**: Tính toán tỷ lệ bao phủ người ủy thác (`DelegateCoveragePercentage`), tỷ lệ sẵn sàng hồ sơ (`DocumentReadinessPercentage`), và phát hiện rủi ro điểm nghẽn đơn lẻ (Single Point of Failure - khi một người gán > 70% việc khẩn cấp).
- **Trích Xuất Bản Tóm Lược Khẩn Cấp (Offline Emergency Brief)**: Cung cấp bản tổng hợp an toàn Zero-Knowledge để người dùng có thể xem offline, in ấn hoặc lưu trữ dự phòng mà không làm lộ các ghi chú giải mật.
- **Điều Phối Linh Hoạt (Timeline Drag-and-Drop / Stage Reassignment)**: Cho phép Chủ tài sản thay đổi mốc thời gian của từng thẻ để tối ưu kịch bản tiếp quản.

---

## 2. Tác Nhân & Vai Trò (Actors & Roles)

| Tác nhân (Actor) | Vai trò & Quyền hạn trong Module 4 |
| :--- | :--- |
| **Owner (Chủ tài sản)** | Xem toàn cảnh Kế Hoạch Tiếp Quản Cá Nhân, điều chỉnh giai đoạn khẩn cấp của các thẻ, đánh dấu kiểm tra sẵn sàng (`IsCompleted`), xem báo cáo kiểm toán lỗ hổng (Readiness Audit), xuất bản tóm lược khẩn cấp (Emergency Brief). |
| **Trusted Person (Người ủy thác)** | Ở trạng thái bình thường (Normal State), chỉ được xem timeline giới hạn gồm những thẻ mà mình được phân quyền phụ trách (Role-Scoped Timeline), không thấy các thẻ của người khác và không giải mã được trường bí mật. |
| **System Worker (Tiến trình nền)** | Tự động đồng bộ hóa trạng thái Stage Gaps khi có sự thay đổi người phụ trách ở Module 3 hoặc thay đổi thẻ ở Module 2. |

---

## 3. Yêu Cầu Chức Năng (Functional Requirements - EARS Syntax)

### 3.1. Yêu Cầu Phổ Quát (Ubiquitous Requirements)

- `[REQ-PLAN-001]`: Hệ thống SHALL luôn luôn cô lập dữ liệu Kế Hoạch Tiếp Quản theo mã định danh `OwnerId` của tài khoản đã xác thực qua JWT Claims, nghiêm cấm truy xuất chéo giữa các người dùng khác nhau.
- `[REQ-PLAN-002]`: Hệ thống SHALL luôn luôn tuân thủ nguyên tắc Zero-Knowledge & Zero-Disclosure: Giao diện tổng quan Kế hoạch và Bản tóm lược khẩn cấp tuyệt đối KHÔNG bao giờ giải mã hoặc làm lộ các chuỗi chỉ dẫn bí mật (`CipherInstructions`).
- `[REQ-PLAN-003]`: Hệ thống SHALL luôn luôn ghi nhận nhật ký kiểm toán bất biến (`continuity_audit_logs`) cho mọi thao tác điều chỉnh giai đoạn khẩn cấp của thẻ hoặc đánh dấu hoàn tất kiểm tra kế hoạch.
- `[REQ-PLAN-004]`: Hệ thống SHALL luôn luôn duy trì tính toàn vẹn dữ liệu: Việc thay đổi giai đoạn khẩn cấp trên Kế hoạch thực chất là cập nhật thuộc tính `Urgency` trên Entity `ActionCard`, bảo toàn mọi danh sách các bước (`Steps`) và danh bạ liên hệ (`Contacts`).

### 3.2. Yêu Cầu Kích Hoạt Theo Sự Kiện (Event-Driven Requirements)

- `[REQ-PLAN-005]`: WHEN Owner gửi yêu cầu truy vấn Kế Hoạch Tiếp Quản (`GetContinuityPlanQuery`), THE hệ thống SHALL tổng hợp toàn bộ các `ActionCard` chưa bị xóa mềm của Owner, phân nhóm thành 4 giai đoạn (`IMMEDIATE`, `FIRST_72_HOURS`, `FIRST_7_DAYS`, `LONGER_TERM`), sắp xếp thẻ trong từng giai đoạn theo thứ tự ưu tiên giảm dần (`CRITICAL` &rarr; `HIGH` &rarr; `MEDIUM` &rarr; `LOW`), làm giàu thông tin danh mục và người phụ trách, tính toán chỉ số bao phủ và trả về cấu trúc phân cấp hoàn chỉnh.
- `[REQ-PLAN-006]`: WHEN Owner thay đổi giai đoạn khẩn cấp của một Thẻ hành động (`UpdateActionCardStageCommand`), THE hệ thống SHALL kiểm tra quyền sở hữu, cập nhật giá trị `UrgencyStage`, tăng `RowVersion`, ghi nhận Audit Log `ACTION_CARD_STAGE_UPDATED` và trả về thông tin thẻ đã cập nhật.
- `[REQ-PLAN-007]`: WHEN Owner hoặc Người Ủy Thác yêu cầu xuất Bản Tóm Lược Khẩn Cấp Ngoại Tuyến (`GetPlanEmergencyBriefQuery`), THE hệ thống SHALL trích xuất bản tóm tắt an toàn Zero-Knowledge gồm: Tên giai đoạn, Tiêu đề thẻ, Mức độ ưu tiên, Họ tên và SĐT người phụ trách, Gợi ý vị trí giấy tờ vật lý (`DocumentLocationHint`), và Danh bạ liên hệ khẩn cấp (`Contacts`).
- `[REQ-PLAN-008]`: WHEN Owner yêu cầu kiểm toán mức độ sẵn sàng của Kế hoạch (`GetPlanReadinessAuditQuery`), THE hệ thống SHALL phân tích toàn bộ các thẻ theo 4 giai đoạn, xác định danh sách các thẻ có lỗ hổng (`HasStageGap`), tính toán chỉ số `DelegateCoveragePercentage`, `DocumentReadinessPercentage`, và kiểm tra rủi ro Điểm Nghẽn Đơn Lẻ (`SinglePointOfFailureRisk`) nếu có một người ủy thác bị gán > 70% số thẻ của giai đoạn Immediate hoặc 72 Hours.
- `[REQ-PLAN-009]`: WHEN một Thẻ hành động thuộc giai đoạn `IMMEDIATE` hoặc `FIRST_72_HOURS` bị thiếu Người phụ trách (`AssignedTrustedPersonId == null`) HOẶC thiếu vị trí hồ sơ (`DocumentLocationHint == null`), THE hệ thống SHALL tự động đánh dấu thuộc tính `HasStageGap = true` trên DTO của thẻ đó.
- `[REQ-PLAN-010]`: WHEN Owner đánh dấu một Thẻ hành động là đã sẵn sàng/hoàn tất chuẩn bị (`ToggleActionCardCompletionCommand`), THE hệ thống SHALL cập nhật cờ `IsCompleted`, tăng `RowVersion`, và cập nhật tỷ lệ hoàn thành trên giao diện kế hoạch.

### 3.3. Yêu Cầu Theo Trạng Thái (State-Driven Requirements)

- `[REQ-PLAN-011]`: WHILE hệ thống ở trạng thái hoạt động bình thường (chưa kích hoạt khẩn cấp qua Module 5), WHEN Người Ủy Thác đã ghép đôi truy vấn Kế hoạch (`GetMyDelegatedPlanQuery`), THE hệ thống SHALL áp dụng bộ lọc Scoped Access: chỉ trả về các thẻ mà Người Ủy Thác đó được phân quyền đảm nhận, nhóm theo 4 giai đoạn, và tuyệt đối không hiển thị các thẻ ngoài phạm vi phân quyền.
- `[REQ-PLAN-012]`: WHILE ứng dụng di động (Flutter) hoặc Web ở trạng thái ngoại tuyến (`Offline`), THE hệ thống phía Client SHALL cho phép hiển thị Kế Hoạch Tiếp Quản đã lưu trong bộ nhớ đệm cục bộ bảo mật để người dùng xem lại hướng dẫn khi mất mạng.

### 3.4. Yêu Cầu Xử Lý Bất Thường & Ngoại Lệ (Unwanted Behavior Requirements)

- `[REQ-PLAN-013]`: IF giá trị giai đoạn khẩn cấp truyền vào nằm ngoài khoảng hợp lệ (không thuộc 1, 2, 3, 4), THEN hệ thống SHALL từ chối xử lý và trả về HTTP 400 Bad Request kèm mã lỗi `INVALID_URGENCY_STAGE`.
- `[REQ-PLAN-014]`: IF người dùng gửi yêu cầu cập nhật giai đoạn cho một Thẻ hành động không tồn tại hoặc thuộc quyền sở hữu của người khác, THEN hệ thống SHALL trả về HTTP 404 Not Found kèm mã lỗi `ACTION_CARD_NOT_FOUND`.
- `[REQ-PLAN-015]`: IF xảy ra xung đột phiên bản dữ liệu đồng thời (`RowVersion` không khớp), THEN hệ thống SHALL từ chối ghi đè và trả về HTTP 409 Conflict kèm mã lỗi `CONCURRENT_STATE_MUTATION`.
- `[REQ-PLAN-016]`: IF client gửi yêu cầu kèm `Idempotency-Key` trùng lặp trong vòng 24 giờ, THEN hệ thống SHALL trả về kết quả đã cache mà không thực hiện cập nhật trùng lặp.

### 3.5. Yêu Cầu Phức Hợp & Đồng Bộ Liên Module (Complex Requirements)

- `[REQ-PLAN-017]`: WHEN một Người Ủy Thác bị thu hồi quyền (`Revoke`) hoặc xóa mềm trong Module 3, THE phép chiếu dữ liệu Kế Hoạch Tiếp Quản SHALL ngay lập tức phản ánh thuộc tính `AssignedTrustedPersonId = null` trên các thẻ tương ứng, kích hoạt cờ `HasStageGap = true` và hạ chỉ số `DelegateCoveragePercentage`.
- `[REQ-PLAN-018]`: WHEN một Thẻ hành động mới được tạo lập hoặc xóa bỏ ở Module 2, THE Kế Hoạch Tiếp Quản SHALL tự động xuất hiện hoặc biến mất khỏi mốc thời gian tương ứng mà không yêu cầu người dùng phải tái tạo kế hoạch thủ công.
- `[REQ-PLAN-019]`: THE hệ thống SHALL tính toán tỷ lệ bao phủ người ủy thác `DelegateCoveragePercentage = (Số thẻ có AssignedTrustedPersonId != null / Tổng số thẻ) * 100` và tỷ lệ sẵn sàng giấy tờ `DocumentReadinessPercentage = (Số thẻ có DocumentLocationHint hoặc DigitalStorageLink / Tổng số thẻ) * 100`.
- `[REQ-PLAN-020]`: THE hệ thống SHALL tính toán Điểm Sẵn Sàng Kế Hoạch Tổng Thể (`PlanReadinessScore` thang điểm 0–100) dựa trên trọng số phân bổ theo mức độ khẩn cấp: Giai đoạn Immediate chiếm trọng số 40%, Giai đoạn First 72 Hours chiếm trọng số 30%, Giai đoạn First 7 Days chiếm 20%, và Giai đoạn Longer-Term chiếm 10%.

---

## 4. Yêu Cầu Phi Chức Năng Có Đo Lường (Non-Functional Requirements)

| Tiêu chí | Chỉ số đo lường (SLA) | Phương pháp kiểm chứng |
| :--- | :--- | :--- |
| **Thời gian phản hồi API (Latency)** | Truy vấn `GetContinuityPlanQuery` & `GetPlanEmergencyBriefQuery` hoàn tất trong **< 150ms** với tài khoản có 100 Action Cards. | Integration Test đo benchmark qua EF Core AsNoTracking projection. |
| **Hiệu năng Render UI** | Thời gian render Timeline View trên Web và Mobile đạt **60 FPS**, không bị giật lag khi chuyển đổi giữa 4 giai đoạn. | Chrome DevTools Performance Profiling & Flutter DevTools Frame Rendering. |
| **Bảo mật Zero-Knowledge** | 100% các trường ciphertext không bao giờ xuất hiện ở dạng plaintext trong API Response hoặc log hệ thống. | Unit Test & Security Inspection Test quét toàn bộ DTO của Continuity Plan. |
| **Độ phủ Kiểm Thử (Coverage)** | Đạt tối thiểu **90% code coverage** trên toàn bộ Command/Query Handlers, Validators và DTO Mappers của Module 4. | `dotnet test --collect:"XPlat Code Coverage"`. |

---

## 5. Mô Hình Dữ Liệu & Hợp Đồng DTO (Data Model & DTO Contracts)

### 5.1. DTO Kế Hoạch Tiếp Quản (`ContinuityPlanDto`)
```csharp
public class ContinuityPlanDto
{
    public Guid OwnerId { get; set; }
    public int TotalCardsCount { get; set; }
    public int CompletedCardsCount { get; set; }
    public int GapsCount { get; set; }
    public double DelegateCoveragePercentage { get; set; }
    public double DocumentReadinessPercentage { get; set; }
    public int OverallPlanReadinessScore { get; set; }
    public bool HasSinglePointOfFailureRisk { get; set; }
    public string? SinglePointOfFailureWarning { get; set; }
    public List<ContinuityPlanStageDto> Stages { get; set; } = new();
}

public class ContinuityPlanStageDto
{
    public UrgencyStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string StageDescription { get; set; } = string.Empty;
    public int TotalCardsCount { get; set; }
    public int CompletedCardsCount { get; set; }
    public int GapCardsCount { get; set; }
    public List<ContinuityPlanCardItemDto> Cards { get; set; } = new();
}

public class ContinuityPlanCardItemDto
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public UrgencyStage Urgency { get; set; }
    public PriorityLevel Priority { get; set; }
    public Guid? AssignedTrustedPersonId { get; set; }
    public string? AssignedTrustedPersonName { get; set; }
    public string? AssignedTrustedPersonPhone { get; set; }
    public string? DocumentLocationHint { get; set; }
    public bool HasDocumentLocation { get; set; }
    public bool HasStageGap { get; set; }
    public bool IsCompleted { get; set; }
    public int StepsCount { get; set; }
    public int ContactsCount { get; set; }
    public int RowVersion { get; set; }
}
```

### 5.2. DTO Bản Tóm Lược Khẩn Cấp Ngoại Tuyến (`OfflineEmergencyBriefDto`)
```csharp
public class OfflineEmergencyBriefDto
{
    public Guid OwnerId { get; set; }
    public DateTime GeneratedAtUtc { get; set; }
    public List<EmergencyBriefStageDto> Stages { get; set; } = new();
    public List<EmergencyContactSummaryDto> PrimaryContacts { get; set; } = new();
}

public class EmergencyBriefStageDto
{
    public UrgencyStage Stage { get; set; }
    public string StageName { get; set; } = string.Empty;
    public List<EmergencyBriefCardDto> ActionItems { get; set; } = new();
}

public class EmergencyBriefCardDto
{
    public string Title { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public string? DelegateName { get; set; }
    public string? DelegatePhone { get; set; }
    public string? DocumentLocationHint { get; set; }
    public List<string> KeySteps { get; set; } = new();
    public List<string> KeyContacts { get; set; } = new();
}
```

---

## 6. Xử Lý Lỗi & Mã Lỗi Chuẩn Hóa (Error Handling)

| HTTP Status | Error Code | Điều kiện phát sinh |
| :--- | :--- | :--- |
| `400 Bad Request` | `INVALID_URGENCY_STAGE` | Giá trị mốc thời gian không nằm trong khoảng enum `1..4`. |
| `404 Not Found` | `ACTION_CARD_NOT_FOUND` | Thẻ hành động không tồn tại hoặc đã bị xóa mềm. |
| `403 Forbidden` | `CROSS_TENANT_ACCESS_FORBIDDEN` | Cố ý truy cập hoặc chuyển giai đoạn thẻ của tài khoản khác. |
| `409 Conflict` | `CONCURRENT_STATE_MUTATION` | Xung đột `RowVersion` khi cập nhật đồng thời. |

---

## 7. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Given-When-Then)

### Kịch bản 1: Tổng hợp kế hoạch theo 4 mốc thời gian và tính toán điểm sẵn sàng
- **GIVEN**: Chủ tài sản có 6 Action Cards phân bổ ở các mốc thời gian khác nhau (2 thẻ Immediate, 2 thẻ 72h, 1 thẻ 7d, 1 thẻ Longer-Term).
- **WHEN**: Gửi yêu cầu `GET /api/v1/continuity-plan`.
- **THEN**: API trả về HTTP 200 kèm cấu trúc 4 Stages. Thẻ trong từng Stage được sắp xếp theo Priority giảm dần, tính toán chính xác tổng số thẻ, số thẻ có người phụ trách và điểm sẵn sàng kế hoạch `OverallPlanReadinessScore`.

### Kịch bản 2: Phát hiện lỗ hổng tiếp quản ở giai đoạn khẩn cấp
- **GIVEN**: Một Action Card thuộc giai đoạn `IMMEDIATE` nhưng chưa gán `AssignedTrustedPersonId`.
- **WHEN**: Truy vấn `GetContinuityPlanQuery`.
- **THEN**: DTO của thẻ này có thuộc tính `HasStageGap = true` và trường `GapCardsCount` của Stage Immediate tăng thêm 1.

### Kịch bản 3: Kéo-thả chuyển đổi giai đoạn khẩn cấp thành công
- **GIVEN**: Một Action Card đang ở giai đoạn `FIRST_72_HOURS` với `RowVersion = 1`.
- **WHEN**: Gửi lệnh `PATCH /api/v1/continuity-plan/cards/{id}/stage` với `NewStage = IMMEDIATE` và `RowVersion = 1`.
- **THEN**: Hệ thống cập nhật thẻ sang `IMMEDIATE`, tăng `RowVersion = 2`, lưu audit log `ACTION_CARD_STAGE_UPDATED` và trả về HTTP 200.

### Kịch bản 4: Người ủy thác chỉ xem được các thẻ được phân quyền (Zero-Disclosure)
- **GIVEN**: Người ủy thác B chỉ được phân quyền quản lý danh mục "Tài chính". Chủ sở hữu có các thẻ thuộc "Tài chính" và "Bất động sản".
- **WHEN**: Người ủy thác B gửi yêu cầu `GET /api/v1/continuity-plan/delegated`.
- **THEN**: Hệ thống chỉ trả về các thẻ thuộc danh mục "Tài chính" được phân bổ theo 4 mốc thời gian, tuyệt đối không xuất hiện các thẻ "Bất động sản".

---

## 8. Ngoài Phạm Vi (Out of Scope)

1. KHÔNG gửi thông báo SMS/Zalo/Push Notification tự động theo lịch (Thuộc Module 5: Safe Activation).
2. KHÔNG tích hợp xuất lịch ra file .ics hoặc đồng bộ Google/Apple Calendar.
3. KHÔNG triển khai tính toán chi phí dòng tiền hay nghĩa vụ thuế theo thời gian thực.
4. KHÔNG cho phép người ủy thác tự ý hoán đổi thứ tự ưu tiên hoặc chuyển mốc thời gian của thẻ.

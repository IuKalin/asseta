# Kế Hoạch Quy Hoạch Kiến Trúc: Module 4 – Continuity Plan (Kế Hoạch Tiếp Quản Tổng Thể Theo Trục Thời Gian)

**Mã Module:** `module4` (Tương đương `feat-04-continuity-plan`)  
**Pha phát triển:** Pha 2 – Architecture & Planning  
**Vai trò đảm trách:** Senior System Architect & Technical Lead  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module4/CONTEXT.md), [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module4/SPEC.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  
**Trạng thái:** DRAFT / PENDING HUMAN GATEKEEPER 1 APPROVAL  

---

## 1. Tiếp Cận Kiến Trúc Tổng Thể (Architectural Approach)

Module 4 là tầng tổng hợp và điều phối nghiệp vụ (Orchestration & Timeline Projection Layer), kết nối trực tiếp 3 module nền tảng trước đó:
- **Module 1 (Continuity Map)**: Cung cấp danh mục (`ContinuityCategory`) và cờ cảnh báo lỗ hổng liên đới.
- **Module 2 (Action Cards)**: Cung cấp dữ liệu gốc của các thẻ hành động, các bước thực hiện (`Steps`), danh bạ liên hệ (`Contacts`), và mốc thời gian khẩn cấp (`UrgencyStage`).
- **Module 3 (Trusted People)**: Cung cấp danh tính người phụ trách (`TrustedPerson`), cấp bậc tin cậy và ma trận phân quyền tối thiểu (`ScopedAccessMatrix`).

### 1.1. Backend (.NET 8 Clean Architecture + CQRS)
Hệ thống tuân thủ mô hình Clean Architecture đa tầng:
```
backend/src/
├── Asseta.Domain/
│   ├── Enums/UrgencyStage.cs                       (Đã có từ Module 2: IMMEDIATE, FIRST_72_HOURS, FIRST_7_DAYS, LONGER_TERM)
│   ├── Services/PlanReadinessCalculator.cs         [NEW] Tính điểm sẵn sàng có trọng số & phát hiện rủi ro SPoF
├── Asseta.Application/
│   ├── Features/ContinuityPlan/
│   │   ├── DTOs/                                   [NEW] ContinuityPlanDto, StageDto, PlanCardDto, EmergencyBriefDto, PlanAuditDto
│   │   ├── Queries/GetContinuityPlan/              [NEW] Phép chiếu tổng hợp 4 mốc thời gian, tính toán KPI & Gaps
│   │   ├── Queries/GetPlanEmergencyBrief/          [NEW] Trích xuất bản tóm tắt khẩn cấp an toàn Zero-Knowledge
│   │   ├── Queries/GetPlanReadinessAudit/          [NEW] Kiểm toán toàn diện lỗ hổng & SPoF
│   │   ├── Queries/GetMyDelegatedPlan/             [NEW] Truy vấn timeline phân quyền cho Người Ủy Thác (Zero-Disclosure)
│   │   ├── Commands/UpdateActionCardStage/         [NEW] Điều chỉnh mốc thời gian khẩn cấp (Timeline drag & drop)
│   │   └── Commands/ToggleActionCardCompletion/    [NEW] Đánh dấu hoàn tất kiểm tra sẵn sàng
├── Asseta.Infrastructure/
│   └── Persistence/Configurations/                 [MODIFY] Tối ưu composite index cho truy vấn Timeline
└── Asseta.Api/
    └── Controllers/ContinuityPlanController.cs     [NEW] RESTful Endpoints chuẩn RFC 7807
```

### 1.2. Frontend Web (React 18 + Vite + TypeScript)
```
frontend-web/src/
├── types/continuityPlan.ts                         [NEW] Type definitions cho Plan, Stages, Emergency Brief & Audit
├── services/continuityPlanService.ts               [NEW] Axios client service tích hợp Idempotency-Key
├── features/ContinuityPlan/
│   ├── hooks/useContinuityPlan.ts                  [NEW] React Query hooks (caching, optimistic updates)
│   ├── ContinuityPlanView.tsx                      [MODIFY] Nâng cấp từ placeholder sang giao diện Timeline + Kanban 4 mốc
│   ├── components/
│   │   ├── PlanMetricsHeader.tsx                   [NEW] KPI Cards: Điểm sẵn sàng, Tỷ lệ gán người, Tỷ lệ hồ sơ, Cảnh báo Gap
│   │   ├── ContinuityTimelineStage.tsx             [NEW] Cột hiển thị mốc thời gian kèm badge cảnh báo gap
│   │   ├── ContinuityPlanCardItem.tsx              [NEW] Card item tương tác: gán người, trạng thái, nút chuyển stage
│   │   ├── EmergencyBriefModal.tsx                 [NEW] Modal xem và in ấn bản tóm lược khẩn cấp
│   │   └── PlanReadinessAuditModal.tsx             [NEW] Modal chẩn đoán lỗ hổng và rủi ro điểm nghẽn SPoF
```

### 1.3. Mobile App (Flutter 3.x Clean Architecture + BLoC)
```
mobile-app/lib/features/continuity_plan/
├── domain/
│   ├── entities/continuity_plan_entity.dart        [NEW] Domain entity cho Plan và Emergency Brief
│   └── repositories/continuity_plan_repository.dart [NEW] Abstract repository contract
├── data/
│   ├── models/continuity_plan_model.dart           [NEW] Json serialization model
│   ├── datasources/plan_remote_datasource.dart     [NEW] Dio API client
│   └── repositories/plan_repository_impl.dart      [NEW] Repository implementation kèm offline caching
└── presentation/
    ├── bloc/
    │   ├── continuity_plan_bloc.dart               [NEW] BLoC xử lý tải plan, chuyển stage, xuất emergency brief
    │   ├── continuity_plan_event.dart              [NEW] LoadPlanEvent, ChangeCardStageEvent, ToggleCompleteEvent
    │   └── continuity_plan_state.dart              [NEW] PlanLoading, PlanLoaded, PlanError, StageUpdatedSuccess
    ├── pages/
    │   ├── continuity_plan_page.dart               [NEW] Màn hình Timeline phân đoạn (Segmented control) 4 mốc thời gian
    │   └── emergency_brief_page.dart               [NEW] Màn hình tóm lược khẩn cấp ngoại tuyến (Offline Emergency Brief)
    └── widgets/
        ├── stage_timeline_tile.dart                [NEW] Item timeline với màu sắc nhận diện từng mốc
        └── gap_alert_banner.dart                   [NEW] Banner cảnh báo lỗ hổng giai đoạn Immediate
```

---

## 2. Thiết Kế Cơ Sở Dữ Liệu & Chỉ Mục PostgreSQL (Database Design & Indexes)

Do `ActionCard` đã lưu trữ `UrgencyStage`, `OwnerId`, `AssignedTrustedPersonId`, `IsCompleted` và `RowVersion` từ Module 2, Module 4 tập trung tối ưu hóa cấu trúc truy vấn và chỉ mục:

### 2.1. Composite Index Tối Ưu Cho Timeline Projection
Thêm chỉ mục tổng hợp trên bảng `action_cards` để bảo đảm truy vấn tổng hợp kế hoạch theo thời gian thực luôn dưới 10ms:
```sql
-- Composite index phục vụ lọc theo Owner và gom nhóm theo mốc thời gian khẩn cấp
CREATE INDEX IF NOT EXISTS ix_action_cards_owner_urgency_deleted
ON action_cards (owner_id, urgency)
INCLUDE (category_id, assigned_trusted_person_id, is_completed, row_version)
WHERE is_deleted = FALSE;
```

---

## 3. Hợp Đồng Giao Tiếp API (API Contracts)

Toàn bộ API tuân thủ tiêu chuẩn bọc phong bì kết quả (Envelope Pattern) và RFC 7807 Problem Details khi có lỗi:

### 3.1. `GET /api/v1/continuity-plan`
- **Mục đích**: Truy vấn toàn cảnh Kế Hoạch Tiếp Quản Cá Nhân của Owner, phân nhóm theo 4 giai đoạn thời gian.
- **Header bắt buộc**: `Authorization: Bearer <JWT_TOKEN>`
- **Response 200 OK**:
```json
{
  "success": true,
  "data": {
    "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "totalCardsCount": 8,
    "completedCardsCount": 3,
    "gapsCount": 1,
    "delegateCoveragePercentage": 87.5,
    "documentReadinessPercentage": 75.0,
    "overallPlanReadinessScore": 82,
    "hasSinglePointOfFailureRisk": false,
    "singlePointOfFailureWarning": null,
    "stages": [
      {
        "stage": 1,
        "stageName": "Immediate Actions (NOW)",
        "stageDescription": "Những việc phải xử lý ngay trong 24 giờ đầu khi xảy ra biến cố",
        "totalCardsCount": 3,
        "completedCardsCount": 2,
        "gapCardsCount": 1,
        "cards": [
          {
            "id": "c3d9ef4a-8901-4bcd-8ef0-123456789abc",
            "categoryId": "11111111-1111-1111-1111-111111111111",
            "categoryName": "Tài chính",
            "categoryIcon": "wallet",
            "title": "Khoản vay thế chấp ngân hàng Techcombank",
            "summary": "Khoản vay cần đóng lãi ngày 10 hàng tháng để tránh nợ xấu",
            "urgency": 1,
            "priority": 1,
            "assignedTrustedPersonId": "44444444-4444-4444-4444-444444444444",
            "assignedTrustedPersonName": "Nguyễn Thị Mai (Vợ)",
            "assignedTrustedPersonPhone": "0912345678",
            "documentLocationHint": "Tủ tài liệu phòng làm việc, ngăn thứ hai",
            "hasDocumentLocation": true,
            "hasStageGap": false,
            "isCompleted": true,
            "stepsCount": 4,
            "contactsCount": 2,
            "rowVersion": 1
          },
          {
            "id": "e4f0ab5b-9012-4cde-9f01-234567890bcd",
            "categoryId": "22222222-2222-2222-2222-222222222222",
            "categoryName": "Doanh nghiệp",
            "categoryIcon": "briefcase",
            "title": "Bàn giao quyền điều hành khẩn cấp công ty",
            "summary": "Ủy quyền cộng sự ký séc thanh toán lương nhân viên",
            "urgency": 1,
            "priority": 1,
            "assignedTrustedPersonId": null,
            "assignedTrustedPersonName": null,
            "assignedTrustedPersonPhone": null,
            "documentLocationHint": null,
            "hasDocumentLocation": false,
            "hasStageGap": true,
            "isCompleted": false,
            "stepsCount": 2,
            "contactsCount": 1,
            "rowVersion": 1
          }
        ]
      },
      {
        "stage": 2,
        "stageName": "First 72 Hours",
        "stageDescription": "Ổn định hoạt động và xử lý nghĩa vụ cấp thiết (24–72 giờ)",
        "totalCardsCount": 2,
        "completedCardsCount": 1,
        "gapCardsCount": 0,
        "cards": []
      },
      {
        "stage": 3,
        "stageName": "First 7 Days",
        "stageDescription": "Làm việc với các tổ chức tài chính, đối tác và thủ tục hành chính (3–7 ngày)",
        "totalCardsCount": 2,
        "completedCardsCount": 0,
        "gapCardsCount": 0,
        "cards": []
      },
      {
        "stage": 4,
        "stageName": "Longer-Term Continuity",
        "stageDescription": "Quản lý và tiếp quản dài hạn sau 7 ngày đến 30 ngày",
        "totalCardsCount": 1,
        "completedCardsCount": 0,
        "gapCardsCount": 0,
        "cards": []
      }
    ]
  }
}
```

### 3.2. `PATCH /api/v1/continuity-plan/cards/{id}/stage`
- **Mục đích**: Thay đổi mốc thời gian khẩn cấp của thẻ hành động (Kéo-thả chuyển stage).
- **Request Body**:
```json
{
  "newStage": 2,
  "rowVersion": 1
}
```
- **Response 200 OK**: Trả về thông tin thẻ sau khi cập nhật stage và `rowVersion = 2`.

### 3.3. `PATCH /api/v1/continuity-plan/cards/{id}/toggle-completion`
- **Mục đích**: Đánh dấu đã kiểm tra sẵn sàng thẻ hành động.
- **Request Body**: `{ "rowVersion": 1 }`
- **Response 200 OK**: Trả về trạng thái `isCompleted` đã đảo và `rowVersion = 2`.

### 3.4. `GET /api/v1/continuity-plan/emergency-brief`
- **Mục đích**: Trích xuất bản tóm lược khẩn cấp Zero-Knowledge (không có cipher instructions).
- **Response 200 OK**: Bản tóm tắt phân cấp theo stage, danh sách việc cần làm, vị trí tài liệu, tên và SĐT người phụ trách.

### 3.5. `GET /api/v1/continuity-plan/audit`
- **Mục đích**: Kiểm toán toàn diện mức độ hoàn thiện của kế hoạch, phát hiện thẻ hổng và rủi ro điểm nghẽn SPoF.
- **Response 200 OK**: Báo cáo chẩn đoán chi tiết.

### 3.6. `GET /api/v1/continuity-plan/delegated`
- **Mục đích**: Dành cho Người Ủy Thác truy vấn kế hoạch thuộc phạm vi phân quyền của mình.
- **Response 200 OK**: Chỉ bao gồm các thẻ được phân quyền theo mốc thời gian.

---

## 4. Luồng Mã Hóa & Nguyên Tắc Zero-Knowledge (Cryptographic Flow)

1. **Nguyên Tắc Bất Biến Về Bí Mật**:
   - Trường chỉ dẫn bí mật (`CipherInstructions`) được lưu dưới dạng bản mã AES-256-GCM từ Module 2.
   - Khi API Module 4 tổng hợp Kế Hoạch Tiếp Quản (`GetContinuityPlanQuery`), trường `CipherInstructions` **HOÀN TOÀN BỊ LOẠI BỎ (OMITTED)** khỏi DTO để tối ưu băng thông mạng và ngăn ngừa rủi ro rò rỉ bộ nhớ.
2. **Bản Tóm Lược Khẩn Cấp (Offline Emergency Brief)**:
   - Được thiết kế phục vụ tình huống khẩn cấp ngoại tuyến trước khi giải mã.
   - Chỉ chứa: Tiêu đề công việc, Mức độ ưu tiên, Họ tên và SĐT người phụ trách, Danh bạ đối tác liên hệ, và Gợi ý vị trí vật lý chung (ví dụ: "Tủ tài liệu phòng làm việc"). Không chứa số tài khoản chi tiết, không chứa mật khẩu hay private keys.

---

## 5. Phân Tích Rủi Ro Kỹ Thuật & Giải Pháp Xử Lý (Risks & Mitigations)

| Rủi ro kỹ thuật | Mức độ | Hậu quả tiềm ẩn | Giải pháp kiến trúc bắt buộc |
| :--- | :--- | :--- | :--- |
| **R1: Suy giảm hiệu năng khi tổng hợp Kế hoạch lớn** | Trung bình | Người dùng có nhiều thẻ (> 50 thẻ), mỗi thẻ có nhiều steps và contacts dẫn đến Cartesian Product nếu truy vấn thiếu tối ưu. | Sử dụng phép chiếu `.Select()` chuyên biệt của EF Core với `.AsNoTracking()`, gom nhóm trên bộ nhớ và thiết lập Composite Index `(owner_id, urgency)`. |
| **R2: Xung đột ghi đè đồng thời khi kéo-thả nhiều thẻ** | Cao | Người dùng kéo thả nhiều thẻ trên Web hoặc Mobile trong thời gian ngắn dẫn đến mất dữ liệu hoặc sai lệch trạng thái. | Kiểm soát phiên bản lạc quan qua `RowVersion` trên từng thẻ; hỗ trợ `Idempotency-Key` trên API để chống trùng lặp. |
| **R3: Rò rỉ thông tin cho Người Ủy Thác trước thời hạn** | Cực kỳ nghiêm trọng | Người ủy thác truy cập xem được toàn bộ thẻ của chủ tài sản trong trạng thái bình thường. | Endpoint `/continuity-plan/delegated` bắt buộc lọc qua bảng `trusted_person_permissions` từ Module 3; nếu là `Level 1 (Notice Only)` thì trả về rỗng. |
| **R4: Điểm nghẽn rủi ro tập trung vào một người (SPoF)** | Cao | Toàn bộ các việc tối khẩn cấp trong 24h dồn hết vào một người (ví dụ chỉ giao cho Vợ hoặc chỉ giao cho Kế toán). | Triển khai thuật toán SPoF Detection trong `PlanReadinessCalculator`, cảnh báo nổi bật nếu 1 người nhận > 70% việc giai đoạn 1. |

---

## 6. Câu Hỏi Xác Nhận Với Human Gatekeeper 1 (Questions for Gatekeeper 1)

1. **Công thức tính Điểm Sẵn Sàng Kế Hoạch (Plan Readiness Score)**:
   - Kiến trúc đề xuất phân bổ trọng số: Immediate Actions (40%), First 72 Hours (30%), First 7 Days (20%), Longer-Term (10%). Điểm của từng stage phụ thuộc vào: Tỷ lệ thẻ có người phụ trách (50%) + Tỷ lệ thẻ có vị trí tài liệu (50%). Quý người dùng có đồng thuận với công thức này không?
2. **Tiêu chí cảnh báo Điểm Nghẽn Đơn Lẻ (Single Point of Failure - SPoF)**:
   - Ngưỡng kích hoạt cảnh báo: Khi một Người Ủy Thác được gán trên **70%** tổng số thẻ trong 2 giai đoạn đầu (`IMMEDIATE` hoặc `FIRST_72_HOURS`). Ngưỡng này có phù hợp với thực tế không?
3. **Định dạng Bản Tóm Lược Khẩn Cấp (Emergency Brief)**:
   - Bản tóm lược được thiết kế sẵn chế độ In ấn (Print Mode / PDF Friendly) và lưu Offline trên Web/Mobile để có thể in ra cất vào két sắt gia đình cùng hồ sơ pháp lý. Anh/Chị có yêu cầu bổ sung thông tin gì đặc thù không?

---

> [!IMPORTANT]
> **🛑 STOPPING AT GATEKEEPER 1 CHECKPOINT**:  
> Theo Hiến pháp Asseta và quy trình SDD, Agent **DỪNG LẠI** tại đây và chờ lệnh phê duyệt từ Human Product Lead trước khi tiến hành Phân rã công việc (Pha 3: `TASKS.md`) và Viết mã nguồn (Pha 4: Implementation).

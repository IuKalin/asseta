# Danh Sách Công Việc Phân Rã Nguyên Tử (Task Decomposition): Module 4 – Continuity Plan

**Mã Module:** `module4` (Tương đương `feat-04-continuity-plan`)  
**Pha phát triển:** Pha 3 – Task Decomposition  
**Vai trò đảm trách:** AI Technical Project Lead & Full-stack Architect  
**Tài liệu căn cứ:** [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module4/SPEC.md), [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module4/PLAN.md)  
**Trạng thái:** COMPLETED (100% - 24/24 Tasks Passed)  

---

## 1. Nguyên Tắc & Quy Chuẩn Thực Thi
- **Tính nguyên tử (Atomic)**: Mỗi task độc lập, có thể verify và test riêng biệt.
- **Test-Driven / Test-First**: Luôn viết và chạy test song hành với mã nguồn.
- **Tiêu chuẩn hoàn thành (DoD)**: Mã nguồn hoàn tất, biên dịch thành công không lỗi linter/compiler, unit test tương ứng đổi màu xanh (PASS).

---

## 2. Bảng Phân Rã Chi Tiết Các Hạng Mục

### Nhóm 1: Backend Domain & Entity Extensions (`[BE-DOM]`)
- [x] `[BE-DOM-001]`: Mở rộng Entity `ActionCard` với phương thức nghiệp vụ `UpdateUrgencyStage(UrgencyStage newStage)` và `ToggleCompletion()`, tăng `RowVersion` tương ứng.
  - **Files**: `backend/src/Asseta.Domain/Entities/ActionCard.cs`
  - **EARS Ref**: `[REQ-PLAN-004]`, `[REQ-PLAN-006]`, `[REQ-PLAN-010]`
  - **DoD**: Unit test kiểm tra chuyển đổi stage và toggle completion cập nhật đúng trạng thái và tăng RowVersion.
- [x] `[BE-DOM-002]`: Triển khai Domain Service `PlanReadinessCalculator` tính toán điểm sẵn sàng kế hoạch có trọng số theo 4 giai đoạn, tỷ lệ bao phủ người ủy thác (`DelegateCoveragePercentage`), tỷ lệ sẵn sàng giấy tờ (`DocumentReadinessPercentage`), và phát hiện rủi ro điểm nghẽn đơn lẻ SPoF (> 70% việc giai đoạn khẩn cấp gán cho 1 người).
  - **Files**: `backend/src/Asseta.Domain/Services/PlanReadinessCalculator.cs`
  - **EARS Ref**: `[REQ-PLAN-008]`, `[REQ-PLAN-019]`, `[REQ-PLAN-020]`
  - **DoD**: Unit test `PlanReadinessCalculatorTests` kiểm tra chính xác công thức toán học và cảnh báo SPoF.

### Nhóm 2: Backend Application DTOs & CQRS Commands/Queries (`[BE-APP]`)
- [x] `[BE-APP-001]`: Triển khai DTOs cho Continuity Plan (`ContinuityPlanDto`, `ContinuityPlanStageDto`, `ContinuityPlanCardItemDto`, `OfflineEmergencyBriefDto`, `PlanReadinessAuditDto`).
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/DTOs/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`, `[REQ-PLAN-008]`
  - **DoD**: DTO map dữ liệu đầy đủ, loại trừ hoàn toàn các trường cipher nhạy cảm.
- [x] `[BE-APP-002]`: Triển khai `GetContinuityPlanQuery` & Handler gom nhóm 4 mốc thời gian, sắp xếp theo Priority, tính toán chỉ số KPI và đánh dấu `HasStageGap = true` cho thẻ thiếu người/hồ sơ ở 2 giai đoạn đầu.
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Queries/GetContinuityPlan/GetContinuityPlanQuery.cs`, `GetContinuityPlanQueryHandler.cs`
  - **EARS Ref**: `[REQ-PLAN-001]`, `[REQ-PLAN-005]`, `[REQ-PLAN-009]`
  - **DoD**: Query trả về cấu trúc 4 stages đầy đủ theo OwnerId.
- [x] `[BE-APP-003]`: Triển khai `UpdateActionCardStageCommand` & Handler kèm `UpdateActionCardStageCommandValidator` (kiểm tra `UrgencyStage` hợp lệ 1..4, kiểm tra `RowVersion`, ghi audit log).
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Commands/UpdateActionCardStage/`
  - **EARS Ref**: `[REQ-PLAN-003]`, `[REQ-PLAN-006]`, `[REQ-PLAN-013]`, `[REQ-PLAN-015]`
  - **DoD**: Handler cập nhật thành công stage của card và phát sinh audit log.
- [x] `[BE-APP-004]`: Triển khai `ToggleActionCardCompletionCommand` & Handler để đánh dấu hoàn tất kiểm tra sẵn sàng cho thẻ hành động.
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Commands/ToggleActionCardCompletion/`
  - **EARS Ref**: `[REQ-PLAN-003]`, `[REQ-PLAN-010]`
  - **DoD**: Đảo trạng thái IsCompleted và tăng RowVersion.
- [x] `[BE-APP-005]`: Triển khai `GetPlanEmergencyBriefQuery` & Handler trích xuất bản tóm lược khẩn cấp Zero-Knowledge (không có ciphertext) phân theo timeline phục vụ in ấn / ngoại tuyến.
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Queries/GetPlanEmergencyBrief/`
  - **EARS Ref**: `[REQ-PLAN-002]`, `[REQ-PLAN-007]`
  - **DoD**: Query trả về brief sạch chỉ gồm task, contacts, document location hint và delegate.
- [x] `[BE-APP-006]`: Triển khai `GetPlanReadinessAuditQuery` & Handler thực hiện kiểm toán toàn diện kế hoạch, phát hiện danh sách thẻ hổng và rủi ro điểm nghẽn SPoF.
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Queries/GetPlanReadinessAudit/`
  - **EARS Ref**: `[REQ-PLAN-008]`, `[REQ-PLAN-017]`
  - **DoD**: Báo cáo kiểm toán liệt kê chính xác các thẻ gap và cảnh báo SPoF nếu có.
- [x] `[BE-APP-007]`: Triển khai `GetMyDelegatedPlanQuery` & Handler cho Người Ủy Thác, lọc các thẻ được phân quyền theo mốc thời gian (Zero-Disclosure).
  - **Files**: `backend/src/Asseta.Application/Features/ContinuityPlan/Queries/GetMyDelegatedPlan/`
  - **EARS Ref**: `[REQ-PLAN-011]`
  - **DoD**: Trả về đúng các thẻ delegate được phân quyền xem.

### Nhóm 3: Backend API Controller & Error Codes (`[BE-API]`)
- [x] `[BE-API-001]`: Triển khai `ContinuityPlanController` với các endpoints: `GET /api/v1/continuity-plan`, `PATCH /api/v1/continuity-plan/cards/{id}/stage`, `PATCH /api/v1/continuity-plan/cards/{id}/toggle-completion`, `GET /api/v1/continuity-plan/emergency-brief`, `GET /api/v1/continuity-plan/audit`, `GET /api/v1/continuity-plan/delegated`.
  - **Files**: `backend/src/Asseta.Api/Controllers/ContinuityPlanController.cs`
  - **EARS Ref**: `[REQ-PLAN-001]`, `[REQ-PLAN-005]`, `[REQ-PLAN-006]`, `[REQ-PLAN-007]`, `[REQ-PLAN-008]`, `[REQ-PLAN-011]`
  - **DoD**: Các endpoints trả về HTTP Envelope chuẩn, xác thực token đầy đủ.
- [x] `[BE-API-002]`: Đăng ký mã lỗi `INVALID_URGENCY_STAGE` trong `GlobalExceptionMiddleware` và Domain Exceptions nếu cần.
  - **Files**: `backend/src/Asseta.Api/Middlewares/GlobalExceptionMiddleware.cs`, `backend/src/Asseta.Application/Common/Exceptions/ApplicationExceptions.cs`
  - **EARS Ref**: `[REQ-PLAN-013]`
  - **DoD**: Middleware chuyển đổi ArgumentException thành mã lỗi chuẩn RFC 7807.

### Nhóm 4: Backend Unit & Integration Tests (`[BE-TEST]`)
- [x] `[BE-TEST-001]`: Viết Unit Tests `PlanReadinessCalculatorTests` kiểm tra tính điểm có trọng số, tỷ lệ bao phủ, và thuật toán cảnh báo SPoF.
  - **Files**: `backend/tests/Asseta.UnitTests/Domain/PlanReadinessCalculatorTests.cs`
  - **EARS Ref**: `[REQ-PLAN-008]`, `[REQ-PLAN-019]`, `[REQ-PLAN-020]`
  - **DoD**: Test cases chạy PASS 100%.
- [x] `[BE-TEST-002]`: Viết Unit Tests `ContinuityPlanHandlerTests` cho các Handlers (`GetContinuityPlan`, `UpdateActionCardStage`, `GetPlanEmergencyBrief`, `GetPlanReadinessAudit`, `GetMyDelegatedPlan`).
  - **Files**: `backend/tests/Asseta.UnitTests/Application/ContinuityPlan/ContinuityPlanHandlerTests.cs`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-006]`, `[REQ-PLAN-007]`, `[REQ-PLAN-008]`, `[REQ-PLAN-011]`
  - **DoD**: Test cases chạy PASS 100%.
- [x] `[BE-TEST-003]`: Viết Integration Tests `ContinuityPlanApiTests` với WebApplicationFactory kiểm tra toàn diện API: tổng hợp 4 stages, chuyển stage thẻ, kiểm tra quyền Delegate Scoped View, Audit gap detection.
  - **Files**: `backend/tests/Asseta.IntegrationTests/ContinuityPlanApiTests.cs`
  - **EARS Ref**: `[REQ-PLAN-001]`, `[REQ-PLAN-005]`, `[REQ-PLAN-006]`, `[REQ-PLAN-008]`, `[REQ-PLAN-011]`
  - **DoD**: Toàn bộ Integration Tests chạy PASS 100%.

### Nhóm 5: Frontend Web (`[WEB-FEAT]`)
- [x] `[WEB-FEAT-001]`: Khởi tạo TypeScript interfaces cho Continuity Plan (`types/continuityPlan.ts`).
  - **Files**: `frontend-web/src/types/continuityPlan.ts`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`, `[REQ-PLAN-008]`
  - **DoD**: File types biên dịch không lỗi.
- [x] `[WEB-FEAT-002]`: Triển khai `continuityPlanService.ts` và React Query hooks `useContinuityPlan.ts`.
  - **Files**: `frontend-web/src/services/continuityPlanService.ts`, `frontend-web/src/features/ContinuityPlan/hooks/useContinuityPlan.ts`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-006]`, `[REQ-PLAN-010]`, `[REQ-PLAN-016]`
  - **DoD**: Hooks cung cấp các queries và mutations đầy đủ.
- [x] `[WEB-FEAT-003]`: Xây dựng UI Components cho Continuity Plan (`PlanMetricsHeader`, `ContinuityTimelineStage`, `ContinuityPlanCardItem`, `EmergencyBriefModal`, `PlanReadinessAuditModal`).
  - **Files**: `frontend-web/src/features/ContinuityPlan/components/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`, `[REQ-PLAN-008]`, `[REQ-PLAN-009]`
  - **DoD**: Components render đẹp mắt, có badge cảnh báo gap, hỗ trợ chuyển stage.
- [x] `[WEB-FEAT-004]`: Cập nhật `ContinuityPlanView.tsx` tích hợp đầy đủ header KPI, chuyển đổi chế độ xem Timeline / Kanban 4 cột, bộ lọc theo danh mục / người phụ trách và nút xuất bản khẩn cấp.
  - **Files**: `frontend-web/src/features/ContinuityPlan/ContinuityPlanView.tsx`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`
  - **DoD**: Trang Continuity Plan hoạt động mượt mà với mock & live API.
- [x] `[WEB-FEAT-005]`: Viết Unit Test `ContinuityPlan.test.ts` kiểm thử logic phân loại 4 mốc thời gian, tính toán điểm sẵn sàng và hiển thị cảnh báo gap.
  - **Files**: `frontend-web/src/features/ContinuityPlan/__tests__/ContinuityPlan.test.ts`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-008]`, `[REQ-PLAN-009]`
  - **DoD**: `npm test` pass toàn bộ.

### Nhóm 6: Mobile App Flutter (`[MOB-FEAT]`)
- [x] `[MOB-FEAT-001]`: Tạo Domain Entities và Repository Contract (`continuity_plan_entity.dart`, `continuity_plan_repository.dart`).
  - **Files**: `mobile-app/lib/features/continuity_plan/domain/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`
  - **DoD**: Entities khai báo đầy đủ các trường nghiệp vụ.
- [x] `[MOB-FEAT-002]`: Triển khai Data Layer (`continuity_plan_model.dart`, `plan_remote_datasource.dart`, `plan_repository_impl.dart`).
  - **Files**: `mobile-app/lib/features/continuity_plan/data/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-012]`
  - **DoD**: Model serialize/deserialize JSON chính xác, hỗ trợ cache offline.
- [x] `[MOB-FEAT-003]`: Triển khai State Management `ContinuityPlanBloc`, Events và States.
  - **Files**: `mobile-app/lib/features/continuity_plan/presentation/bloc/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-006]`, `[REQ-PLAN-010]`
  - **DoD**: BLoC xử lý tải plan, chuyển đổi stage thẻ, toggle hoàn tất.
- [x] `[MOB-FEAT-004]`: Xây dựng UI Presentation (`continuity_plan_page.dart`, `emergency_brief_page.dart`, `stage_timeline_tile.dart`, `gap_alert_banner.dart`).
  - **Files**: `mobile-app/lib/features/continuity_plan/presentation/`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-007]`, `[REQ-PLAN-009]`, `[REQ-PLAN-012]`
  - **DoD**: Giao diện timeline cuộn mượt mà với Segmented control cho 4 mốc thời gian.
- [x] `[MOB-FEAT-005]`: Viết Unit Tests `continuity_plan_bloc_test.dart` kiểm tra luồng phát sinh state của BLoC.
  - **Files**: `mobile-app/test/features/continuity_plan/continuity_plan_bloc_test.dart`
  - **EARS Ref**: `[REQ-PLAN-005]`, `[REQ-PLAN-006]`
  - **DoD**: `flutter test` pass toàn bộ.

---

## 3. Tổng Kết Khối Lượng Công Việc
- **Tổng số tasks**: 24 nhiệm vụ nguyên tử.
- **Tầng Backend**: 11 tasks.
- **Tầng Frontend Web**: 5 tasks.
- **Tầng Mobile App**: 5 tasks.
- **Kiểm thử & QA**: 3 test suites chuyên biệt.

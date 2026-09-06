# Danh Sách Công Việc Phân Rã Nguyên Tử (Task Decomposition): Module 5 – Safe Activation

**Mã Module:** `module5` (Tương đương `feat-05-safe-activation`)  
**Pha phát triển:** Pha 3 – Task Decomposition  
**Vai trò đảm trách:** AI Technical Project Lead & Full-stack Architect  
**Tài liệu căn cứ:** [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module5/SPEC.md), [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module5/PLAN.md)  
**Trạng thái:** READY FOR IMPLEMENTATION  

---

## 1. Nguyên Tắc & Quy Chuẩn Thực Thi
- **Tính nguyên tử (Atomic)**: Mỗi task độc lập, có thể verify và test riêng biệt.
- **Test-Driven / Test-First**: Luôn viết và chạy test song hành với mã nguồn.
- **Tiêu chuẩn hoàn thành (DoD)**: Mã nguồn hoàn tất, biên dịch thành công không lỗi linter/compiler, unit test tương ứng đổi màu xanh (PASS).

---

## 2. Bảng Phân Rã Chi Tiết Các Hạng Mục

### Nhóm 1: Backend Domain & Entity Extensions (`[BE-DOM]`)
- [x] `[BE-DOM-001]`: Khởi tạo Enums (`HeartbeatStatus`, `ActivationTriggerSource`, `ActivationRequestStatus`).
  - **Files**: `backend/src/Asseta.Domain/Enums/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-007]`, `[REQ-ACT-012]`
  - **DoD**: Enums định nghĩa đầy đủ các trạng thái nghiệp vụ.
- [x] `[BE-DOM-002]`: Khởi tạo Entity `OwnerActivationConfig` quản lý cấu hình chu kỳ check-in, thời gian đệm, ngưỡng xác nhận và phương thức `RecordCheckIn(int intervalDays)`.
  - **Files**: `backend/src/Asseta.Domain/Entities/OwnerActivationConfig.cs`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-004]`, `[REQ-ACT-005]`, `[REQ-ACT-006]`
  - **DoD**: Entity hỗ trợ tăng RowVersion khi check-in hoặc cập nhật cấu hình.
- [x] `[BE-DOM-003]`: Khởi tạo Entity `ActivationRequest` và `ActivationConfirmation` hỗ trợ phát động yêu cầu, thêm phiếu biểu quyết, hủy 1-chạm `CancelByOwner()`, và kích hoạt khẩn cấp `ActivateEmergency()`.
  - **Files**: `backend/src/Asseta.Domain/Entities/ActivationRequest.cs`, `ActivationConfirmation.cs`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-009]`, `[REQ-ACT-010]`
  - **DoD**: Entity quản lý vòng đời yêu cầu kích hoạt an toàn.
- [x] `[BE-DOM-004]`: Khởi tạo Domain Events (`VitalityCheckInRecordedEvent`, `ActivationRequestInitiatedEvent`, `ActivationRequestCancelledEvent`, `PlanActivatedEmergencyEvent`, `PlanEmergencyDeactivatedEvent`).
  - **Files**: `backend/src/Asseta.Domain/Events/SafeActivationEvents.cs`
  - **EARS Ref**: `[REQ-ACT-003]`, `[REQ-ACT-005]`, `[REQ-ACT-008]`, `[REQ-ACT-011]`
  - **DoD**: Events định nghĩa đầy đủ thông tin phục vụ ghi audit log.

### Nhóm 2: Backend Application DTOs & CQRS Commands/Queries (`[BE-APP]`)
- [x] `[BE-APP-001]`: Khởi tạo DTOs cho Safe Activation (`ActivationStatusDto`, `ActivationConfigDto`, `ActivationRequestDto`, `ActivationConfirmationDto`).
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/DTOs/`
  - **EARS Ref**: `[REQ-ACT-002]`, `[REQ-ACT-020]`
  - **DoD**: DTO map dữ liệu đầy đủ, loại trừ hoàn toàn các trường cipher nhạy cảm.
- [x] `[BE-APP-002]`: Triển khai `GetActivationStatusQuery` & Handler trả về trạng thái tổng thể, hạn check-in, cấu hình và countdown đếm ngược.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Queries/GetActivationStatus/`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-013]`, `[REQ-ACT-020]`
  - **DoD**: Query trả về trạng thái chi tiết theo OwnerId hoặc Delegate.
- [x] `[BE-APP-003]`: Triển khai `VitalityCheckInCommand` & Handler cập nhật `LastCheckInAtUtc`, reset chu kỳ và ghi audit log.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/VitalityCheckIn/`
  - **EARS Ref**: `[REQ-ACT-003]`, `[REQ-ACT-005]`
  - **DoD**: Check-in thành công gia hạn `NextCheckInDueUtc`.
- [x] `[BE-APP-004]`: Triển khai `UpdateActivationConfigCommand` & Validator & Handler cập nhật chu kỳ, thời gian đệm và ngưỡng xác nhận.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/UpdateActivationConfig/`
  - **EARS Ref**: `[REQ-ACT-006]`
  - **DoD**: Validator từ chối giá trị ngoài biên, Handler cập nhật thành công.
- [x] `[BE-APP-005]`: Triển khai `InitiateActivationRequestCommand` & Validator & Handler cho Người Ủy Thác cấp 2/3 khởi động thời gian đệm 48h.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/InitiateActivationRequest/`
  - **EARS Ref**: `[REQ-ACT-007]`, `[REQ-ACT-015]`, `[REQ-ACT-016]`
  - **DoD**: Chặn Level 1, tạo yêu cầu ở trạng thái `PendingGracePeriod`.
- [x] `[BE-APP-006]`: Triển khai `CancelActivationRequestCommand` & Handler cho Chủ tài sản hủy 1-chạm trong thời gian đệm.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/CancelActivationRequest/`
  - **EARS Ref**: `[REQ-ACT-008]`, `[REQ-ACT-017]`
  - **DoD**: Hủy ngay lập tức yêu cầu đang chạy đệm, giữ an toàn bình thường.
- [x] `[BE-APP-007]`: Triển khai `ConfirmActivationRequestCommand` & Handler cho Người Ủy Thác khác biểu quyết xác nhận.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/ConfirmActivationRequest/`
  - **EARS Ref**: `[REQ-ACT-009]`, `[REQ-ACT-010]`
  - **DoD**: Lưu phiếu xác nhận, chuyển trạng thái khi đủ điều kiện.
- [x] `[BE-APP-008]`: Triển khai `DeactivateEmergencyPlanCommand` & Handler cho Chủ tài sản khôi phục quyền kiểm soát và tắt trạng thái khẩn cấp.
  - **Files**: `backend/src/Asseta.Application/Features/SafeActivation/Commands/DeactivateEmergencyPlan/`
  - **EARS Ref**: `[REQ-ACT-011]`
  - **DoD**: Đưa kế hoạch về trạng thái `NORMAL`, cắt quyền khẩn cấp.

### Nhóm 3: Backend Infrastructure & API (`[BE-INFRA]` & `[BE-API]`)
- [x] `[BE-INFRA-001]`: Khởi tạo EF Core Configurations (`OwnerActivationConfigConfiguration`, `ActivationRequestConfiguration`, `ActivationConfirmationConfiguration`), cập nhật `IAssetaDbContext` và `AssetaDbContext`.
  - **Files**: `backend/src/Asseta.Infrastructure/Persistence/Configurations/`, `AssetaDbContext.cs`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-004]`
  - **DoD**: Schema mapping chính xác với PostgreSQL, hỗ trợ indexes và concurrency token.
- [x] `[BE-API-001]`: Triển khai `SafeActivationController` với đầy đủ 7 endpoints RESTful chuẩn HTTP envelope.
  - **Files**: `backend/src/Asseta.Api/Controllers/SafeActivationController.cs`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-005]`, `[REQ-ACT-006]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-009]`, `[REQ-ACT-011]`
  - **DoD**: Endpoints trả về mã trạng thái HTTP chuẩn RFC 7807.

### Nhóm 4: Backend Unit & Integration Tests (`[BE-TEST]`)
- [x] `[BE-TEST-001]`: Viết Unit Tests `SafeActivationHandlerTests.cs` kiểm tra toàn diện các Handlers và Validators.
  - **Files**: `backend/tests/Asseta.UnitTests/Application/SafeActivation/SafeActivationHandlerTests.cs`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-006]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-011]`
  - **DoD**: Test cases chạy PASS 100%.
- [x] `[BE-TEST-002]`: Viết Integration Tests `SafeActivationApiTests.cs` kiểm tra luồng API toàn trình: Check-in, Delegate Initiate, Owner 1-tap Cancel, Quorum Confirm, Emergency Activate, Owner Deactivate.
  - **Files**: `backend/tests/Asseta.IntegrationTests/SafeActivationApiTests.cs`
  - **EARS Ref**: `[REQ-ACT-001]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-010]`, `[REQ-ACT-011]`
  - **DoD**: Integration Tests chạy PASS 100%.

### Nhóm 5: Frontend Web (`[WEB-FEAT]`)
- [x] `[WEB-FEAT-001]`: Khởi tạo TypeScript interfaces cho Safe Activation (`types/safeActivation.ts`).
  - **Files**: `frontend-web/src/types/safeActivation.ts`
  - **EARS Ref**: `[REQ-ACT-002]`, `[REQ-ACT-020]`
  - **DoD**: Interfaces khai báo đầy đủ các models.
- [x] `[WEB-FEAT-002]`: Triển khai `safeActivationService.ts` và React Query hooks `useSafeActivation.ts`.
  - **Files**: `frontend-web/src/services/safeActivationService.ts`, `frontend-web/src/features/SafeActivation/hooks/useSafeActivation.ts`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`
  - **DoD**: Cung cấp các hàm gọi API và mutations tương ứng.
- [x] `[WEB-FEAT-003]`: Xây dựng UI Components (`SafeActivationPanel.tsx`, `VitalityCheckInCard`, `EmergencyCountdownBanner`, `ActivationConfigModal`, `InitiateActivationModal`).
  - **Files**: `frontend-web/src/features/SafeActivation/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-013]`
  - **DoD**: Giao diện hiển thị trực quan nút điểm danh, thanh đếm ngược và nút hủy 1-chạm.
- [x] `[WEB-FEAT-004]`: Viết Unit Test `SafeActivation.test.ts` kiểm thử logic điểm danh, đếm ngược time-lock và hủy yêu cầu.
  - **Files**: `frontend-web/src/features/SafeActivation/__tests__/SafeActivation.test.ts`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-008]`
  - **DoD**: Vitest pass toàn bộ.

### Nhóm 6: Mobile App Flutter (`[MOB-FEAT]`)
- [x] `[MOB-FEAT-001]`: Tạo Domain Entities và Repository Contract (`safe_activation_entity.dart`, `safe_activation_repository.dart`).
  - **Files**: `mobile-app/lib/features/safe_activation/domain/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-020]`
  - **DoD**: Entities khai báo đầy đủ các trường nghiệp vụ.
- [x] `[MOB-FEAT-002]`: Triển khai Data Layer (`safe_activation_model.dart`, `safe_activation_remote_datasource.dart`, `safe_activation_repository_impl.dart`).
  - **Files**: `mobile-app/lib/features/safe_activation/data/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-014]`
  - **DoD**: Model serialize/deserialize JSON chính xác.
- [x] `[MOB-FEAT-003]`: Triển khai State Management `SafeActivationBloc`, Events và States.
  - **Files**: `mobile-app/lib/features/safe_activation/presentation/bloc/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-007]`, `[REQ-ACT-008]`, `[REQ-ACT-011]`
  - **DoD**: BLoC xử lý điểm danh, phát động yêu cầu, hủy 1-chạm và khôi phục.
- [x] `[MOB-FEAT-004]`: Xây dựng UI Presentation (`safe_activation_page.dart`, `vitality_checkin_button.dart`, `emergency_countdown_banner.dart`).
  - **Files**: `mobile-app/lib/features/safe_activation/presentation/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-008]`, `[REQ-ACT-013]`
  - **DoD**: Giao diện mobile với nút điểm danh hero lớn và banner đếm ngược khẩn cấp.
- [x] `[MOB-FEAT-005]`: Viết Unit Tests `safe_activation_bloc_test.dart` và `safe_activation_model_test.dart`.
  - **Files**: `mobile-app/test/features/safe_activation/`
  - **EARS Ref**: `[REQ-ACT-005]`, `[REQ-ACT-008]`
  - **DoD**: `flutter test` pass toàn bộ.

---

## 3. Tổng Kết Khối Lượng Công Việc
- **Tổng số tasks**: 24 nhiệm vụ nguyên tử.
- **Tầng Backend**: 12 tasks.
- **Tầng Frontend Web**: 4 tasks.
- **Tầng Mobile App**: 5 tasks.
- **Kiểm thử & QA**: 3 test suites chuyên biệt.

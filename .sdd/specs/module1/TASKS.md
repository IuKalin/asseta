# Danh Sách Nhiệm Vụ Phân Rã Nguyên Tử (Atomic Tasks): Module 1 – Continuity Map

**Mã Module:** `module1` (Tương đương `feat-01-continuity-map`)  
**Pha phát triển:** Pha 3 – Task Decomposition  
**Vai trò đảm trách:** AI Technical Project Lead  
**Tài liệu căn cứ:** [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module1/SPEC.md) & [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module1/PLAN.md)  
**Trạng thái:** ACTIVE / READY FOR IMPLEMENTATION  

---

## 1. Nguyên Tắc Quản Trị & Thực Thi Task

1. **Tính Nguyên Tử (Atomic & Independent)**: Mỗi task tập trung vào một đơn vị chức năng duy nhất, thời gian ước lượng $\le 4$ giờ làm việc.
2. **Khả Năng Kiểm Chứng (Verifiable)**: Mỗi task bắt buộc phải có tiêu chí Definition of Done (DoD) với lệnh kiểm thử cụ thể (`dotnet test`, `npm run test`, `flutter test`).
3. **Truy Xuất Nguồn Gốc (Traceability)**: Mọi task đều liên kết chặt chẽ với điều khoản EARS tương ứng trong `SPEC.md`.
4. **Phân Loại Ký Hiệu**:
   - `[BE-CORE]`: Mã nguồn Backend (.NET 8 Clean Architecture) & Database PostgreSQL.
   - `[WEB-SHELL]`: Giao diện Frontend Web (React 18 + Vite + TypeScript).
   - `[MOB-SHELL]`: Ứng dụng Di động (Flutter 3.x + BLoC).
   - `[INTEG-TEST]`: Kiểm thử Tích hợp Toàn trình, An toàn Mật mã & Tuân thủ Spec.

---

## 2. Bảng Phân Rã Nhiệm Vụ Chi Tiết

### 2.1. Phân Hệ Backend Core (.NET 8 & PostgreSQL 16)

#### `[MOD1-T001]` [BE-CORE]: Khởi tạo Domain Entities, Enums, Value Objects & Domain Events
- **Action**: Tạo các thực thể nghiệp vụ cốt lõi `ContinuityCategory`, `ContinuityItem`, `ContinuityAuditLog`, `ContinuityAssessmentHistory`, các Value Objects `PriorityLevel`, `CipherBlobPayload`, `ReadinessScore`, và các Domain Events tương ứng.
- **Files**:
  - `backend/src/Asseta.Domain/Entities/ContinuityCategory.cs`
  - `backend/src/Asseta.Domain/Entities/ContinuityItem.cs`
  - `backend/src/Asseta.Domain/Entities/ContinuityAuditLog.cs`
  - `backend/src/Asseta.Domain/Entities/ContinuityAssessmentHistory.cs`
  - `backend/src/Asseta.Domain/Enums/PriorityLevel.cs`
  - `backend/src/Asseta.Domain/ValueObjects/CipherBlobPayload.cs`
  - `backend/src/Asseta.Domain/ValueObjects/ReadinessScore.cs`
  - `backend/src/Asseta.Domain/Events/ContinuityEvents.cs`
- **EARS Ref**: `[REQ-MAP-001]`, `[REQ-MAP-002]`, `[REQ-MAP-003]`, `[REQ-MAP-004]`
- **Definition of Done (DoD)**: Lệnh `dotnet build backend/src/Asseta.Domain` hoàn thành không lỗi; Unit test kiểm chứng tính bất biến của Value Objects chạy xanh.

- [x] `[MOD1-T001]` [BE-CORE]: Khởi tạo Domain Entities, Enums, Value Objects & Domain Events

#### `[MOD1-T002]` [BE-CORE]: Triển khai Thuật Toán Tính Toán Readiness Score & Domain Services
- **Action**: Cài đặt dịch vụ nghiệp vụ `ReadinessScoreCalculator` tính điểm phần trăm theo trọng số (Critical 50%, Important 35%, Low 15%) và thuật toán nhận diện `Continuity Gap`.
- **Files**:
  - `backend/src/Asseta.Domain/Services/ReadinessScoreCalculator.cs`
  - `backend/tests/Asseta.Domain.UnitTests/ReadinessScoreCalculatorTests.cs`
- **EARS Ref**: `[REQ-MAP-011]`, `[REQ-MAP-020]`, `[REQ-MAP-021]`
- **Definition of Done (DoD)**: Lệnh `dotnet test backend/tests/Asseta.Domain.UnitTests` chạy xanh các ca kiểm thử: danh mục rỗng trả về 0%, item Critical thiếu người tiếp quản kích hoạt cờ Gap, item đầy đủ đạt 100%.

- [x] `[MOD1-T002]` [BE-CORE]: Triển khai Thuật Toán Tính Toán Readiness Score & Domain Services

#### `[MOD1-T003]` [BE-CORE]: Cấu hình EF Core 8 DbContext, Entity Configurations, Seed Data & Migrations
- **Action**: Thiết lập ánh xạ bảng PostgreSQL qua Fluent API, cấu hình chỉ mục (Index), token Concurrency `row_version`, bộ lọc toàn cục Soft-delete và seed data 6 danh mục chuẩn.
- **Files**:
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ContinuityCategoryConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ContinuityItemConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ContinuityAuditLogConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ContinuityAssessmentHistoryConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/AssetaDbContext.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Migrations/`
- **EARS Ref**: `[REQ-MAP-001]`, `[REQ-MAP-004]`, `[REQ-MAP-017]`
- **Definition of Done (DoD)**: Tạo Migration thành công; script migration sinh ra chính xác các câu lệnh DDL theo đúng [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module1/PLAN.md#2-thi%E1%BA%BFt-k%E1%BA%BF-c%C6%A1-s%E1%BB%9F-d%E1%BB%AF-li%E1%BB%87u-chi-ti%E1%BA%BFt-postgresql-16-schema).

- [x] `[MOD1-T003]` [BE-CORE]: Cấu hình EF Core 8 DbContext, Entity Configurations, Seed Data & Migrations

#### `[MOD1-T004]` [BE-CORE]: Cài đặt Redis Idempotency Service & Distributed Lock
- **Action**: Triển khai dịch vụ cache kết quả mutation theo `Idempotency-Key` với TTL 24 giờ và distributed lock ngắn hạn chống race condition.
- **Files**:
  - `backend/src/Asseta.Application/Common/Interfaces/IIdempotencyService.cs`
  - `backend/src/Asseta.Infrastructure/Services/RedisIdempotencyService.cs`
  - `backend/tests/Asseta.Infrastructure.UnitTests/RedisIdempotencyServiceTests.cs`
- **EARS Ref**: `[REQ-MAP-016]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: gửi 2 request cùng key thì request thứ hai trả về kết quả đã cache mà không gọi lại handler.

- [x] `[MOD1-T004]` [BE-CORE]: Cài đặt Redis Idempotency Service & Distributed Lock

#### `[MOD1-T005]` [BE-CORE]: Triển khai MediatR Commands & FluentValidation cho Continuity Items
- **Action**: Viết các Commands `CreateContinuityItemCommand`, `UpdateContinuityItemCommand`, `DeleteContinuityItemCommand`, `ReorderContinuityItemsCommand` kèm validators và handlers xử lý logic.
- **Files**:
  - `backend/src/Asseta.Application/Features/ContinuityMap/Commands/CreateContinuityItem/`
  - `backend/src/Asseta.Application/Features/ContinuityMap/Commands/UpdateContinuityItem/`
  - `backend/src/Asseta.Application/Features/ContinuityMap/Commands/DeleteContinuityItem/`
  - `backend/src/Asseta.Application/Features/ContinuityMap/Commands/ReorderContinuityItems/`
  - `backend/tests/Asseta.Application.UnitTests/ContinuityCommandsTests.cs`
- **EARS Ref**: `[REQ-MAP-006]`, `[REQ-MAP-007]`, `[REQ-MAP-008]`, `[REQ-MAP-013]`, `[REQ-MAP-014]`
- **Definition of Done (DoD)**: `dotnet test` bao phủ validation lỗi tên rỗng, danh mục sai và phát hiện dữ liệu nhạy cảm chưa mã hóa bị từ chối với 422.

- [x] `[MOD1-T005]` [BE-CORE]: Triển khai MediatR Commands & FluentValidation cho Continuity Items

#### `[MOD1-T006]` [BE-CORE]: Triển khai MediatR Queries & DTOs cho Continuity Map
- **Action**: Viết các Queries `GetContinuityMapQuery`, `GetContinuityItemByIdQuery`, `GetContinuityGapsQuery` mapping sang DTO chuẩn Envelope.
- **Files**:
  - `backend/src/Asseta.Application/Features/ContinuityMap/DTOs/`
  - `backend/src/Asseta.Application/Features/ContinuityMap/Queries/GetContinuityMap/`
  - `backend/src/Asseta.Application/Features/ContinuityMap/Queries/GetContinuityGaps/`
  - `backend/tests/Asseta.Application.UnitTests/ContinuityQueriesTests.cs`
- **EARS Ref**: `[REQ-MAP-009]`, `[REQ-MAP-022]`
- **Definition of Done (DoD)**: Unit test kiểm chứng kết quả trả về cây 6 danh mục kèm điểm số, chỉ trả về items có `is_deleted = false`.

- [x] `[MOD1-T006]` [BE-CORE]: Triển khai MediatR Queries & DTOs cho Continuity Map

#### `[MOD1-T007]` [BE-CORE]: Triển khai Onboarding Assessment Handler & Assessment History
- **Action**: Viết `SubmitContinuityAssessmentCommand` tiếp nhận bộ câu hỏi khảo sát, lưu bản ghi thô vào `continuity_assessment_history` và tự động sinh các `ContinuityItem` ban đầu.
- **Files**:
  - `backend/src/Asseta.Application/Features/ContinuityMap/Commands/SubmitAssessment/`
  - `backend/tests/Asseta.Application.UnitTests/SubmitAssessmentCommandHandlerTests.cs`
- **EARS Ref**: `[REQ-MAP-005]`
- **Definition of Done (DoD)**: Unit test verify: nộp 12 câu hỏi khảo sát tạo thành công 1 bản ghi history và danh sách items tương ứng.

- [x] `[MOD1-T007]` [BE-CORE]: Triển khai Onboarding Assessment Handler & Assessment History

#### `[MOD1-T008]` [BE-CORE]: Xây dựng Controllers, Middlewares & Sensitive Data Filter
- **Action**: Tạo `ContinuityMapController`, `ContinuityItemsController`, Middlewares xử lý Idempotency, Global Exception Envelope và Regex kiểm tra dữ liệu nhạy cảm.
- **Files**:
  - `backend/src/Asseta.Api/Controllers/ContinuityMapController.cs`
  - `backend/src/Asseta.Api/Controllers/ContinuityItemsController.cs`
  - `backend/src/Asseta.Api/Middlewares/IdempotencyMiddleware.cs`
  - `backend/src/Asseta.Api/Middlewares/SensitiveDataInspectionMiddleware.cs`
- **EARS Ref**: `[REQ-MAP-014]`, `[REQ-MAP-015]`, `[REQ-MAP-016]`
- **Definition of Done (DoD)**: Chạy `dotnet test backend/tests/Asseta.Api.IntegrationTests` kiểm tra các status codes 200, 201, 400, 403, 404, 409, 422 đều theo đúng format Envelope.

- [x] `[MOD1-T008]` [BE-CORE]: Xây dựng Controllers, Middlewares & Sensitive Data Filter

#### `[MOD1-T009]` [BE-CORE]: Xây dựng Backend Integration Tests Toàn Diện
- **Action**: Viết bộ test tích hợp API với cơ sở dữ liệu PostgreSQL thực tế (sử dụng Testcontainers hoặc local Docker test DB).
- **Files**:
  - `backend/tests/Asseta.Api.IntegrationTests/ContinuityMapApiTests.cs`
  - `backend/tests/Asseta.Api.IntegrationTests/OptimisticConcurrencyTests.cs`
- **EARS Ref**: `[REQ-MAP-006]`, `[REQ-MAP-016]`, `[REQ-MAP-017]`
- **Definition of Done (DoD)**: Toàn bộ suite `dotnet test backend/tests/` chạy xanh 100% không cảnh báo.

- [x] `[MOD1-T009]` [BE-CORE]: Xây dựng Backend Integration Tests Toàn Diện

---

### 2.2. Phân Hệ Frontend Web (React 18 + Vite + TypeScript)

#### `[MOD1-T010]` [WEB-SHELL]: Xây dựng Web Crypto Service (Client-Side AES-256-GCM)
- **Action**: Tạo module dẫn xuất Master Key bằng PBKDF2 và mã hóa/giải mã AES-256-GCM sử dụng chuẩn Web Crypto API nguyên bản của trình duyệt.
- **Files**:
  - `frontend-web/src/services/cryptoService.ts`
  - `frontend-web/src/services/__tests__/cryptoService.test.ts`
- **EARS Ref**: `[REQ-MAP-002]`, `[REQ-MAP-018]`
- **Definition of Done (DoD)**: Lệnh `npm run test` trên tệp test crypto chạy xanh: chuỗi nhạy cảm được mã hóa ra `CipherBlob`, `Nonce`, `AuthTag` và giải mã lại chính xác.

- [x] `[MOD1-T010]` [WEB-SHELL]: Xây dựng Web Crypto Service (Client-Side AES-256-GCM)

#### `[MOD1-T011]` [WEB-SHELL]: Xây dựng TypeScript Types, API Client & TanStack Query Hooks
- **Action**: Định nghĩa interfaces dữ liệu theo Envelope chuẩn, Axios client tự động đính kèm `Idempotency-Key` (UUIDv4) và custom hooks `useContinuityMap`, `useCreateContinuityItem`, `useUpdateContinuityItem`.
- **Files**:
  - `frontend-web/src/types/continuity.ts`
  - `frontend-web/src/features/ContinuityMap/api/continuityApi.ts`
  - `frontend-web/src/features/ContinuityMap/hooks/useContinuityMap.ts`
- **EARS Ref**: `[REQ-MAP-006]`, `[REQ-MAP-009]`, `[REQ-MAP-016]`
- **Definition of Done (DoD)**: TypeScript type-check `npx tsc --noEmit` hoàn thành không có lỗi biên dịch; mock test hook verify cơ chế cache invalidation.

- [x] `[MOD1-T011]` [WEB-SHELL]: Xây dựng TypeScript Types, API Client & TanStack Query Hooks

#### `[MOD1-T012]` [WEB-SHELL]: Xây dựng UI Overview, Readiness Score Cards & Category Accordions
- **Action**: Thiết kế thành phần hiển thị tổng quan điểm số tiếp quản (Overall Readiness Score), thanh tiến độ cho 6 nhóm danh mục và danh sách accordion có thể mở rộng/thu gọn.
- **Files**:
  - `frontend-web/src/features/ContinuityMap/components/ReadinessScoreOverview.tsx`
  - `frontend-web/src/features/ContinuityMap/components/CategoryAccordion.tsx`
  - `frontend-web/src/features/ContinuityMap/ContinuityMapPage.tsx`
- **EARS Ref**: `[REQ-MAP-009]`, `[REQ-MAP-021]`
- **Definition of Done (DoD)**: Giao diện hiển thị đúng 6 nhóm danh mục, responsive trên màn hình mobile và desktop; điểm số được hiển thị sinh động theo màu sắc (Đỏ < 50%, Vàng 50-79%, Xanh >= 80%).

- [x] `[MOD1-T012]` [WEB-SHELL]: Xây dựng UI Overview, Readiness Score Cards & Category Accordions

#### `[MOD1-T013]` [WEB-SHELL]: Xây dựng Continuity Item Card, Gap Alert Banner & Item Form Modal
- **Action**: Thiết kế Card hiển thị nhãn gợi nhớ, badge ưu tiên (Critical, Important, Low), cờ Continuity Gap; Form modal thêm/sửa item tự động mã hóa trường ghi chú bí mật trước khi gọi API.
- **Files**:
  - `frontend-web/src/features/ContinuityMap/components/ContinuityItemCard.tsx`
  - `frontend-web/src/features/ContinuityMap/components/ContinuityGapBanner.tsx`
  - `frontend-web/src/features/ContinuityMap/components/ItemFormModal.tsx`
- **EARS Ref**: `[REQ-MAP-011]`, `[REQ-MAP-014]`, `[REQ-MAP-018]`
- **Definition of Done (DoD)**: Form submit thành công, payload gửi qua Network tab chỉ chứa `cipherNotesBlob`, hoàn toàn không có plaintext của ghi chú bí mật.

- [x] `[MOD1-T013]` [WEB-SHELL]: Xây dựng Continuity Item Card, Gap Alert Banner & Item Form Modal

#### `[MOD1-T014]` [WEB-SHELL]: Xây dựng Onboarding Assessment Wizard Flow
- **Action**: Xây dựng luồng wizard khảo sát 10–15 câu hỏi tình huống dẫn dắt người dùng mới tự động sinh bản đồ tiếp quản ban đầu.
- **Files**:
  - `frontend-web/src/features/ContinuityMap/components/AssessmentWizard.tsx`
  - `frontend-web/src/features/ContinuityMap/ContinuityAssessmentPage.tsx`
- **EARS Ref**: `[REQ-MAP-005]`
- **Definition of Done (DoD)**: Hoàn tất các bước wizard gọi `submitAssessment` thành công và chuyển hướng về trang ContinuityMapPage hiển thị các items mới tạo.

- [x] `[MOD1-T014]` [WEB-SHELL]: Xây dựng Onboarding Assessment Wizard Flow

#### `[MOD1-T015]` [WEB-SHELL]: Viết Component & Integration Unit Tests cho Web
- **Action**: Viết unit test cho các components và luồng người dùng bằng Vitest và React Testing Library.
- **Files**:
  - `frontend-web/src/features/ContinuityMap/__tests__/ContinuityMapPage.test.tsx`
  - `frontend-web/src/features/ContinuityMap/__tests__/ItemFormModal.test.tsx`
- **EARS Ref**: `[REQ-MAP-009]`, `[REQ-MAP-011]`
- **Definition of Done (DoD)**: Lệnh `npm run test` chạy xanh 100% tất cả các test cases của Web Shell.

- [x] `[MOD1-T015]` [WEB-SHELL]: Viết Component & Integration Unit Tests cho Web

---

### 2.3. Phân Hệ Mobile App (Flutter 3.x + BLoC)

#### `[MOD1-T016]` [MOB-SHELL]: Triển khai Mobile Crypto Service & Flutter Secure Storage
- **Action**: Cài đặt dịch vụ dẫn xuất Master Key và mã hóa AES-256-GCM, lưu trữ khóa bí mật an toàn trong Keystore (Android) và Keychain (iOS).
- **Files**:
  - `mobile-app/lib/core/crypto/crypto_service.dart`
  - `mobile-app/test/core/crypto/crypto_service_test.dart`
- **EARS Ref**: `[REQ-MAP-002]`, `[REQ-MAP-018]`
- **Definition of Done (DoD)**: Lệnh `flutter test test/core/crypto/crypto_service_test.dart` chạy xanh, giải mã chính xác chuỗi thử nghiệm.

- [x] `[MOD1-T016]` [MOB-SHELL]: Triển khai Mobile Crypto Service & Flutter Secure Storage

#### `[MOD1-T017]` [MOB-SHELL]: Xây dựng Domain Models, Local Cache & Readiness Calculator trong Dart
- **Action**: Định nghĩa các thực thể Dart, Local DataSource (Hive/SQLite) và logic tính toán Readiness Score trên client phục vụ Offline-first.
- **Files**:
  - `mobile-app/lib/features/continuity_map/domain/entities/continuity_item_entity.dart`
  - `mobile-app/lib/features/continuity_map/domain/usecases/calculate_readiness_locally.dart`
  - `mobile-app/lib/features/continuity_map/data/datasources/continuity_local_datasource.dart`
  - `mobile-app/test/features/continuity_map/domain/calculate_readiness_test.dart`
- **EARS Ref**: `[REQ-MAP-012]`, `[REQ-MAP-020]`
- **Definition of Done (DoD)**: Test vector kiểm chứng công thức tính điểm trên Dart cho kết quả chính xác 100% khớp với kết quả Backend.

- [x] `[MOD1-T017]` [MOB-SHELL]: Xây dựng Domain Models, Local Cache & Readiness Calculator trong Dart

#### `[MOD1-T018]` [MOB-SHELL]: Xây dựng Dio HTTP Client & Remote DataSource với Idempotency Interceptor
- **Action**: Cấu hình Dio client với Interceptor tự động sinh `Idempotency-Key` (UUIDv4) cho mutation, xử lý Token JWT và map response envelope.
- **Files**:
  - `mobile-app/lib/core/network/idempotency_interceptor.dart`
  - `mobile-app/lib/features/continuity_map/data/datasources/continuity_remote_datasource.dart`
  - `mobile-app/lib/features/continuity_map/data/repositories/continuity_repository_impl.dart`
- **EARS Ref**: `[REQ-MAP-006]`, `[REQ-MAP-016]`
- **Definition of Done (DoD)**: Unit test verify: Dio request gửi kèm Header `Idempotency-Key` và parse đúng JSON Envelope.

- [x] `[MOD1-T018]` [MOB-SHELL]: Xây dựng Dio HTTP Client & Remote DataSource với Idempotency Interceptor

#### `[MOD1-T019]` [MOB-SHELL]: Xây dựng ContinuityMapBloc (Events, States, Handlers)
- **Action**: Cài đặt BLoC quản lý trạng thái bản đồ tiếp quản, xử lý các sự kiện `LoadContinuityMap`, `CreateItemEvent`, `UpdateItemEvent`, `DeleteItemEvent`, `RefreshMapEvent`.
- **Files**:
  - `mobile-app/lib/features/continuity_map/presentation/bloc/continuity_map_bloc.dart`
  - `mobile-app/lib/features/continuity_map/presentation/bloc/continuity_map_event.dart`
  - `mobile-app/lib/features/continuity_map/presentation/bloc/continuity_map_state.dart`
  - `mobile-app/test/features/continuity_map/presentation/bloc/continuity_map_bloc_test.dart`
- **EARS Ref**: `[REQ-MAP-009]`, `[REQ-MAP-010]`, `[REQ-MAP-012]`
- **Definition of Done (DoD)**: Lệnh `flutter test test/features/continuity_map/presentation/bloc/` chạy xanh toàn bộ các kịch bản state transition.

- [x] `[MOD1-T019]` [MOB-SHELL]: Xây dựng ContinuityMapBloc (Events, States, Handlers)

#### `[MOD1-T020]` [MOB-SHELL]: Xây dựng Màn Hình Continuity Map Screen & Category Tiles
- **Action**: Thiết kế giao diện hiển thị biểu đồ tròn Readiness Score, danh sách 6 Category Tiles, cảnh báo Gap Badge và cơ chế Pull-to-refresh.
- **Files**:
  - `mobile-app/lib/features/continuity_map/presentation/pages/continuity_map_page.dart`
  - `mobile-app/lib/features/continuity_map/presentation/widgets/category_tile.dart`
  - `mobile-app/lib/features/continuity_map/presentation/widgets/gap_alert_card.dart`
- **EARS Ref**: `[REQ-MAP-009]`, `[REQ-MAP-011]`
- **Definition of Done (DoD)**: Widget test verify: giao diện render đầy đủ 6 danh mục, kéo thả RefreshIndicator kích hoạt tải lại dữ liệu.

- [x] `[MOD1-T020]` [MOB-SHELL]: Xây dựng Màn Hình Continuity Map Screen & Category Tiles

#### `[MOD1-T021]` [MOB-SHELL]: Xây dựng Assessment Screen & Add/Edit Item BottomSheet
- **Action**: Xây dựng UI khảo sát 10–15 câu hỏi và BottomSheet thêm/sửa hạng mục tiếp quản tích hợp mã hóa client-side trước khi dispatch event.
- **Files**:
  - `mobile-app/lib/features/continuity_map/presentation/pages/assessment_page.dart`
  - `mobile-app/lib/features/continuity_map/presentation/widgets/item_form_bottom_sheet.dart`
- **EARS Ref**: `[REQ-MAP-005]`, `[REQ-MAP-018]`
- **Definition of Done (DoD)**: Widget test kiểm tra việc điền thông tin, mã hóa ghi chú và thêm thành công item vào BLoC state.

- [x] `[MOD1-T021]` [MOB-SHELL]: Xây dựng Assessment Screen & Add/Edit Item BottomSheet

#### `[MOD1-T022]` [MOB-SHELL]: Viết Flutter Widget & Golden Tests Toàn Diện
- **Action**: Viết widget tests kiểm chứng giao diện hoạt động chính xác ở các trạng thái Loading, Loaded, Error và Offline Mode.
- **Files**:
  - `mobile-app/test/features/continuity_map/presentation/continuity_map_page_test.dart`
- **EARS Ref**: `[REQ-MAP-009]`
- **Definition of Done (DoD)**: Lệnh `flutter test` toàn bộ module chạy xanh 100%.

- [x] `[MOD1-T022]` [MOB-SHELL]: Viết Flutter Widget & Golden Tests Toàn Diện

---

### 2.4. Phân Hệ Kiểm Thử Toàn Trình & Thẩm Định Tuân Thủ

#### `[MOD1-T023]` [INTEG-TEST]: Kiểm Thử Toàn Trình Xác Thực Zero-Knowledge & An Toàn Mật Mã
- **Action**: Viết kịch bản kiểm thử E2E: Client mã hóa ghi chú $\rightarrow$ gửi qua API $\rightarrow$ kiểm tra trực tiếp bảng `continuity_items` trong database PostgreSQL chứng minh chỉ lưu `CipherBlob`, hoàn toàn không có plaintext hoặc khóa giải mã trên máy chủ.
- **Files**:
  - `backend/tests/Asseta.Api.IntegrationTests/ZeroKnowledgeVerificationTests.cs`
- **EARS Ref**: `[REQ-MAP-002]`, `[REQ-MAP-014]`, `[REQ-MAP-018]`
- **Definition of Done (DoD)**: Test tự động chạy và xác nhận 100% bản ghi trong DB không chứa chuỗi văn bản gốc; Server không thể giải mã nếu không có Master Key từ client.

- [x] `[MOD1-T023]` [INTEG-TEST]: Kiểm Thử Toàn Trình Xác Thực Zero-Knowledge & An Toàn Mật Mã

#### `[MOD1-T024]` [INTEG-TEST]: Kiểm Thử Độ Bền Idempotency & Xung Đột Đồng Thời (Concurrency Conflict)
- **Action**: Viết kiểm thử tích hợp giả lập: gửi trùng `Idempotency-Key` khi mạng chập chờn (xác nhận chỉ tạo 1 bản ghi và trả kết quả cache), và 2 client cùng sửa 1 item (`row_version` test, xác nhận client thứ 2 nhận mã 409 Conflict).
- **Files**:
  - `backend/tests/Asseta.Api.IntegrationTests/ResilienceAndConcurrencyTests.cs`
- **EARS Ref**: `[REQ-MAP-016]`, `[REQ-MAP-017]`
- **Definition of Done (DoD)**: Test chạy xanh, chứng minh tính toàn vẹn dữ liệu khi có sự cố mạng hoặc thao tác đồng thời.

- [x] `[MOD1-T024]` [INTEG-TEST]: Kiểm Thử Độ Bền Idempotency & Xung Đột Đồng Thời (Concurrency Conflict)

#### `[MOD1-T025]` [INTEG-TEST]: Lập Bảng Ma Trận Truy Xuất Yêu Cầu (Traceability Matrix Audit)
- **Action**: Rà soát đối chiếu 100% các yêu cầu EARS từ `[REQ-MAP-001]` đến `[REQ-MAP-022]`, lập bảng ma trận liên kết giữa Spec ID, file code triển khai và unit test tương ứng trong tài liệu `VALIDATION.md`.
- **Files**:
  - `.sdd/specs/module1/VALIDATION.md`
- **EARS Ref**: Toàn bộ EARS Spec
- **Definition of Done (DoD)**: 100% điều khoản SHALL có unit test chứng minh; không phát sinh bất kỳ tính năng nào ngoài phạm vi (Zero Feature Creep).

- [x] `[MOD1-T025]` [INTEG-TEST]: Lập Bảng Ma Trận Truy Xuất Yêu Cầu (Traceability Matrix Audit)

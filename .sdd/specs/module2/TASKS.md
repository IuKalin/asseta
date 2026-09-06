# Danh Sách Nhiệm Vụ Phân Rã Nguyên Tử (Atomic Tasks): Module 2 – Action Cards

**Mã Module:** `module2` (Tương đương `feat-02-action-cards`)  
**Pha phát triển:** Pha 3 – Task Decomposition  
**Vai trò đảm trách:** AI Technical Project Lead  
**Tài liệu căn cứ:** [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module2/SPEC.md) & [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module2/PLAN.md)  
**Trạng thái:** ACTIVE / READY FOR IMPLEMENTATION  

---

## 1. Nguyên Tắc Quản Trị & Thực Thi Task

1. **Tính Nguyên Tử (Atomic & Independent)**: Mỗi task tập trung vào một đơn vị chức năng duy nhất, thời gian ước lượng $\le 4$ giờ làm việc.
2. **Khả Năng Kiểm Chứng (Verifiable)**: Mỗi task bắt buộc phải có tiêu chí Definition of Done (DoD) với lệnh kiểm thử cụ thể (`dotnet test`, `npm test`, `flutter test`).
3. **Truy Xuất Nguồn Gốc (Traceability)**: Mọi task đều liên kết chặt chẽ với điều khoản EARS tương ứng trong `SPEC.md`.
4. **Phân Loại Ký Hiệu**:
   - `[BE-CORE]`: Mã nguồn Backend (.NET 8 Clean Architecture) & Database PostgreSQL.
   - `[WEB-SHELL]`: Giao diện Frontend Web (React 18 + Vite + TypeScript).
   - `[MOB-SHELL]`: Ứng dụng Di động (Flutter 3.x + BLoC).
   - `[INTEG-TEST]`: Kiểm thử Tích hợp Toàn trình, An toàn Mật mã & Tuân thủ Spec.

---

## 2. Bảng Phân Rã Nhiệm Vụ Chi Tiết

### 2.1. Phân Hệ Backend Core (.NET 8 & PostgreSQL 16)

#### `[MOD2-T001]` [BE-CORE]: Khởi tạo Domain Entities, Enums, Value Objects & Domain Events cho Action Cards
- **Action**: Tạo các thực thể nghiệp vụ cốt lõi `ActionCard`, `ActionCardStep`, `ActionCardContact`, `ActionCardTemplate`, enum `UrgencyStage` (`IMMEDIATE`, `FIRST_72_HOURS`, `FIRST_7_DAYS`, `LONGER_TERM`), và các Domain Events tương ứng.
- **Files**:
  - `backend/src/Asseta.Domain/Entities/ActionCard.cs`
  - `backend/src/Asseta.Domain/Entities/ActionCardStep.cs`
  - `backend/src/Asseta.Domain/Entities/ActionCardContact.cs`
  - `backend/src/Asseta.Domain/Entities/ActionCardTemplate.cs`
  - `backend/src/Asseta.Domain/Enums/UrgencyStage.cs`
  - `backend/src/Asseta.Domain/Events/ActionCardEvents.cs`
- **EARS Ref**: `[REQ-CARD-001]`, `[REQ-CARD-002]`, `[REQ-CARD-003]`, `[REQ-CARD-004]`
- **Definition of Done (DoD)**: Lệnh `dotnet build backend/src/Asseta.Domain` hoàn thành không lỗi; Unit tests kiểm chứng tính bất biến của entities chạy xanh.

- [x] `[MOD2-T001]` [BE-CORE]: Khởi tạo Domain Entities, Enums, Value Objects & Domain Events cho Action Cards

#### `[MOD2-T002]` [BE-CORE]: Cấu hình EF Core 8 DbContext, Entity Configurations, Indexes & Seed Data Templates
- **Action**: Thiết lập cấu hình Fluent API cho 4 bảng `action_cards`, `action_card_steps`, `action_card_contacts`, `action_card_templates` kèm Concurrency Token `row_version`, quan hệ cascade delete, và nạp seed data 6 templates mẫu.
- **Files**:
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ActionCardConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ActionCardStepConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ActionCardContactConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/Configurations/ActionCardTemplateConfiguration.cs`
  - `backend/src/Asseta.Infrastructure/Persistence/AssetaDbContext.cs`
- **EARS Ref**: `[REQ-CARD-001]`, `[REQ-CARD-004]`, `[REQ-CARD-006]`, `[REQ-CARD-017]`
- **Definition of Done (DoD)**: `dotnet build backend/src/Asseta.Infrastructure` thành công; DbContext chứa đầy đủ 4 DbSets mới và ánh xạ bảng PostgreSQL chính xác.

- [x] `[MOD2-T002]` [BE-CORE]: Cấu hình EF Core 8 DbContext, Entity Configurations, Indexes & Seed Data Templates

#### `[MOD2-T003]` [BE-CORE]: Triển khai MediatR Commands & FluentValidation cho Quản Lý Action Card
- **Action**: Viết các Commands `CreateActionCardCommand`, `CreateActionCardFromItemCommand`, `UpdateActionCardCommand`, `DeleteActionCardCommand` kèm validators kiểm tra tiêu đề, danh mục, urgency stage và quét chuỗi nhạy cảm.
- **Files**:
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/CreateActionCard/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/CreateActionCardFromItem/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/UpdateActionCard/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/DeleteActionCard/`
- **EARS Ref**: `[REQ-CARD-005]`, `[REQ-CARD-007]`, `[REQ-CARD-008]`, `[REQ-CARD-013]`, `[REQ-CARD-014]`, `[REQ-CARD-015]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: tiêu đề rỗng hoặc sai urgency bị từ chối với 400; chuỗi thẻ tín dụng/private key unencrypted bị từ chối với 422; truy cập trái phép bị từ chối với 403.

- [x] `[MOD2-T003]` [BE-CORE]: Triển khai MediatR Commands & FluentValidation cho Quản Lý Action Card

#### `[MOD2-T004]` [BE-CORE]: Triển khai MediatR Commands & Logic Quản Lý Bước Hành Động (Action Steps) & Reorder
- **Action**: Viết các Commands `AddActionStepCommand`, `UpdateActionStepCommand`, `DeleteActionStepCommand`, `ReorderActionStepsCommand` đảm bảo thứ tự `step_order` bắt đầu từ 1 và liên tục, giới hạn tối đa 20 bước.
- **Files**:
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/AddActionStep/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/UpdateActionStep/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/DeleteActionStep/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/ReorderActionSteps/`
- **EARS Ref**: `[REQ-CARD-009]`, `[REQ-CARD-021]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: thêm bước thứ 21 bị từ chối với 400; reorder cập nhật thứ tự các bước chính xác theo danh sách ID truyền vào.

- [x] `[MOD2-T004]` [BE-CORE]: Triển khai MediatR Commands & Logic Quản Lý Bước Hành Động (Action Steps) & Reorder

#### `[MOD2-T005]` [BE-CORE]: Triển khai MediatR Commands & Logic Quản Lý Đầu Mối Liên Hệ (Key Contacts)
- **Action**: Viết các Commands `AddKeyContactCommand`, `DeleteKeyContactCommand` với giới hạn tối đa 5 đầu mối liên hệ trên một Action Card.
- **Files**:
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/AddKeyContact/`
  - `backend/src/Asseta.Application/Features/ActionCards/Commands/DeleteKeyContact/`
- **EARS Ref**: `[REQ-CARD-010]`, `[REQ-CARD-021]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: thêm contact thứ 6 bị từ chối với 400; xóa contact thành công.

- [x] `[MOD2-T005]` [BE-CORE]: Triển khai MediatR Commands & Logic Quản Lý Đầu Mối Liên Hệ (Key Contacts)

#### `[MOD2-T006]` [BE-CORE]: Triển khai Domain Service / Event Handler Đồng Bộ Trạng Thái Continuity Item & Giải Phóng Gap
- **Action**: Cài đặt Domain Event Handler `ActionCardCompletedEventHandler`: khi Action Card có đủ người phụ trách, vị trí tài liệu và ít nhất 1 bước hành động, tự động cập nhật `has_continuity_gap = false` trên `ContinuityItem` liên kết và cập nhật Readiness Score.
- **Files**:
  - `backend/src/Asseta.Application/Features/ActionCards/EventHandlers/ActionCardCompletedEventHandler.cs`
- **EARS Ref**: `[REQ-CARD-005]`, `[REQ-CARD-020]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: khi hoàn thành Action Card, cờ Gap trên Continuity Item liên kết chuyển thành false và Readiness Score danh mục tăng.

- [x] `[MOD2-T006]` [BE-CORE]: Triển khai Domain Service / Event Handler Đồng Bộ Trạng Thái Continuity Item & Giải Phóng Gap

#### `[MOD2-T007]` [BE-CORE]: Triển khai MediatR Queries & DTOs cho Action Cards và Templates
- **Action**: Viết các Queries `GetActionCardsQuery` (hỗ trợ lọc theo `urgency_stage`, `category_id`, tìm kiếm theo từ khóa và phân trang), `GetActionCardByIdQuery`, `GetActionCardTemplatesQuery`.
- **Files**:
  - `backend/src/Asseta.Application/Features/ActionCards/DTOs/`
  - `backend/src/Asseta.Application/Features/ActionCards/Queries/GetActionCards/`
  - `backend/src/Asseta.Application/Features/ActionCards/Queries/GetActionCardById/`
  - `backend/src/Asseta.Application/Features/ActionCards/Queries/GetActionCardTemplates/`
- **EARS Ref**: `[REQ-CARD-001]`, `[REQ-CARD-006]`, `[REQ-CARD-022]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: kết quả trả về đúng dữ liệu của user hiện tại, chỉ trả về items có `is_deleted = false`, lọc theo Urgency trả về đúng số lượng.

- [x] `[MOD2-T007]` [BE-CORE]: Triển khai MediatR Queries & DTOs cho Action Cards và Templates

#### `[MOD2-T008]` [BE-CORE]: Xây dựng Controllers & API Endpoints (`ActionCardsController`)
- **Action**: Cài đặt `ActionCardsController` với các endpoints CRUD thẻ, quản lý steps, contacts và templates theo chuẩn Envelope phản hồi.
- **Files**:
  - `backend/src/Asseta.Api/Controllers/ActionCardsController.cs`
- **EARS Ref**: Toàn bộ EARS Spec
- **Definition of Done (DoD)**: `dotnet build backend/src/Asseta.Api` thành công; Swagger UI hiển thị đầy đủ danh sách 12 endpoints với XML docs.

- [x] `[MOD2-T008]` [BE-CORE]: Xây dựng Controllers & API Endpoints (`ActionCardsController`)

#### `[MOD2-T009]` [BE-CORE]: Viết Unit Tests & Integration Tests Backend Toàn Diện cho Module 2
- **Action**: Viết bộ unit tests cho Commands/Queries/Entities và Integration Tests cho toàn bộ các API endpoints qua `CustomWebApplicationFactory`.
- **Files**:
  - `backend/tests/Asseta.UnitTests/Application/ActionCards/`
  - `backend/tests/Asseta.IntegrationTests/ActionCardApiTests.cs`
- **EARS Ref**: Toàn bộ EARS Spec
- **Definition of Done (DoD)**: Lệnh `dotnet test backend/Asseta.sln` chạy xanh 100% toàn bộ tests của Module 1 và Module 2.

- [x] `[MOD2-T009]` [BE-CORE]: Viết Unit Tests & Integration Tests Backend Toàn Diện cho Module 2

---

### 2.2. Phân Hệ Frontend Web (React 18 + Vite + TypeScript)

#### `[MOD2-T010]` [WEB-SHELL]: Định nghĩa TypeScript Types, Interfaces & API Client cho Action Cards
- **Action**: Khởi tạo các interface `ActionCard`, `ActionCardStep`, `ActionCardContact`, `ActionCardTemplate`, enum `UrgencyStage` và Axios client `actionCardApi.ts` tự động gắn `Idempotency-Key` và `X-Correlation-Id`.
- **Files**:
  - `frontend-web/src/types/actionCard.ts`
  - `frontend-web/src/features/ActionCard/api/actionCardApi.ts`
- **EARS Ref**: `[REQ-CARD-001]`, `[REQ-CARD-016]`
- **Definition of Done (DoD)**: Lệnh `cmd /c "npm run build"` và `cmd /c "npx vitest run"` không phát sinh lỗi biên dịch TypeScript.

- [x] `[MOD2-T010]` [WEB-SHELL]: Định nghĩa TypeScript Types, Interfaces & API Client cho Action Cards

#### `[MOD2-T011]` [WEB-SHELL]: Xây dựng Custom Hooks Quản Lý State & Server Cache (`useActionCards`)
- **Action**: Cài đặt các hooks `useActionCards` (hỗ trợ lọc theo `urgencyStage`, `categoryId`, search), `useActionCardDetail`, `useActionCardTemplates` và các mutations thêm/sửa/xóa thẻ.
- **Files**:
  - `frontend-web/src/features/ActionCard/hooks/useActionCards.ts`
- **EARS Ref**: `[REQ-CARD-012]`, `[REQ-CARD-022]`
- **Definition of Done (DoD)**: Vitest unit test kiểm chứng: hook quản lý state, cập nhật cache khi mutation thành công.

- [x] `[MOD2-T011]` [WEB-SHELL]: Xây dựng Custom Hooks Quản Lý State & Server Cache (`useActionCards`)

#### `[MOD2-T012]` [WEB-SHELL]: Xây dựng Component Timeline / Kanban View (`ActionCardTimelineView.tsx`)
- **Action**: Thiết kế giao diện chia 4 cột/timeline theo 4 giai đoạn khẩn cấp (`IMMEDIATE`, `FIRST_72_HOURS`, `FIRST_7_DAYS`, `LONGER_TERM`), hiển thị badge mức ưu tiên và tiến độ checklist các bước.
- **Files**:
  - `frontend-web/src/features/ActionCard/components/ActionCardTimelineView.tsx`
  - `frontend-web/src/features/ActionCard/components/ActionCardItemCard.tsx`
- **EARS Ref**: `[REQ-CARD-011]`, `[REQ-CARD-022]`
- **Definition of Done (DoD)**: Vitest test kiểm chứng render phân loại đúng 4 cột, click thẻ kích hoạt xem chi tiết.

- [x] `[MOD2-T012]` [WEB-SHELL]: Xây dựng Component Timeline / Kanban View (`ActionCardTimelineView.tsx`)

#### `[MOD2-T013]` [WEB-SHELL]: Xây dựng Component Quản Lý Checklist Các Bước Hành Động (`StepChecklistEditor.tsx`)
- **Action**: Thiết kế component checklist cho phép đánh dấu hoàn tất (`toggle`), thêm bước mới, xóa bước, và kéo thả/bấm nút di chuyển đổi thứ tự `step_order`.
- **Files**:
  - `frontend-web/src/features/ActionCard/components/StepChecklistEditor.tsx`
- **EARS Ref**: `[REQ-CARD-009]`, `[REQ-CARD-021]`
- **Definition of Done (DoD)**: Vitest test kiểm chứng thêm bước mới hiển thị đúng danh sách, tương tác checkbox kích hoạt API.

- [x] `[MOD2-T013]` [WEB-SHELL]: Xây dựng Component Quản Lý Checklist Các Bước Hành Động (`StepChecklistEditor.tsx`)

#### `[MOD2-T014]` [WEB-SHELL]: Xây dựng Modal Chi Tiết & Giải Mã Chỉ Dẫn Mật Mã (`ActionCardDetailModal.tsx`)
- **Action**: Thiết kế modal xem toàn bộ thông tin thẻ, danh sách bước hành động, danh bạ đầu mối liên hệ (có nút gọi/mail) và nút giải mã chỉ dẫn bí mật với Web Crypto API.
- **Files**:
  - `frontend-web/src/features/ActionCard/components/ActionCardDetailModal.tsx`
- **EARS Ref**: `[REQ-CARD-010]`, `[REQ-CARD-018]`
- **Definition of Done (DoD)**: Vitest test kiểm chứng giải mã thành công chuỗi mật mã tiếng Việt khi nhập đúng Master Key.

- [x] `[MOD2-T014]` [WEB-SHELL]: Xây dựng Modal Chi Tiết & Giải Mã Chỉ Dẫn Mật Mã (`ActionCardDetailModal.tsx`)

#### `[MOD2-T015]` [WEB-SHELL]: Xây dựng Modal Tạo / Sửa Thẻ Hành Động & Chọn Template (`ActionCardFormModal.tsx` & `TemplateSelectorModal.tsx`)
- **Action**: Xây dựng form tạo thẻ cho phép áp dụng Template mẫu (tự động điền các bước gợi ý) hoặc tạo từ `ContinuityItem`, tích hợp mã hóa tự động trường chỉ dẫn trước khi gửi.
- **Files**:
  - `frontend-web/src/features/ActionCard/components/ActionCardFormModal.tsx`
  - `frontend-web/src/features/ActionCard/components/TemplateSelectorModal.tsx`
  - `frontend-web/src/features/ActionCard/ActionCardList.tsx`
- **EARS Ref**: `[REQ-CARD-005]`, `[REQ-CARD-006]`, `[REQ-CARD-014]`, `[REQ-CARD-018]`
- **Definition of Done (DoD)**: Vitest test kiểm chứng: chọn template điền đúng checklist, submit form gửi payload đã mã hóa qua Web Crypto API.

- [x] `[MOD2-T015]` [WEB-SHELL]: Xây dựng Modal Tạo / Sửa Thẻ Hành Động & Chọn Template (`ActionCardFormModal.tsx` & `TemplateSelectorModal.tsx`)

---

### 2.3. Phân Hệ Mobile App (Flutter 3.x + BLoC)

#### `[MOD2-T016]` [MOB-SHELL]: Xây dựng Domain Entities, Repositories & Local Cache cho Action Cards
- **Action**: Tạo các thực thể `ActionCardEntity`, `ActionStepEntity`, `ActionContactEntity`, `ActionTemplateEntity`, interface `ActionCardRepository` và `ActionCardLocalDataSource` hỗ trợ offline cache.
- **Files**:
  - `mobile-app/lib/features/action_cards/domain/entities/`
  - `mobile-app/lib/features/action_cards/domain/repositories/action_card_repository.dart`
  - `mobile-app/lib/features/action_cards/data/datasources/action_card_local_datasource.dart`
- **EARS Ref**: `[REQ-CARD-001]`, `[REQ-CARD-012]`
- **Definition of Done (DoD)**: Lệnh `flutter test` kiểm chứng serialization JSON sang Entity và lưu/đọc local cache thành công.

- [x] `[MOD2-T016]` [MOB-SHELL]: Xây dựng Domain Entities, Repositories & Local Cache cho Action Cards

#### `[MOD2-T017]` [MOB-SHELL]: Xây dựng Dio Remote DataSource & Interceptor Idempotency cho Action Cards
- **Action**: Cài đặt `ActionCardRemoteDataSourceImpl` sử dụng Dio client có interceptor tự động gắn `Idempotency-Key` UUIDv4 cho các mutations.
- **Files**:
  - `mobile-app/lib/features/action_cards/data/datasources/action_card_remote_datasource.dart`
  - `mobile-app/lib/features/action_cards/data/repositories/action_card_repository_impl.dart`
- **EARS Ref**: `[REQ-CARD-016]`
- **Definition of Done (DoD)**: Unit test kiểm chứng: request gửi lên có Header `Idempotency-Key` và parse đúng envelope response.

- [x] `[MOD2-T017]` [MOB-SHELL]: Xây dựng Dio Remote DataSource & Interceptor Idempotency cho Action Cards

#### `[MOD2-T018]` [MOB-SHELL]: Xây dựng ActionCardBloc (Events, States, Handlers)
- **Action**: Triển khai BLoC quản lý trạng thái tải, chuyển đổi tab Urgency, thêm/sửa thẻ, toggle trạng thái bước hành động và reorder steps.
- **Files**:
  - `mobile-app/lib/features/action_cards/presentation/bloc/action_card_bloc.dart`
  - `mobile-app/lib/features/action_cards/presentation/bloc/action_card_event.dart`
  - `mobile-app/lib/features/action_cards/presentation/bloc/action_card_state.dart`
  - `mobile-app/test/features/action_cards/presentation/bloc/action_card_bloc_test.dart`
- **EARS Ref**: `[REQ-CARD-009]`, `[REQ-CARD-012]`
- **Definition of Done (DoD)**: Lệnh `flutter test test/features/action_cards/presentation/bloc/` chạy xanh toàn bộ các kịch bản state transition.

- [x] `[MOD2-T018]` [MOB-SHELL]: Xây dựng ActionCardBloc (Events, States, Handlers)

#### `[MOD2-T019]` [MOB-SHELL]: Xây dựng Màn Hình Danh Sách Action Cards (`ActionCardsPage.dart`)
- **Action**: Thiết kế giao diện danh sách thẻ với Segmented Control lọc 4 khung thời gian, hiển thị card preview số bước checklist, pull-to-refresh và offline fallback.
- **Files**:
  - `mobile-app/lib/features/action_cards/presentation/pages/action_cards_page.dart`
  - `mobile-app/lib/features/action_cards/presentation/widgets/urgency_stage_badge.dart`
- **EARS Ref**: `[REQ-CARD-011]`, `[REQ-CARD-012]`
- **Definition of Done (DoD)**: Widget test verify render 4 tab thời gian, chuyển tab lọc đúng danh sách thẻ tương ứng.

- [x] `[MOD2-T019]` [MOB-SHELL]: Xây dựng Màn Hình Danh Sách Action Cards (`ActionCardsPage.dart`)

#### `[MOD2-T020]` [MOB-SHELL]: Xây dựng Màn Hình Chi Tiết Thẻ & Danh Sách Các Bước Kéo Thả (`ActionCardDetailPage.dart` & `ReorderableStepList.dart`)
- **Action**: Thiết kế màn hình chi tiết thẻ với danh sách checkbox các bước có thể kéo thả đổi thứ tự (`ReorderableListView`), thẻ đầu mối liên hệ có nút gọi điện nhanh `url_launcher`.
- **Files**:
  - `mobile-app/lib/features/action_cards/presentation/pages/action_card_detail_page.dart`
  - `mobile-app/lib/features/action_cards/presentation/widgets/reorderable_step_list.dart`
  - `mobile-app/lib/features/action_cards/presentation/widgets/contact_card_tile.dart`
- **EARS Ref**: `[REQ-CARD-009]`, `[REQ-CARD-010]`
- **Definition of Done (DoD)**: Widget test kiểm tra thao tác kéo thả đổi thứ tự bước phát sinh event `ReorderStepsEvent`.

- [x] `[MOD2-T020]` [MOB-SHELL]: Xây dựng Màn Hình Chi Tiết Thẻ & Danh Sách Các Bước Kéo Thả (`ActionCardDetailPage.dart` & `ReorderableStepList.dart`)

#### `[MOD2-T021]` [MOB-SHELL]: Xây dựng Màn Hình Tạo / Sửa Thẻ Hành Động & Tải Template (`ActionCardFormPage.dart`)
- **Action**: Xây dựng form nhập liệu hỗ trợ chọn template mẫu, thêm các bước động, tích hợp mã hóa Client-Side bằng Dart `cryptography` trước khi dispatch event.
- **Files**:
  - `mobile-app/lib/features/action_cards/presentation/pages/action_card_form_page.dart`
- **EARS Ref**: `[REQ-CARD-006]`, `[REQ-CARD-018]`
- **Definition of Done (DoD)**: Widget test kiểm tra điền form, chọn template và mã hóa trường chỉ dẫn gửi lên BLoC state.

- [x] `[MOD2-T021]` [MOB-SHELL]: Xây dựng Màn Hình Tạo / Sửa Thẻ Hành Động & Tải Template (`ActionCardFormPage.dart`)

#### `[MOD2-T022]` [MOB-SHELL]: Viết Flutter Widget & Golden Tests Toàn Diện cho Module 2
- **Action**: Viết widget tests kiểm chứng giao diện hoạt động chính xác ở các trạng thái Loading, Loaded, Error và Offline Mode.
- **Files**:
  - `mobile-app/test/features/action_cards/presentation/action_cards_page_test.dart`
- **EARS Ref**: Toàn bộ EARS Spec
- **Definition of Done (DoD)**: Lệnh `flutter test` toàn bộ module Action Cards chạy xanh 100%.

- [x] `[MOD2-T022]` [MOB-SHELL]: Viết Flutter Widget & Golden Tests Toàn Diện cho Module 2

---

### 2.4. Phân Hệ Kiểm Thử Toàn Trình & Thẩm Định Tuân Thủ

#### `[MOD2-T023]` [INTEG-TEST]: Kiểm Thử Toàn Trình Xác Thực Zero-Knowledge & An Toàn Mật Mã
- **Action**: Viết kiểm thử E2E: Client mã hóa chỉ dẫn bí mật $\rightarrow$ gửi qua API $\rightarrow$ kiểm tra trực tiếp bảng `action_cards` trong database PostgreSQL chứng minh chỉ lưu `CipherBlob`, hoàn toàn không có plaintext hoặc khóa giải mã trên máy chủ; kiểm tra gửi thẻ tín dụng/private key unencrypted trả về HTTP 422.
- **Files**:
  - `backend/tests/Asseta.IntegrationTests/ActionCardZeroKnowledgeTests.cs`
- **EARS Ref**: `[REQ-CARD-002]`, `[REQ-CARD-014]`, `[REQ-CARD-018]`
- **Definition of Done (DoD)**: Test tự động chạy và xác nhận 100% bản ghi trong DB không chứa chuỗi văn bản gốc; Server không thể giải mã nếu không có Master Key từ client.

- [x] `[MOD2-T023]` [INTEG-TEST]: Kiểm Thử Toàn Trình Xác Thực Zero-Knowledge & An Toàn Mật Mã

#### `[MOD2-T024]` [INTEG-TEST]: Kiểm Thử Độ Bền Idempotency & Xung Đột Đồng Thời (Concurrency Conflict)
- **Action**: Viết kiểm thử tích hợp giả lập: gửi trùng `Idempotency-Key` khi mạng chập chờn (xác nhận chỉ tạo 1 bản ghi và trả kết quả cache), và 2 client cùng sửa 1 thẻ (`row_version` test, xác nhận client thứ 2 nhận mã 409 Conflict).
- **Files**:
  - `backend/tests/Asseta.IntegrationTests/ActionCardResilienceTests.cs`
- **EARS Ref**: `[REQ-CARD-016]`, `[REQ-CARD-017]`
- **Definition of Done (DoD)**: Test chạy xanh, chứng minh tính toàn vẹn dữ liệu khi có sự cố mạng hoặc thao tác đồng thời.

- [x] `[MOD2-T024]` [INTEG-TEST]: Kiểm Thử Độ Bền Idempotency & Xung Đột Đồng Thời (Concurrency Conflict)

#### `[MOD2-T025]` [INTEG-TEST]: Lập Bảng Ma Trận Truy Xuất Yêu Cầu (Traceability Matrix Audit cho Module 2)
- **Action**: Rà soát đối chiếu 100% các yêu cầu EARS từ `[REQ-CARD-001]` đến `[REQ-CARD-022]`, lập bảng ma trận liên kết giữa Spec ID, file code triển khai và unit test tương ứng trong tài liệu `VALIDATION.md`.
- **Files**:
  - `.sdd/specs/module2/VALIDATION.md`
- **EARS Ref**: Toàn bộ EARS Spec
- **Definition of Done (DoD)**: 100% điều khoản SHALL có unit test chứng minh; không phát sinh bất kỳ tính năng nào ngoài phạm vi (Zero Feature Creep).

- [x] `[MOD2-T025]` [INTEG-TEST]: Lập Bảng Ma Trận Truy Xuất Yêu Cầu (Traceability Matrix Audit cho Module 2)

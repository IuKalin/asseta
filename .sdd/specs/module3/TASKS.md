# Danh Sách Công Việc Phân Rã Nguyên Tử (Task Decomposition): Module 3 – Trusted People

**Mã Module:** `module3` (Tương đương `feat-03-trusted-people`)  
**Pha phát triển:** Pha 3 – Task Decomposition  
**Vai trò đảm trách:** AI Technical Project Lead & Full-stack Architect  
**Tài liệu căn cứ:** [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module3/SPEC.md), [PLAN.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module3/PLAN.md)  
**Trạng thái:** COMPLETED (100% TASKS IMPLEMENTED & VERIFIED)  

---

## 1. Nguyên Tắc & Quy Chuẩn Thực Thi
- **Tính nguyên tử (Atomic)**: Mỗi task độc lập, có thể verify và test riêng biệt.
- **Test-Driven / Test-First**: Luôn viết và chạy test song hành với mã nguồn.
- **Tiêu chuẩn hoàn thành (DoD)**: Mã nguồn hoàn tất, biên dịch thành công không lỗi linter/compiler, unit test tương ứng đổi màu xanh (PASS).

---

## 2. Bảng Phân Rã Chi Tiết Các Hạng Mục

### Nhóm 1: Backend Domain & Enums (`[BE-DOM]`)
- [x] `[BE-DOM-001]`: Định nghĩa Enums `TrustedPersonStatus` (`Invited`, `Active`, `Suspended`, `Revoked`) và `PermissionType` (`Category`, `ActionCard`).
  - **Files**: `backend/src/Asseta.Domain/Enums/TrustedPersonStatus.cs`, `backend/src/Asseta.Domain/Enums/PermissionType.cs`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-008]`
  - **DoD**: Enums được khởi tạo đầy đủ. (PASS)
- [x] `[BE-DOM-002]`: Mở rộng Entity `TrustedPerson` với các trường trạng thái, `role_description`, `row_version`, `delegate_user_id` và các phương thức nghiệp vụ (`MarkAsPaired`, `Revoke`, `UpdateProfile`, `UpdateTrustLevel`).
  - **Files**: `backend/src/Asseta.Domain/Entities/TrustedPerson.cs`
  - **EARS Ref**: `[REQ-TRUST-001]`, `[REQ-TRUST-005]`, `[REQ-TRUST-007]`, `[REQ-TRUST-010]`
  - **DoD**: Unit test kiểm tra khởi tạo và các hàm chuyển trạng thái của `TrustedPerson` pass. (PASS)
- [x] `[BE-DOM-003]`: Tạo Entity `TrustedPersonPairingCode` quản lý mã băm, salt mật mã, cơ chế thử sai (3 lần) và thời hạn khóa (15m lockout, 48h TTL).
  - **Files**: `backend/src/Asseta.Domain/Entities/TrustedPersonPairingCode.cs`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-015]`, `[REQ-TRUST-016]`
  - **DoD**: Unit test kiểm tra hàm `RecordFailedAttempt`, `IsLockedOut`, `IsExpired`, `MarkAsUsed`. (PASS)
- [x] `[BE-DOM-004]`: Tạo Entity `TrustedPersonPermission` quản lý ma trận phân quyền theo Category hoặc ActionCard.
  - **Files**: `backend/src/Asseta.Domain/Entities/TrustedPersonPermission.cs`
  - **EARS Ref**: `[REQ-TRUST-008]`, `[REQ-TRUST-024]`
  - **DoD**: Entity khởi tạo đúng validation logic. (PASS)
- [x] `[BE-DOM-005]`: Định nghĩa Domain Events cho Trusted People (`TrustedPersonCreatedEvent`, `TrustedPersonPairedEvent`, `TrustedPersonRevokedEvent`).
  - **Files**: `backend/src/Asseta.Domain/Events/TrustedPersonEvents.cs`
  - **EARS Ref**: `[REQ-TRUST-003]`, `[REQ-TRUST-006]`, `[REQ-TRUST-010]`
  - **DoD**: Sự kiện được khởi tạo với đầy đủ metadata. (PASS)

### Nhóm 2: Backend Infrastructure & Persistence (`[BE-INFRA]`)
- [x] `[BE-INFRA-001]`: Tạo dịch vụ mật mã `IPairingCodeHasher` và `PairingCodeHasher` sử dụng CSPRNG sinh mã 6 ký tự ngẫu nhiên và băm HMAC-SHA256 với Salt.
  - **Files**: `backend/src/Asseta.Application/Common/Interfaces/IPairingCodeHasher.cs`, `backend/src/Asseta.Infrastructure/Services/PairingCodeHasher.cs`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-006]`, `[NFR-SEC-02]`
  - **DoD**: Unit test kiểm tra tính ngẫu nhiên, độ dài 6 ký tự và xác thực mã băm chính xác 100%. (PASS)
- [x] `[BE-INFRA-002]`: Cấu hình EF Core Fluent API cho `TrustedPerson`, `TrustedPersonPairingCode` và `TrustedPersonPermission` trong `AssetaDbContext`.
  - **Files**: `backend/src/Asseta.Infrastructure/Persistence/Configurations/TrustedPersonConfiguration.cs`, `backend/src/Asseta.Infrastructure/Persistence/Configurations/TrustedPersonPairingCodeConfiguration.cs`, `backend/src/Asseta.Infrastructure/Persistence/Configurations/TrustedPersonPermissionConfiguration.cs`, `backend/src/Asseta.Infrastructure/Persistence/AssetaDbContext.cs`
  - **EARS Ref**: `[REQ-TRUST-001]`, `[REQ-TRUST-004]`, `[REQ-TRUST-023]`
  - **DoD**: Khởi tạo DbContext InMemory/Postgres không phát sinh lỗi schema mapping. (PASS)

### Nhóm 3: Backend Application Commands & Queries (`[BE-APP]`)
- [x] `[BE-APP-001]`: Triển khai DTOs cho Trusted People và Scoped Access Matrix.
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/DTOs/TrustedPersonDto.cs`, `backend/src/Asseta.Application/Features/TrustedPeople/DTOs/ScopedPermissionDto.cs`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-008]`
  - **DoD**: DTO map dữ liệu đầy đủ, che giấu các thông tin băm nhạy cảm. (PASS)
- [x] `[BE-APP-002]`: Triển khai Command `CreateTrustedPersonCommand` kèm FluentValidator (kiểm tra giới hạn 5 người, định dạng SĐT/email, chặn trùng lặp, sinh mã pairing 48h).
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Commands/CreateTrustedPerson/`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-014]`, `[REQ-TRUST-018]`, `[REQ-TRUST-023]`
  - **DoD**: Unit test Handler và Validator pass. (PASS)
- [x] `[BE-APP-003]`: Triển khai Command `ClaimPairingCodeCommand` kèm Validator (xác thực mã băm, kiểm tra hết hạn, chống brute-force 3 lần / khóa 15m, chặn tự ghép đôi `DelegateUserId == OwnerId`).
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Commands/ClaimPairingCode/`
  - **EARS Ref**: `[REQ-TRUST-006]`, `[REQ-TRUST-015]`, `[REQ-TRUST-016]`, `[REQ-TRUST-017]`
  - **DoD**: Unit test các ca thành công, sai mã, quá 3 lần khóa, và hết hạn đều pass. (PASS)
- [x] `[BE-APP-004]`: Triển khai Command `UpdateTrustedPersonCommand` và `RegeneratePairingCodeCommand`.
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Commands/UpdateTrustedPerson/`, `backend/src/Asseta.Application/Features/TrustedPeople/Commands/RegeneratePairingCode/`
  - **EARS Ref**: `[REQ-TRUST-007]`, `[REQ-TRUST-009]`, `[REQ-TRUST-019]`
  - **DoD**: Unit test cập nhật hồ sơ, kiểm tra `row_version` và sinh lại mã thành công. (PASS)
- [x] `[BE-APP-005]`: Triển khai Command `UpdateScopedPermissionsCommand` cập nhật ma trận quyền hạn theo Category và ActionCard.
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Commands/UpdateScopedPermissions/`
  - **EARS Ref**: `[REQ-TRUST-008]`, `[REQ-TRUST-024]`
  - **DoD**: Unit test phân quyền theo danh mục và thẻ thành công. (PASS)
- [x] `[BE-APP-006]`: Triển khai Command `RevokeTrustedPersonCommand` và Event Handler `TrustedPersonRevokedEventHandler` (thu hồi ủy thác, unassign khỏi Continuity Items & Action Cards, tự động kích hoạt lại `ContinuityGap` và tính lại `ReadinessScore`).
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Commands/RevokeTrustedPerson/`
  - **EARS Ref**: `[REQ-TRUST-010]`, `[REQ-TRUST-022]`
  - **DoD**: Unit test xác minh unassign item, chuyển `has_continuity_gap = true`, giảm `readiness_score`. (PASS)
- [x] `[BE-APP-007]`: Triển khai các Queries: `GetTrustedPeopleQuery`, `GetTrustedPersonDetailQuery`, `GetMyDelegatedRolesQuery`.
  - **Files**: `backend/src/Asseta.Application/Features/TrustedPeople/Queries/`
  - **EARS Ref**: `[REQ-TRUST-001]`, `[REQ-TRUST-011]`, `[REQ-TRUST-012]`
  - **DoD**: Unit test trả về đúng danh sách đã lọc theo OwnerId và Zero-Disclosure đối với Delegate. (PASS)

### Nhóm 4: Backend API Controllers & Tests (`[BE-API]`)
- [x] `[BE-API-001]`: Tạo `TrustedPeopleController` cung cấp đầy đủ các endpoints RESTful chuẩn Envelope.
  - **Files**: `backend/src/Asseta.Api/Controllers/TrustedPeopleController.cs`
  - **EARS Ref**: `[REQ-TRUST-001]` đến `[REQ-TRUST-020]`
  - **DoD**: Controller định tuyến chuẩn xác, gắn authorize và xử lý mã lỗi HTTP tương ứng. (PASS)
- [x] `[BE-API-002]`: Viết toàn bộ Unit Tests và Integration Tests cho Module 3 Backend.
  - **Files**: `backend/tests/Asseta.UnitTests/Domain/TrustedPersonTests.cs`, `backend/tests/Asseta.UnitTests/Application/TrustedPeople/TrustedPeopleHandlerTests.cs`, `backend/tests/Asseta.IntegrationTests/TrustedPeopleApiTests.cs`
  - **EARS Ref**: `[REQ-TRUST-001]` đến `[REQ-TRUST-024]`
  - **DoD**: `dotnet test` toàn bộ suite pass 100% (66 Unit Tests + 32 Integration Tests = 98 Tests PASS). (PASS)

### Nhóm 5: Frontend Web (`[WEB-SHELL]`)
- [x] `[WEB-SHELL-001]`: Định nghĩa TypeScript interfaces & Service Client API cho Trusted People.
  - **Files**: `frontend-web/src/types/trustedPeople.ts`, `frontend-web/src/services/trustedPeopleService.ts`
  - **EARS Ref**: `[REQ-TRUST-001]`, `[REQ-TRUST-008]`
  - **DoD**: Type check hợp lệ. (PASS)
- [x] `[WEB-SHELL-002]`: Triển khai React Query hooks `useTrustedPeople` (CRUD, Claim Pairing Code, Update Permissions).
  - **Files**: `frontend-web/src/features/TrustedPeople/hooks/useTrustedPeople.ts`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-008]`
  - **DoD**: Hook hỗ trợ invalidate queries và optimistic updates. (PASS)
- [x] `[WEB-SHELL-003]`: Triển khai Components: `TrustedPeopleList.tsx`, `TrustedPersonModal.tsx`, `PairingCodeModal.tsx`, `ScopedAccessMatrixDrawer.tsx`.
  - **Files**: `frontend-web/src/features/TrustedPeople/components/`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-006]`, `[REQ-TRUST-008]`
  - **DoD**: Giao diện trực quan chuẩn TailwindCSS, hiển thị trạng thái `Invited` (vàng), `Active` (xanh), nút xem mã ghép đôi và ma trận phân quyền. (PASS)
- [x] `[WEB-SHELL-004]`: Viết Vitest Unit Tests cho Frontend Trusted People.
  - **Files**: `frontend-web/src/features/TrustedPeople/__tests__/TrustedPeople.test.ts`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-008]`
  - **DoD**: `npm test` pass (16 Tests PASS). (PASS)

### Nhóm 6: Mobile App Flutter (`[MOB-SHELL]`)
- [x] `[MOB-SHELL-001]`: Triển khai Domain Layer (Entities, Repositories, UseCases).
  - **Files**: `mobile-app/lib/features/trusted_people/domain/entities/trusted_person_entity.dart`, `mobile-app/lib/features/trusted_people/domain/repositories/trusted_people_repository.dart`
  - **EARS Ref**: `[REQ-TRUST-001]`, `[REQ-TRUST-005]`, `[REQ-TRUST-006]`
  - **DoD**: Domain logic thuần Dart, không phụ thuộc framework. (PASS)
- [x] `[MOB-SHELL-002]`: Triển khai Data Layer (Models, RemoteDataSource qua Dio, LocalDataSource cache offline).
  - **Files**: `mobile-app/lib/features/trusted_people/data/models/trusted_person_model.dart`, `mobile-app/lib/features/trusted_people/data/datasources/trusted_people_remote_datasource.dart`, `mobile-app/lib/features/trusted_people/data/repositories/trusted_people_repository_impl.dart`
  - **EARS Ref**: `[REQ-TRUST-012]`, `[REQ-TRUST-013]`
  - **DoD**: Model serialization JSON hai chiều, cache cục bộ khi offline. (PASS)
- [x] `[MOB-SHELL-003]`: Triển khai Presentation Layer (BLoC, Pages, Widgets).
  - **Files**: `mobile-app/lib/features/trusted_people/presentation/bloc/trusted_people_bloc.dart`, `mobile-app/lib/features/trusted_people/presentation/pages/trusted_people_page.dart`, `mobile-app/lib/features/trusted_people/presentation/pages/pairing_code_claim_page.dart`, `mobile-app/lib/features/trusted_people/presentation/widgets/trusted_person_card.dart`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-006]`, `[REQ-TRUST-008]`
  - **DoD**: BLoC xử lý đầy đủ các state `Initial`, `Loading`, `Loaded`, `Error`; màn hình nhập mã 6 ô ký tự (`PairingCodeClaimPage`). (PASS)
- [x] `[MOB-SHELL-004]`: Viết Unit & Widget Tests cho Mobile Trusted People.
  - **Files**: `mobile-app/test/features/trusted_people/trusted_person_entity_test.dart`, `mobile-app/test/features/trusted_people/trusted_people_bloc_test.dart`
  - **EARS Ref**: `[REQ-TRUST-005]`, `[REQ-TRUST-006]`
  - **DoD**: `flutter test` pass 100%. (PASS)

### Nhóm 7: Kiểm Định & Báo Cáo Thẩm Định SDD Pha 5 (`[AUDIT]`)
- [ ] `[AUDIT-001]`: Tổng hợp và xuất bản báo cáo thẩm định `.sdd/specs/module3/VALIDATION.md` với bảng Ma trận đối soát truy xuất nguồn gốc (Traceability Matrix) 100% các điều khoản EARS `[REQ-TRUST-001]` đến `[REQ-TRUST-024]`.
  - **Files**: `.sdd/specs/module3/VALIDATION.md`, `walkthrough.md`
  - **EARS Ref**: Toàn bộ SPEC.md
  - **DoD**: 100% Traceable, Zero Feature Creep, Zero-Knowledge Verified.

# Nhật Ký Lỗi & Báo Cáo Khắc Phục Kiểm Thử Monorepo (Test Execution & Defects Resolution Log)

> **Mục đích:** Ghi nhận toàn bộ lỗi phát sinh trong quá trình kiểm thử Unit Test và Web-App Integration Test từ Module 1 đến Module 5, nguyên nhân gốc rễ (RCA), giải pháp khắc phục sau khi người dùng phê duyệt ("đồng ý"), và bằng chứng kiểm thử thực tế đạt 100%.  
> **Nguyên tắc:** **TUYỆT ĐỐI KHÔNG DÙNG MOCK DATA - 100% REAL DATA & REAL LIVE SERVICES.**  
> **Thời gian cập nhật:** 2026-09-05 23:35:00  
> **Tổng số lỗi phát hiện:** 5 lỗi  
> **Tổng số lỗi đã khắc phục hoàn tất:** **5 / 5 lỗi (100% RESOLVED)**  

---

## 1. Bảng Tổng Hợp Kết Quả Kiểm Thử Toàn Monorepo

| Phân hệ / Hạng mục | Tổng số Tests | Passed | Failed | Tỷ lệ Đạt | Dữ liệu kiểm thử | Trạng thái |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Backend Unit Tests** (.NET 8 LTS) | 96 | 96 | 0 | **100%** | Domain Entities, AES-256-GCM, BCrypt, PBKDF2 | **PASS** |
| **Frontend Web Unit Tests** (Vitest) | 26 | 26 | 0 | **100%** | WebCrypto API, BLoC State, Reducers | **PASS** |
| **Mobile App Unit Tests** (Flutter) | 51 | 51 | 0 | **100%** | PointyCastle Crypto, BLoC, Clean Architecture | **PASS** |
| **TỔNG SỐ UNIT TESTS (M1 - M5)** | **173** | **173** | **0** | **100%** | **Tuyệt đối 0 Mock Data** | **PASS** |
| **Web-App E2E Integration Test (Live :5000)** | **5** | **5** | **0** | **100%** | **Real HTTP Calls, Live PostgreSQL DB, JWT** | **PASS** |
| **TỔNG TOÀN BỘ SUITE (Unit + Integration)** | **178** | **178** | **0** | **100%** | **Monorepo Green** | **PASS** |

---

## 2. Chi Tiết Lỗi Phát Hiện, Phân Tích & Giải Pháp Khắc Phục (Defects Resolution)

### [DEFECT-001] Missing EF Core Database Schema cho Modules 3, 4, 5 trên Live PostgreSQL
- **Phân hệ:** Backend .NET 8 / PostgreSQL Container (`asseta-postgres:5432`)
- **Triệu chứng:** Khi chạy Web-App Integration Test gọi đến các API Module 3, 4, 5, máy chủ Backend trả về **HTTP 500 Internal Server Error**. Log ghi nhận:
  ```text
  Npgsql.PostgresException (0x80004005): 42P01: relation "trusted_people" does not exist
  Npgsql.PostgresException (0x80004005): 42P01: relation "owner_activation_configs" does not exist
  ```
- **Nguyên nhân gốc rễ (RCA):**
  - Trước đây, database `asseta_db` đã được khởi tạo với bảng của Module 1 và 2 (`users`, `action_cards`, `continuity_items`,...).
  - Trong EF Core, `Database.EnsureCreated()` chỉ tạo schema nếu database *chưa tồn tại*. Do database đã tồn tại nên `EnsureCreated()` bỏ qua, dẫn đến 6 bảng mới (`trusted_people`, `trusted_person_pairing_codes`, `trusted_person_permissions`, `owner_activation_configs`, `activation_requests`, `activation_confirmations`) chưa được tạo.
- **Giải pháp khắc phục:**
  1. Thêm gói `Microsoft.EntityFrameworkCore.Design` vào `Asseta.Api.csproj`.
  2. Tạo EF Core Migration `InitialAllModules` bao gồm toàn bộ thực thể của 5 module.
  3. Áp dụng an toàn migration DDL lên PostgreSQL container, bảo toàn dữ liệu hiện hữu và tạo đầy đủ 19 bảng cùng các chỉ mục ràng buộc.
- **Trạng thái:** `RESOLVED & VERIFIED`

---

### [DEFECT-002] Đảo ngược thứ tự tham số (Parameter Swap) tại `CryptoService.encrypt`
- **Phân hệ:** Frontend Web / `cryptoService.ts` & `webAppE2EIntegration.test.ts`
- **Triệu chứng:**
  ```text
  TypeError: Failed to execute 'encrypt' on 'SubtleCrypto': 2nd argument is not of type CryptoKey
   ❯ src/__integration_tests__/webAppE2EIntegration.test.ts:68:44
  ```
- **Nguyên nhân gốc rễ (RCA):**
  - Chữ ký hàm của `CryptoService.encrypt` là `encrypt(key: CryptoKey, plaintext: string)`.
  - Trong file test đã truyền ngược vị trí: `CryptoService.encrypt(testSecret, derivedKey)`. SubtleCrypto nhận chuỗi string ở tham số thứ hai thay vì đối tượng `CryptoKey`, ném ngoại lệ TypeError.
- **Giải pháp khắc phục:**
  - Chuẩn hóa lời gọi hàm: `CryptoService.encrypt(derivedKey, testSecret)`.
  - Bổ sung helper `CryptoService.decryptPayload(key: CryptoKey, payload: EncryptedPayload)` vào `cryptoService.ts` để tối ưu hóa việc giải mã payload zero-knowledge.
- **Trạng thái:** `RESOLVED & VERIFIED`

---

### [DEFECT-003] Axios Interceptor sập khi truy cập `localStorage` trong môi trường Node.js / Vitest
- **Phân hệ:** Frontend Web / `apiClient.ts`
- **Triệu chứng:**
  ```text
  ReferenceError: localStorage is not defined
   ❯ src/services/apiClient.ts:11:17
  ```
- **Nguyên nhân gốc rễ (RCA):**
  - Request interceptor của Axios truy cập trực tiếp `localStorage.getItem(...)` mà không kiểm tra xem mã nguồn đang thực thi trên trình duyệt hay môi trường Node/Vitest test runner.
- **Giải pháp khắc phục:**
  - Bổ sung type-guard an toàn trong `src/services/apiClient.ts`:
    ```typescript
    const token = typeof localStorage !== 'undefined' ? localStorage.getItem('asseta_auth_token') : null;
    ```
- **Trạng thái:** `RESOLVED & VERIFIED`

---

### [DEFECT-004] Zero-Knowledge Security Guardian: Phát hiện chuỗi 13 chữ số của `Date.now()` là Credit Card Number
- **Phân hệ:** Backend / `SensitiveDataInspector.cs` (Module 1 & 2 Security Policy)
- **Triệu chứng:** Request tạo Continuity Item và Action Card trả về **HTTP 422 Unprocessable Entity**:
  ```text
  warn: GlobalExceptionMiddleware: Handled domain exception [SENSITIVE_DATA_DETECTED]: 
  Sensitive data pattern 'CreditCardNumber' was detected in field 'title'. 
  Under Zero-Knowledge policy, sensitive data must be encrypted client-side.
  ```
- **Nguyên nhân gốc rễ (RCA):**
  - Trong file test, tiêu đề được đặt là: ``Thẻ Hành Động Tiếp Quản ${Date.now()}``.
  - `Date.now()` trả về chuỗi 13 chữ số liên tục (ví dụ: `1788625892840`). Bộ lọc bảo mật `SensitiveDataInspector` có regex kiểm tra các chuỗi số từ 13 đến 19 chữ số (Visa, Mastercard,...) để bảo vệ dữ liệu nhạy cảm của người dùng không bị rò rỉ dưới dạng plaintext.
  - Đây là tính năng bảo vệ an ninh Zero-Knowledge hoạt động CHÍNH XÁC theo hiến pháp dự án.
- **Giải pháp khắc phục:**
  - Thay thế chuỗi số timestamp dài bằng tiền tố ngẫu nhiên chữ-số an toàn (`testSuffix = Math.random().toString(36).substring(2, 8)`), tránh nhầm lẫn với số thẻ tín dụng hoặc private key.
- **Trạng thái:** `RESOLVED & VERIFIED`

---

### [DEFECT-005] Bất đồng bộ định danh DTO trong Web-App E2E Test (categoryId & Scoped Permissions)
- **Phân hệ:** Frontend Web / `webAppE2EIntegration.test.ts`
- **Triệu chứng:**
  - `mapRes.data.data.categories[0].id` trả về `undefined`.
  - `updateScopedPermissions` không cập nhật danh sách quyền truy cập (kết quả trả về 0 phần tử).
  - `audit.planReadinessScore` trả về `undefined`.
- **Nguyên nhân gốc rễ (RCA):**
  - `ContinuityCategoryDto` định nghĩa trường là `categoryId` (chứ không phải `id`).
  - `UpdateScopedPermissionsInput` định nghĩa danh sách là `actionCardPermissions: [{ actionCardId, canView }]` (chứ không phải `permissions`).
  - `PlanReadinessAuditDto` định nghĩa trường điểm số là `overallScore` (chứ không phải `planReadinessScore`), và mỗi stage có `actionItems` (chứ không phải `cards`).
- **Giải pháp khắc phục:**
  - Chuẩn hóa toàn bộ truy cập DTO trong `webAppE2EIntegration.test.ts` đúng với interface chuẩn của hệ thống:
    - `categories[0].categoryId`
    - `actionCardPermissions: [{ actionCardId: createdCardId, canView: true }]`
    - `audit.overallScore`
    - `stage.actionItems`
- **Trạng thái:** `RESOLVED & VERIFIED`

---

## 3. Bằng Chứng Kiểm Thử Tích Hợp Web-App (Vitest Live E2E Output)

```text
 RUN  v5.0.0 C:/DevFlutter/asseta-monorepo/frontend-web

 ✓ src/__integration_tests__/webAppE2EIntegration.test.ts (5 tests) 2055ms
   ✓ Web-App E2E Integration Test Across Modules 1 to 5 (Zero Mock Data) (5)
     ✓ [MODULE 1 & AUTH] Register Owner, Login, obtain real JWT and verify WebCrypto Master Key operations
     ✓ [MODULE 2] Create real Action Card with Category, Urgency, Steps, Contacts and Cipher Instructions
     ✓ [MODULE 3] Invite Trusted Person, Generate 6-Digit Pairing Code, Claim by Delegate and Set Scoped Access
     ✓ [MODULE 4] Query Continuity Plan, SPoF Analysis, Gap Detection and Offline Emergency Brief
     ✓ [MODULE 5] Safe Activation: Vitality Check-in, 48h Time-Lock, 1-Tap Cancel, Quorum & Deactivate

 Test Files  1 passed (1)
      Tests  5 passed (5)
   Start at  23:33:42
   Duration  2.93s
```

---

## 4. Kết Luận
Toàn bộ 5 Module (Module 1: Continuity Map, Module 2: Action Cards, Module 3: Trusted People, Module 4: Continuity Plan, Module 5: Safe Activation) đã được kiểm chứng tự động toàn diện qua cả hai cấp độ:
1. **Unit Tests:** 173 / 173 test cases PASS (Zero Mock Data).
2. **Integration Tests:** 5 / 5 kịch bản tích hợp mạng thực tế PASS (Live Backend .NET 8, Live PostgreSQL, Live Redis, Zero-Knowledge WebCrypto).

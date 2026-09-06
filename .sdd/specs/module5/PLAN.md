# Kế Hoạch Kiến Trúc & Quy Hoạch Kỹ Thuật: Module 5 – Safe Activation & Dead Man's Switch Protocol

**Mã Module:** `module5` (Tương đương `feat-05-safe-activation`)  
**Pha phát triển:** Pha 2 – Architecture & Planning  
**Vai trò đảm trách:** Senior Solution Architect & Security Lead  
**Tài liệu căn cứ:** [CONTEXT.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module5/CONTEXT.md), [SPEC.md](file:///c:/DevFlutter/asseta-monorepo/.sdd/specs/module5/SPEC.md), [Hiến Pháp Dự Án Asseta](file:///c:/DevFlutter/asseta-monorepo/.sdd/constitution.md)  
**Trạng thái:** DRAFT / WAITING FOR HUMAN APPROVAL (Gatekeeper 1)  

---

## 1. Phương Tiếp Cận Kiến Trúc (Architectural Approach)

Hệ thống tuân thủ mô hình **Clean Architecture + CQRS + MediatR** trên Backend (.NET 8 LTS), **React 18 + TanStack Query** trên Frontend Web, và **Flutter 3.x + BLoC** trên Mobile App.

```
+-------------------------------------------------------------------------------+
|                                CLIENT TIER                                    |
|   Web (React 18 / Vite)                    Mobile App (Flutter 3.x / BLoC)    |
|   - SafeActivationDashboard.tsx            - safe_activation_page.dart        |
|   - VitalityCheckInWidget.tsx              - vitality_button.dart             |
|   - EmergencyCountdownBanner.tsx           - countdown_time_lock_banner.dart  |
+---------------------------------------+---------------------------------------+
                                        | HTTPS / REST (JWT Claims)
+---------------------------------------v---------------------------------------+
|                                BACKEND TIER (.NET 8)                          |
|   Controllers: SafeActivationController                                      |
|   Middlewares: GlobalExceptionMiddleware, IdempotencyMiddleware               |
|                                       │                                       |
|   Application Layer (CQRS + MediatR):                                         |
|   - Commands: VitalityCheckIn, UpdateConfig, InitiateRequest, CancelRequest,  |
|               ConfirmRequest, DeactivateEmergencyPlan                         |
|   - Queries:  GetActivationStatusQuery, GetMyActivationRequestsQuery         |
|   - Domain Services: TimeLockEvaluator, QuorumEvaluator                       |
|                                       │                                       |
|   Domain Layer:                                                               |
|   - Entities: OwnerActivationConfig, ActivationRequest, ActivationConfirmation|
|   - Enums:    HeartbeatStatus, ActivationTriggerSource, ActivationRequestStatus|
|                                       │                                       |
|   Infrastructure:                                                             |
|   - AssetaDbContext + EF Core 8 (Npgsql)                                      |
|   - Tables: owner_activation_configs, activation_requests,                    |
|             activation_confirmations, continuity_audit_logs                   |
+-------------------------------------------------------------------------------+
```

---

## 2. Thiết Kế Cơ Sở Dữ Liệu (PostgreSQL 16 Schema)

### 2.1. Bảng `owner_activation_configs`
Lưu trữ cấu hình điểm danh và tham số kích hoạt an toàn của Chủ tài sản:
```sql
CREATE TABLE owner_activation_configs (
    id UUID PRIMARY KEY,
    owner_id UUID NOT NULL UNIQUE,
    check_in_interval_days INT NOT NULL DEFAULT 30,
    grace_period_hours INT NOT NULL DEFAULT 48,
    min_confirmations_required INT NOT NULL DEFAULT 1,
    last_check_in_at_utc TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    next_check_in_due_utc TIMESTAMPTZ NOT NULL,
    status VARCHAR(32) NOT NULL DEFAULT 'ACTIVE',
    row_version INT NOT NULL DEFAULT 1,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at_utc TIMESTAMPTZ
);

CREATE INDEX idx_activation_configs_owner ON owner_activation_configs(owner_id);
CREATE INDEX idx_activation_configs_due ON owner_activation_configs(next_check_in_due_utc, status);
```

### 2.2. Bảng `activation_requests`
Lưu trữ các phiên phát động yêu cầu kích hoạt khẩn cấp:
```sql
CREATE TABLE activation_requests (
    id UUID PRIMARY KEY,
    owner_id UUID NOT NULL,
    trigger_source VARCHAR(32) NOT NULL, -- 'TRUSTED_PERSON_REQUEST' hoặc 'SYSTEM_TIMEOUT'
    initiated_by_trusted_person_id UUID NULL,
    reason TEXT NULL,
    status VARCHAR(32) NOT NULL DEFAULT 'PENDING_GRACE_PERIOD', -- 'PENDING_GRACE_PERIOD', 'CANCELLED_BY_OWNER', 'ACTIVATED', 'REJECTED'
    grace_period_expires_at_utc TIMESTAMPTZ NOT NULL,
    cancelled_at_utc TIMESTAMPTZ NULL,
    activated_at_utc TIMESTAMPTZ NULL,
    row_version INT NOT NULL DEFAULT 1,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at_utc TIMESTAMPTZ,
    CONSTRAINT fk_activation_requests_trusted_person 
        FOREIGN KEY (initiated_by_trusted_person_id) REFERENCES trusted_people(id) ON DELETE SET NULL
);

CREATE INDEX idx_activation_requests_owner_status ON activation_requests(owner_id, status);
```

### 2.3. Bảng `activation_confirmations`
Lưu trữ phiếu biểu quyết/xác nhận từ các Người Ủy Thác:
```sql
CREATE TABLE activation_confirmations (
    id UUID PRIMARY KEY,
    activation_request_id UUID NOT NULL,
    trusted_person_id UUID NOT NULL,
    is_confirmed BOOLEAN NOT NULL DEFAULT TRUE,
    note TEXT NULL,
    confirmed_at_utc TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at_utc TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at_utc TIMESTAMPTZ,
    CONSTRAINT fk_activation_confirmations_request 
        FOREIGN KEY (activation_request_id) REFERENCES activation_requests(id) ON DELETE CASCADE,
    CONSTRAINT fk_activation_confirmations_trusted_person 
        FOREIGN KEY (trusted_person_id) REFERENCES trusted_people(id) ON DELETE CASCADE,
    CONSTRAINT uq_request_trusted_person UNIQUE (activation_request_id, trusted_person_id)
);
```

---

## 3. Hợp Đồng API (RESTful API Contracts)

| Method | Endpoint | Mô tả | Authorization |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/safe-activation/status` | Lấy trạng thái kích hoạt, thời hạn check-in, countdown đệm | Owner & Active Delegates |
| `POST` | `/api/v1/safe-activation/check-in` | Thực hiện điểm danh "Tôi Vẫn Ổn", gia hạn check-in | Owner only |
| `PUT` | `/api/v1/safe-activation/config` | Cập nhật cấu hình chu kỳ, thời gian đệm và quorum | Owner only |
| `POST` | `/api/v1/safe-activation/requests` | Phát động yêu cầu kích hoạt khẩn cấp | Level 2/3 Active Delegate |
| `POST` | `/api/v1/safe-activation/requests/{id}/cancel` | Hủy yêu cầu kích hoạt 1-chạm | Owner only |
| `POST` | `/api/v1/safe-activation/requests/{id}/confirm` | Biểu quyết xác nhận yêu cầu kích hoạt | Level 2/3 Active Delegate |
| `POST` | `/api/v1/safe-activation/deactivate` | Tắt chế độ khẩn cấp, khôi phục trạng thái Normal | Owner only |

---

## 4. Cơ Chế Mã Hóa & Luồng An Toàn Zero-Knowledge

1. **Trạng Thái Bình Thường (`NORMAL` / `ACTIVE`):**
   - Server tuyệt đối không giải mã và không cung cấp khóa cho Người Ủy Thác.
   - Người Ủy Thác chỉ thấy vai trò và bản tóm lược an toàn (nhóm theo 4 giai đoạn từ Module 4).
2. **Khi Kích Hoạt Khẩn Cấp (`ACTIVATED`):**
   - Server đánh dấu trạng thái kế hoạch là `EmergencyActive`.
   - Các API truy xuất Thẻ hành động trả về nội dung cho phép giải mã phía Client đối với các thẻ thuộc phạm vi phân quyền Scoped Access.
   - Không chia sẻ chéo: Delegate A không thể xem hoặc giải mã thẻ của Delegate B.
3. **Khi Khôi Phục (`DEACTIVATED`):**
   - Ngay lập tức cắt bỏ mọi quyền truy xuất khẩn cấp của Người Ủy Thác, đưa toàn bộ hệ thống về trạng thái Normal.

---

## 5. Phân Tích Rủi Ro & Biện Pháp Giảm Thiểu (Risks & Mitigation)

| Rủi ro kỹ thuật | Mức độ | Phương án xử lý |
| :--- | :--- | :--- |
| **Rủi ro 1: Kích hoạt thù địch / cướp quyền (Hostile Activation)** | Cao | Bắt buộc có **Time-Lock Grace Period tối thiểu 24-48 giờ**. Gửi cảnh báo đa kênh (SMS, Email, Push). Chủ tài sản có quyền hủy 1-chạm ngay lập tức từ điện thoại. |
| **Rủi ro 2: Chủ tài sản đi công tác / mất mạng tạm thời (False Positive)** | Trung bình | Gửi thông báo nhắc nhở trước 7 ngày; yêu cầu xác nhận độc lập từ các Người Ủy Thác khác (Quorum); Chủ tài sản có thể khôi phục trạng thái bất kỳ lúc nào sau khi có mạng. |
| **Rủi ro 3: Xung đột tranh chấp đồng thời (Concurrency Race)** | Trung bình | Áp dụng khóa lạc quan `RowVersion` trên cả `OwnerActivationConfig` và `ActivationRequest`. Nếu Owner hủy đồng thời khi hệ thống đang kích hoạt, quyền của Owner luôn được ưu tiên tuyệt đối. |

---

## 6. Câu Hỏi Xác Nhận Với Human (Gatekeeper 1)

1. Chu kỳ điểm danh mặc định nên là **30 ngày**, và thời gian đệm bảo vệ (Grace Period) mặc định là **48 giờ** hay **72 giờ**? (Kế hoạch đề xuất mặc định: 30 ngày check-in và 48 giờ đệm).
2. Khi người ủy thác gửi yêu cầu kích hoạt, hệ thống tự động tính người đó là 1 phiếu xác nhận đầu tiên? (Đồng ý, giúp quy trình minh bạch).

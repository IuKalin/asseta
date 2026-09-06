---
description: asseta-sdd-
---

# GEMINI.md — ANTIGRAVITY ORCHESTRATION & SDD WORKFLOW SPECIFICATION

# Project: Asseta (Personal Continuity System)

# Method: Hybrid Spec-Driven Development (SDD) & Agent-Driven Development (ADD)

# Version: 1.0.0 | Status: ACTIVE | Target: Production 2026

================================================================================
SECTION 1: SYSTEM ROLES & GOVERNANCE RULES
================================================================================

## 1.1 VAI TRÒ CỦA GEMINI / AGENT TRONG ANTIGRAVITY

Bạn là AI Solution Architect & Senior Tech Lead Engine trực thuộc hệ điều phối Antigravity.

- Bạn KHÔNG viết code bừa bãi khi chưa có Spec và Plan được con người phê duyệt.
- Bạn tuân thủ triết lý "Core & Shell": SDD quản lý Core bất biến; ADD tăng tốc Shell ngoại vi.
- Bạn xem "Spec là Interface", "Code là dẫn xuất tạm thời", "Unit Test là thước đo chân lý".

## 1.2 NGUYÊN TẮC BẤT BIẾN (CONSTITUTIONAL CONSTRAINTS)

1. ZERO-KNOWLEDGE & NON-POSSESSION:
   - Tuyệt đối KHÔNG BAO GIỜ lưu trữ mật khẩu, OTP, mã thẻ tín dụng (CVV), seed phrase, khóa private key trên server.
   - Hướng dẫn vị trí giấy tờ hoặc ghi chú khẩn cấp PHẢI được mã hóa client-side trước khi truyền qua mạng bằng Master Key do người dùng giữ. Server ASP.NET Core & PostgreSQL chỉ lưu bản mã (CipherBlob).
2. STACK BẮT BUỘC:
   - Backend: ASP.NET Core 8 Web API (Clean Architecture + MediatR CQRS + PostgreSQL 16 qua Docker).
   - Mobile: Flutter 3.x (Clean Architecture + BLoC State Management).
   - Web: ReactJS 18+ (TypeScript + Vite + TailwindCSS + TanStack Query).
3. WORKFLOW INTEGRITY:
   - Nghiêm cấm nhảy cóc các pha. Mọi chuyển đổi trạng thái phải đi qua Gatekeeper (Human Checkpoint).
   - Nếu một tác vụ hoặc module quá phức tạp (> 4 giờ làm việc hoặc > 5 nghiệp vụ độc lập), BẮT BUỘC phải phân rã thành các Sub-features trước khi kích hoạt quy trình 5 pha.

================================================================================
SECTION 2: PIPELINE 5 PHA SDD CHO ANTIGRAVITY AGENTS
================================================================================

Luồng hoạt động tuần tự của Antigravity Agent:
[Pha 0: Context Discovery] ──► [Pha 1: Spec Writing] ──► [Pha 2: Architecture & Plan]
                                                                  │
                                                        [🛑 Human Gate 1: Approval]
                                                                  ▼
[Pha 5: Validation & Audit] ◄── [Pha 4: Implementation] ◄── [Pha 3: Task Decomposition]
         │
[🛑 Human Gate 2: PR Merge]

--------------------------------------------------------------------------------

### PHA 0: CONTEXT DISCOVERY (ĐỊNH HÌNH BỐI CẢNH & PHẠM VI)

--------------------------------------------------------------------------------

- Mục tiêu: Định danh ranh giới bài toán, thuật ngữ nghiệp vụ, ràng buộc kỹ thuật.
- Trách nhiệm: Human định hướng + AI rà soát.
- Artifact sinh ra: `.sdd/specs/feat-{name}/CONTEXT.md`
- Cấu trúc CONTEXT.md:
  1. Problem Statement: Vấn đề sinh tử mà feature giải quyết.
  2. Domain Knowledge: Thuật ngữ, từ điển nghiệp vụ chuẩn xác.
  3. Constraints: Ràng buộc kiến trúc, công nghệ, bảo mật.
  4. Assumptions & Open Questions: Giả định kỹ thuật và các câu hỏi cần làm rõ.
  5. Out of Scope: Những gì tuyệt đối KHÔNG làm.

--------------------------------------------------------------------------------

### PHA 1: SPECIFICATION WRITING (ĐẶC TẢ HÀNH VI EARS)

--------------------------------------------------------------------------------

- Mục tiêu: Viết đặc tả yêu cầu không thể diễn giải sai, loại bỏ tính mơ hồ.
- Trách nhiệm: Human/AI cộng tác.
- Artifact sinh ra: `.sdd/specs/feat-{name}/SPEC.md` (Version: 1.0.0 APPROVED).
- Quy chuẩn cú pháp EARS (Easy Approach to Requirements Syntax):
  - Ubiquitous: "Hệ thống SHALL [hành vi]."
  - Event-driven: "WHEN [sự kiện kích hoạt], THE hệ thống SHALL [hành vi]."
  - State-driven: "WHILE [trạng thái duy trì], THE hệ thống SHALL [hành vi]."
  - Unwanted behavior: "IF [lỗi/ngoại lệ], THEN hệ thống SHALL [xử lý]."
  - Optional: "WHERE [điều kiện tuỳ chọn], THE hệ thống SHALL [hành vi]."
- Bộ 8 thành phần bắt buộc trong SPEC.md:
  1. Context & Goal | 2. Actors & Roles | 3. Functional Requirements (EARS) |
  2. Non-Functional Requirements (Có số đo) | 5. Data Model (Schema) |
  3. Error Handling | 7. Acceptance Criteria (Given-When-Then checklist) |
  4. Out of Scope (Tường minh những thứ không làm).

--------------------------------------------------------------------------------

### PHA 2: ARCHITECTURE & PLANNING (QUY HOẠCH KỸ THUẬT)

--------------------------------------------------------------------------------

- Mục tiêu: Thiết kế chi tiết cấu trúc hệ thống, schema PostgreSQL, contracts API và phân tích rủi ro.
- Trách nhiệm: AI đóng vai Senior Solution Architect.
- Artifact sinh ra: `.sdd/specs/feat-{name}/PLAN.md`.
- Nội dung PLAN.md:
  1. Architectural Approach: Clean Architecture, CQRS handlers, BLoC pattern.
  2. Database Schema: Bảng PostgreSQL, kiểu dữ liệu UUID, khóa ngoại, chỉ mục (Index).
  3. API Contracts: Định nghĩa endpoints, Request/Response payloads, mã lỗi HTTP.
  4. Cryptographic Flow: Mô tả luồng Client-Side Master Key mã hóa Blob trước khi qua API.
  5. Risks & Mitigation: Tối thiểu 3 rủi ro kỹ thuật và phương án xử lý.
  6. Questions for Human: Các giả định còn nghi ngờ cần làm rõ.
- 🛑 GATEKEEPER 1: Agent DỪNG LẠI tại đây. Human phải review và gõ lệnh phê duyệt PLAN.md mới được chuyển tiếp sang Pha 3.

--------------------------------------------------------------------------------

### PHA 3: TASK DECOMPOSITION (PHÂN RÃ CÔNG VIỆC NGUYÊN TỬ)

--------------------------------------------------------------------------------

- Mục tiêu: Chuyển PLAN.md thành các task độc lập, có thể thực thi tuần tự hoặc song song.
- Trách nhiệm: AI Technical Project Lead.
- Artifact sinh ra: `.sdd/specs/feat-{name}/TASKS.md`.
- Quy chuẩn Task:
  - Tính chất: Atomic (nguyên tử), Independent (độc lập), Verifiable (kiểm thử được).
  - Giới hạn thời gian: Mỗi task ≤ 4 giờ làm việc.
  - Phân loại prefix: [BE-CORE], [MOB-SHELL], [WEB-SHELL], [INTEG-TEST].
  - Cấu trúc mỗi task:
    - ID: `[FEAT-XXX-T001]`
    - Action: Động từ + danh từ rõ ràng.
    - Files: Danh sách files cần tạo/chỉnh sửa.
    - EARS Ref: Trỏ về ID điều khoản trong SPEC.md.
    - Definition of Done (DoD): Tiêu chí test xanh để đánh dấu hoàn thành.

--------------------------------------------------------------------------------

### PHA 4: AGENTIC IMPLEMENTATION (THỰC THI & TỰ KIỂM TRA)

--------------------------------------------------------------------------------

- Mục tiêu: Tạo mã nguồn và unit test song hành theo nguyên tắc Plan-Act-Check.
- Trách nhiệm: AI Coder Agent (vận hành qua Antigravity tool calling).
- Quy trình bắt buộc của Agent:
  1. Shadow Plan: Trước khi sửa file, Agent in ra dự định (Sẽ đọc file nào? Sẽ sửa file nào? Sẽ chạy lệnh gì?).
  2. Test-First / Test-Driven: Luôn viết unit test đồng thời với code logic.
  3. Environment Execution: Chạy build và test bằng command line:
     - Backend: `dotnet test`
     - Flutter: `flutter test`
     - Web: `npm run test`
  4. Self-Healing Loop: Nếu test đỏ hoặc build fail, Agent tự đọc stderr, sửa code và chạy lại test. Nếu quá 3 lần lặp không sửa được, gọi Escape Hatch nhờ con người can thiệp.
  5. Checkpoint: Cập nhật dấu `[x]` vào file `TASKS.md`.

--------------------------------------------------------------------------------

### PHA 5: VALIDATION & COMPLIANCE AUDIT (THẨM ĐỊNH NGUỒN GỐC)

--------------------------------------------------------------------------------

- Mục tiêu: Kiểm chứng tính toàn vẹn giữa Spec, Code và Tests.
- Trách nhiệm: AI Auditor + Human Final Review.
- Artifact sinh ra: `.sdd/specs/feat-{name}/VALIDATION.md`.
- Bảng Traceability Matrix bắt buộc:
  | EARS Spec ID | Tóm tắt Yêu cầu | File Code / Vị trí | Tên Unit Test | Trạng thái (PASS/FAIL) |
- 3 Vòng kiểm tra an toàn:
  1. Spec Compliance: Mọi SHALL đều có code và test tương ứng.
  2. Out of Scope Check: Grep codebase đảm bảo AI không tự chế thêm tính năng thừa.
  3. Security Self-Check: Quét hardcode secrets, kiểm tra SQL injection, kiểm tra Zero-Knowledge.
- 🛑 GATEKEEPER 2: Human nghiệm thu ma trận Traceability Matrix trước khi merge code vào develop/main.

================================================================================
SECTION 3: QUY TẮC PHÂN RÃ FEATURE LỚN (DECOMPOSITION PROTOCOL)
================================================================================

Khi một module có quy mô lớn, Antigravity BẮT BUỘC phải chia nhỏ thành các Feature con:

1. QUY TẮC KÍCH HOẠT PHÂN RÃ:
   - Module có > 2 tầng kiến trúc liên đới (VD: Cần cả Quartz Background Worker, FCM Notification và PostgreSQL Schema).
   - Logic nghiệp vụ có chứa State Machine > 3 trạng thái.
   - Module chứa cả phần mã hóa Client lẫn lưu trữ Server.

2. CẤU TRÚC PHÂN RÃ THỰC THẾ CHO ASSETA:
   - Module Safe Activation:
     - Feat-05A: Dead Man's Switch Heartbeat & Scheduler (Chu kỳ 7/14/30 ngày).
     - Feat-05B: Grace Period Multi-Channel Alert (48h hoãn cảnh báo trên Flutter).
     - Feat-05C: Master Key Secret Sharing & Handover (Mở khóa và bàn giao dữ liệu).
   - Module Trusted People:
     - Feat-03A: Mobile Identity Handshake & Pairing Code (Cài app Flutter & liên kết).
     - Feat-03B: Need-to-Know Scoped Access Control (Phân quyền thẻ tiếp quản).

3. ĐIỀU PHỐI VÒNG LẶP:
   - Mỗi Feature con (Sub-feature) được cấp riêng một thư mục trong `.sdd/specs/` và thực thi ĐẦY ĐỦ quy trình 5 pha độc lập.

================================================================================
SECTION 4: META-PROMPT ENGINE DÀNH CHO ANTIGRAVITY RUNNER
================================================================================

Dưới đây là các chỉ lệnh kích hoạt mà Orchestrator sẽ inject vào context của Agent:

### LỆNH KÍCH HOẠT PHA 0 & PHA 1

```text
Role: Business Analyst & Spec Architect.
Context: Đọc kỹ GEMINI.md và tài liệu nền tảng tại root context.
Task: Khởi tạo thư mục `.sdd/specs/{FEAT_NAME}/`. Tạo ra:
1. CONTEXT.md: Bối cảnh, mục tiêu, ràng buộc, giả định và Out of Scope.
2. SPEC.md: Chuẩn hóa 8 thành phần với cú pháp EARS.
Yêu cầu: KHÔNG code. Kiểm tra nghiêm ngặt quy tắc Zero-Knowledge.

Role: Principal Solution Architect.
Task: Đọc file `.sdd/specs/{FEAT_NAME}/SPEC.md`. Tạo ra file PLAN.md.
Nội dung: Thiết kế Clean Architecture ASP.NET Core, Schema PostgreSQL, Flutter BLoC, Contracts API và Cryptographic Flow.
Ràng buộc: DỪNG LẠI sau khi viết xong PLAN.md. Chờ lệnh human approve trước khi sang Pha 3.

Role: Technical Project Lead.
Task: Đọc PLAN.md đã được duyệt. Tạo ra file TASKS.md.
Nội dung: Danh sách task nguyên tử ([BE-CORE], [MOB-SHELL], [WEB-SHELL]), thời lượng ≤ 4h/task, gắn tag EARS Spec Ref và Acceptance Criteria.

Role: Full-stack Senior Software Engineer.
Task: Thực thi Task [{TASK_ID}] trong file TASKS.md.
Quy trình: 
1. Hiển thị Shadow Plan.
2. Triển khai Code song song với Unit Test.
3. Chạy `dotnet test` hoặc `flutter test` thực tế qua CLI.
4. Tự sửa lỗi nếu test fail. Đánh dấu [x] vào TASKS.md khi tất cả pass.

Role: Quality Assurance & Security Auditor.
Task: Đối soát mã nguồn dự án với SPEC.md của {FEAT_NAME}.
Nội dung: Tạo VALIDATION.md chứa Traceability Matrix (Spec ↔ Code ↔ Tests), quét sạch các đoạn code ngoài scope, và xác thực tính an toàn bảo mật.


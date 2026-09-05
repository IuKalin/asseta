# Quy Định Ngữ Cảnh & Quyền Hạn Cho AI Agents (Agent Constitution)

Tài liệu này định danh quyền hạn, vai trò và quy chuẩn làm việc cho các Agent trong monorepo Asseta.

---

## 1. Phân Định Quyền Hạn (Agent Personas)

1. **System Architect Agent**: Quyền chỉnh sửa `.sdd/`, cấu trúc thư mục, quy chuẩn kiến trúc và ADR.
2. **Backend Agent (.NET 8)**: Quyền chỉnh sửa `backend/`, phụ trách Clean Architecture, EF Core, CQRS.
3. **Frontend Agent (React)**: Quyền chỉnh sửa `frontend-web/`, tuân thủ thiết kế UI/UX, responsive, Axios client.
4. **Mobile Agent (Flutter)**: Quyền chỉnh sửa `mobile-app/`, tuân thủ Clean Architecture + BLoC.
5. **QA & Security Gatekeeper Agent**: Kiểm tra tuân thủ hiến pháp `.sdd/constitution.md`, EARS và không cho phép feature creep.

---

## 2. Decision Framework Bắt Buộc (Step 5.5.4)

| Loại Issue | Action bắt buộc | Ví dụ xử lý |
| :--- | :--- | :--- |
| **Logic gap nghiêm trọng** | Thêm quy tắc xử lý trực tiếp vào Spec NGAY | Concurrent submission -> Bổ sung quy tắc Idempotency |
| **Edge case quan trọng** | Thêm vào mục Unwanted Patterns / Constraints | Product bị xóa -> Thêm `WHERE product.deleted = false` |
| **Ambiguity có ảnh hưởng** | Làm rõ hành vi mong muốn và SLA trong Spec | Rating consistency -> Quy định rõ "eventual consistency, chấp nhận trễ tối đa 5 phút" |
| **Out of scope (intentional)** | Ghi rõ ràng vào mục "Out of Scope" của tài liệu | Buyer ban handling -> "Out of scope cho sprint/milestone này" |
| **Nice to have, low risk** | Chuyển vào Backlog / Future Tasks (không block spec) | Review cooldown -> Tạo backlog item, không đưa vào luồng chính |
| **False positive của AI** | Bỏ qua (Ignore) và ghi chú lý do kỹ thuật | Rating null vs 0 -> Xử lý ở tầng DB Schema, không đưa vào spec |

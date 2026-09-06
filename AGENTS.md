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

---

## 3. Quy Chuẩn Ponytail (Lazy Senior Dev Mode & YAGNI)

Tất cả các Agent khi viết hoặc sửa code PHẢI áp dụng thang bậc tư duy Ponytail (The Ladder) trước khi sinh code:

1. **Có cần thiết phải làm không? (YAGNI)**: Yêu cầu mang tính suy đoán, chưa cần dùng -> Bỏ qua.
2. **Đã có sẵn trong codebase chưa?**: Tái sử dụng helper, util, type, hoặc pattern hiện có; tuyệt đối không code lại cái đã có.
3. **Thư viện chuẩn (Stdlib) có sẵn không?**: Tận dụng tối đa BCL (.NET), Web API / JS native, Dart core.
4. **Nền tảng gốc (Native Platform) có hỗ trợ không?**: Tận dụng tính năng native của trình duyệt, hệ điều hành, DB constraint thay vì viết logic app cồng kềnh.
5. **Thư viện đã cài đặt có giải quyết được không?**: Không cài thêm package mới nếu code tối thiểu hoặc package sẵn có giải quyết được.
6. **Có thể viết trong 1 dòng / tối giản không?**: Luôn chọn cách viết trực diện, ngắn gọn nhất.
7. **Chỉ khi các bước trên không thỏa mãn**: Mới viết lượng code tối thiểu hoạt động được.

### Nguyên Tắc Cốt Lõi
- **Bug fix = Root Cause, not Symptom**: Truy tìm nguyên nhân gốc rễ, sửa một lần tại nguồn thay vì chắp vá từng chỗ gọi.
- **Không sinh Abstraction thừa**: Không thêm interface, factory, pattern trừ khi người dùng hoặc spec yêu cầu rõ ràng.
- **Xóa code tốt hơn thêm code**: Ưu tiên xóa bỏ boilerplate, giữ diff ngắn nhất, an toàn nhất (100% Data safety & Trust boundaries).

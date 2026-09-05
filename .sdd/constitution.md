# Hiến Pháp Dự Án Asseta (Asseta Constitution)

**Phiên bản:** v1.0.0  
**Hiệu lực:** Toàn bộ monorepo (Backend, Frontend, Mobile, DevOps)  
**Trạng thái:** BẤT BIẾN (IMMUTABLE)

---

## 1. Nguyên Tắc Cốt Lõi (Core Principles)

1. **Spec First & Zero Assumption**: Không dòng code nào được viết nếu chưa có tài liệu đặc tả chuẩn EARS được phê duyệt (APPROVED) trong `.sdd/specs/`.
2. **Zero Trust & Defense in Depth**: Mọi thao tác truy cập tài sản, chuyển giao quyền hạn, giải mã bí mật đều phải trải qua xác thực đa yếu tố và ghi log kiểm toán (Audit Trail) không thể đảo ngược.
3. **Idempotency & Resilience**: Mọi API mutation (POST/PUT/DELETE) và tác vụ chuyển trạng thái tài sản phải hỗ trợ Idempotency Key để đảm bảo an toàn tuyệt đối khi mạng chập chờn.
4. **Data Privacy & Client-side Safety**: Khóa bí mật cá nhân và mật khẩu giải mã tài sản KHÔNG BAO GIỜ được lưu trữ dạng plaintext tại máy chủ. Áp dụng mã hóa đầu cuối (E2EE) hoặc Shamir Secret Sharing.
5. **No Feature Creep**: Tất cả các ý tưởng, tối ưu hóa không nằm trong Scope của `SPEC.md` phải được chuyển vào Backlog, nghiêm cấm tự ý bổ sung vào mã nguồn.

---

## 2. Quy Chuẩn Đặt Tên & Cấu Trúc File

- **Module SDD**: Đặt tên thư mục theo định dạng `feat-XX-ten-tinh-nang`
- **Tập tin bắt buộc trong mỗi Module**:
  1. `CONTEXT.md`: Bối cảnh nghiệp vụ, stakeholder, user journey.
  2. `SPEC.md`: 8 thành phần chuẩn EARS, State Machine, Contract.
  3. `PLAN.md`: Kế hoạch chia pha, phụ thuộc, milestone.
  4. `TASKS.md`: Danh sách công việc phân rã theo checklist chi tiết.

---

## 3. Decision Framework Phân Loại Vấn Đề

| Loại Issue | Action bắt buộc |
| :--- | :--- |
| **Logic gap nghiêm trọng** | Thêm quy tắc xử lý trực tiếp vào Spec NGAY |
| **Edge case quan trọng** | Thêm vào mục Unwanted Patterns / Constraints |
| **Ambiguity có ảnh hưởng** | Làm rõ hành vi mong muốn và SLA trong Spec |
| **Out of scope (intentional)** | Ghi rõ ràng vào mục "Out of Scope" của tài liệu |
| **Nice to have, low risk** | Chuyển vào Backlog / Future Tasks (không block spec) |
| **False positive của AI** | Bỏ qua (Ignore) và ghi chú lý do kỹ thuật |

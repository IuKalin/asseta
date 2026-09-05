# 2. Áp Dụng Phương Pháp Luận Spec-Driven Development (SDD)

**Ngày:** 2026-09-05  
**Trạng thái:** Chấp nhận (Accepted)

## Bối Cảnh
Hệ thống kết hợp sự phát triển của nhiều AI Agent (Backend, Frontend, Mobile, QA) cùng lập trình viên con người, cần một Single Source of Truth bất biến để loại bỏ sai lệch ngữ cảnh.

## Quyết Định
Sử dụng thư mục `.sdd/` làm Single Source of Truth duy nhất:
1. Mọi tính năng phải có đặc tả chuẩn 8 thành phần EARS.
2. Tuân thủ `constitution.md` và hệ thống ràng buộc 3 tầng (Stack, Business, Safety).
3. Tuân thủ Decision Framework cho việc phân loại và xử lý phản hồi.

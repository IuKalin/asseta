# Ràng Buộc Nghiệp Vụ (Business Rules Constraints)

1. **Nguyên tắc Ủy Thác (Trusted Delegation)**:
   - Một người được ủy thác (Delegate) chỉ có quyền xem tài sản khi giao thức Kích Hoạt An Toàn (Safe Activation) đã hoàn thành đầy đủ thời gian chờ (Time-lock cooldown) mà chủ sở hữu không hủy bỏ.
2. **Nguyên tắc Bất Biến Lịch Sử (Audit Immutability)**:
   - Mọi chỉnh sửa về danh mục tài sản, thay đổi người ủy thác và yêu cầu kích hoạt kế hoạch khẩn cấp phải ghi nhật ký kiểm toán không thể xóa (`is_deleted = false`, log vĩnh viễn).
3. **Phân Quyền Thẻ Hành Động (Action Cards Permission)**:
   - Thẻ hành động khẩn cấp chỉ được hiển thị nội dung nhạy cảm (mật mã tài khoản, hướng dẫn két sắt) cho đúng đối tượng được chỉ định sau khi kích hoạt thành công.

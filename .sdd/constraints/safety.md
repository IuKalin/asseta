# Ràng Buộc An Toàn & Mật Mã (Safety & Cryptography Constraints)

1. **Mã Hóa Dữ Liệu**:
   - Sử dụng chuẩn `AES-256-GCM` cho dữ liệu nhạy cảm của tài sản (Secure Vault).
   - Khóa giải mã được dẫn xuất qua `Argon2id` hoặc phân chia qua thuật toán `Shamir's Secret Sharing (k-of-n)`.
2. **Bảo Vệ Rate Limit & Chống Brute Force**:
   - Áp dụng Rate Limiting tối đa 5 yêu cầu kích hoạt khẩn cấp trong 1 giờ cho mỗi tài khoản.
3. **Time-lock Safeguard**:
   - Bất kỳ yêu cầu kích hoạt kế hoạch chuyển giao nào đều phải có độ trễ tối thiểu 24 giờ (hoặc tùy cấu hình người dùng từ 12h - 72h) để ngăn chặn hành vi chiếm đoạt tài khoản tức thời.

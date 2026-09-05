# Mẫu Đặc Tả Chuẩn 8 Thành Phần EARS (Easy Approach to Requirements Syntax)

## 1. Ubiquitous Requirements (Phổ quát)
- Hệ thống PHẢI luôn luôn [yêu cầu liên tục].

## 2. Event-driven Requirements (Theo sự kiện)
- KHI [sự kiện kích hoạt xảy ra], hệ thống PHẢI [hành động đáp ứng].

## 3. State-driven Requirements (Theo trạng thái)
- TRONG KHI [hệ thống đang ở trạng thái X], hệ thống PHẢI [duy trì hành vi Y].

## 4. Unwanted Behavior Requirements (Xử lý lỗi & bất thường)
- NẾU [điều kiện lỗi / dữ liệu không hợp lệ], THÌ hệ thống PHẢI [phản hồi an toàn, thông báo lỗi rõ ràng].

## 5. Optional Requirements (Tùy chọn)
- NƠI MÀ [tính năng tùy chọn được bật], hệ thống PHẢI [thực hiện tác vụ mở rộng].

## 6. Complex Requirements (Phức hợp)
- KHI [sự kiện A] TRONG KHI [trạng thái B], NẾU [điều kiện C], THÌ hệ thống PHẢI [xử lý phức hợp].

## 7. Edge Cases & Boundary Conditions (Trường hợp biên)
- Xử lý khi dữ liệu chạm ngưỡng, mạng rớt giữa chừng, thao tác đồng thời (concurrency).

## 8. Acceptance Criteria & Verification (Tiêu chí nghiệm thu)
- [ ] Gherkin Given-When-Then kịch bản 1
- [ ] Gherkin Given-When-Then kịch bản 2

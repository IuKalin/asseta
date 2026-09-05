# 3. Lựa Chọn ASP.NET Core 8 Clean Architecture

**Ngày:** 2026-09-05  
**Trạng thái:** Chấp nhận (Accepted)

## Bối Cảnh
Backend cần xử lý nghiệp vụ bảo vệ tài sản, time-lock và kiểm toán với tính độc lập cao giữa CSDL và giao diện API.

## Quyết Định
Triển khai Clean Architecture gồm 4 tầng:
- `Asseta.Domain`: Core Entities, Enums, Value Objects, Domain Exceptions (Không phụ thuộc tầng ngoài).
- `Asseta.Application`: CQRS Use Cases, Interfaces, DTOs.
- `Asseta.Infrastructure`: EF Core, PostgreSQL Context, Repositories, Security Services.
- `Asseta.Api`: Controllers, Middleware, OpenAPI configuration.

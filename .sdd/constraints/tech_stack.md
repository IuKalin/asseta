# Ràng Buộc Công Nghệ (Tech Stack Constraints)

- **Backend**:
  - Runtime: .NET 8 LTS (C# 12)
  - Kiến trúc: Clean Architecture (Domain, Application, Infrastructure, Api)
  - Thư viện: MediatR (CQRS), FluentValidation, EF Core 8, Npgsql
  - CSDL: PostgreSQL 16 (hỗ trợ pgcrypto, uuid-ossp), Redis 7
- **Frontend Web**:
  - Runtime: Node.js 20+, React 18+
  - Build Tool: Vite + TypeScript
  - Styling: TailwindCSS
  - State & HTTP: Zustand / TanStack Query, Axios
- **Mobile App**:
  - Framework: Flutter 3.x (Dart 3.x)
  - Kiến trúc: Clean Architecture + BLoC State Management
  - Dependencies: `flutter_bloc`, `equatable`, `dio`, `get_it`, `dartz`

# Bộ Nhớ Dự Án & Kiến Trúc Tóm Tắt (Project Memory)

## Tổng Quan Kiến Trúc
- **Tên dự án**: Asseta Monorepo (Quản lý và chuyển giao kế thừa tài sản an toàn).
- **Mô hình phát triển**: Spec-Driven Development (SDD) & Agent-Driven Development.
- **Backend**: ASP.NET Core 8 Web API (Clean Architecture + MediatR CQRS + EF Core).
- **Frontend**: React 18 + Vite + TypeScript + TailwindCSS.
- **Mobile**: Flutter 3.x + Clean Architecture + BLoC.
- **Database**: PostgreSQL 16 + Redis.

## Lệnh Thường Dùng (Cheatsheet)
- Backend build: `cd backend && dotnet build`
- Frontend dev: `cd frontend-web && npm run dev`
- Mobile run: `cd mobile-app && flutter run`
- Docker stack: `docker compose -f docker/docker-compose.yml up -d`

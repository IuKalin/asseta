# Quick Start & Run Guide

Tài liệu hướng dẫn chi tiết tiếng Việt: [Xem Hướng Dẫn Khởi Chạy Chi Tiết](file:///c:/DevFlutter/asseta-monorepo/docs/HUONG_DAN_CHAY_APP.md)

---

## Quick Terminal Commands Reference

### 1. Start Infrastructure (PostgreSQL & Redis)
```bash
docker compose -f docker/docker-compose.yml up -d postgres redis
```

### 2. Run Backend API (.NET 8 - Port 5000)
```bash
cd backend
dotnet ef database update --project src/Asseta.Infrastructure --startup-project src/Asseta.Api
dotnet run --project src/Asseta.Api --urls=http://localhost:5000
```
- Health Check: `http://localhost:5000/`
- Swagger UI: `http://localhost:5000/swagger`

### 3. Run Frontend Web (React + Vite - Port 5173)
```bash
cd frontend-web
npm install
npm run dev
```
- Web URL: `http://localhost:5173/`

### 4. Run Mobile App (Flutter)
```bash
cd mobile-app
flutter pub get
flutter run -d windows   # For Windows Desktop
flutter run -d chrome    # For Web Browser
```

### 5. Automated Tests (Zero Mock Data)
```bash
dotnet test backend/tests/Asseta.UnitTests                           # 96 Backend tests
cd frontend-web && npm test                                         # 26 Web tests
cd mobile-app && flutter test                                       # 51 Flutter tests
cd frontend-web && npx vitest run src/__integration_tests__/webAppE2EIntegration.test.ts  # 5 E2E tests
```

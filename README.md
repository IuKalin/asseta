# Asseta Monorepo

> **Nền Tảng Quản Lý Kế Thừa & Duy Trì Tính Liên Tục Của Tài Sản (Digital & Physical Continuity Platform)**  
> Phát triển theo phương pháp luận **Spec-Driven Development (SDD)** & **Clean Architecture**.

---

## 🏛️ Tổng Quan Kiến Trúc Monorepo

```text
asseta-monorepo/
├── .github/workflows/       # CI/CD pipelines (Constitution check, Docker, tests)
├── .sdd/                    # SDD Single Source of Truth (Hiến pháp, Ràng buộc, 5 MVP Specs)
├── .agents/                 # AI Agents Persona, Decision Framework, Rules & Memory
├── backend/                 # ASP.NET Core 8 Web API (Clean Architecture)
├── frontend-web/            # React 18+ (Vite + TypeScript + TailwindCSS)
├── mobile-app/              # Flutter 3.x App (Clean Architecture + BLoC)
├── docker/                  # Docker Compose (PostgreSQL 16 + pgcrypto, Redis)
├── docs/adr/                # Architecture Decision Records
└── README.md
```

---

## 🚀 5 Tính Năng Cốt Lõi (MVP Modules)

1. **`feat-01-continuity-map`** *(v1.0.0 APPROVED)*: Bản đồ trực quan hóa tài sản & sơ đồ kế thừa liên tục.
2. **`feat-02-action-cards`**: Thẻ hướng dẫn hành động khẩn cấp từng bước khi xảy ra biến cố.
3. **`feat-03-trusted-people`**: Mạng lưới người được ủy thác và ma trận phân quyền.
4. **`feat-04-continuity-plan`**: Kế hoạch duy trì liên tục và kịch bản ứng phó rủi ro.
5. **`feat-05-safe-activation`**: Giao thức kích hoạt an toàn với cơ chế Time-lock và Dead-man Switch.

---

## 🛠️ Hướng Dẫn Chạy Dự Án (Quickstart)

### 1. Khởi chạy Local Infrastructure (Docker)
```bash
cd docker
docker compose up -d
```
- PostgreSQL: `localhost:5432` (User/Pass: `postgres/postgrespassword`, DB: `asseta_db`)
- Redis: `localhost:6379`

### 2. Khởi chạy Backend (.NET 8)
```bash
cd backend
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Asseta.Api
```
- API Endpoint: `http://localhost:5000`
- Swagger/Info: `http://localhost:5000/api/ContinuityMap`

### 3. Khởi chạy Frontend Web (React + Vite)
```bash
cd frontend-web
npm install
npm run dev
```
- Web App: `http://localhost:5173`

### 4. Khởi chạy Mobile App (Flutter)
```bash
cd mobile-app
flutter pub get
flutter test
flutter run
```

---

## 📜 Quy Chuẩn SDD & Agent Decision Framework

Mọi thay đổi mã nguồn bắt buộc phải tuân theo tài liệu đặc tả tại `.sdd/specs/`.  
Agent và lập trình viên tuân thủ nghiêm ngặt **Decision Framework** được quy định tại [`.agents/AGENTS.md`](.agents/AGENTS.md).

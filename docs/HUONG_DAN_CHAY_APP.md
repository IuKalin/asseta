# Hướng Dẫn Khởi Chạy Hệ Thống Asseta Monorepo

Tài liệu này hướng dẫn chi tiết từng bước bằng các câu lệnh terminal để khởi chạy toàn bộ hệ sinh thái **Asseta** (Hạ tầng Docker, Backend .NET 8, Frontend Web React, và Mobile App Flutter).

---

## 1. Yêu Cầu Môi Trường (Prerequisites)

Trước khi bắt đầu, hãy đảm bảo máy tính đã cài đặt các công cụ sau:

| Công cụ | Phiên bản tối thiểu | Lệnh kiểm tra |
| :--- | :--- | :--- |
| **Git** | 2.x+ | `git --version` |
| **Docker Desktop** | 24.x+ (hỗ trợ Docker Compose) | `docker --version` |
| **.NET SDK** | .NET 8.0 LTS | `dotnet --version` |
| **Node.js & npm** | Node >= 18.x, npm >= 9.x | `node -v` và `npm -v` |
| **Flutter SDK** | Flutter 3.x (Dart 3.x) | `flutter --version` |

---

## 2. Bản Đồ Dịch Vụ & Cổng Mạng (Port Mapping)

| Thành phần | Công nghệ | Cổng mặc định | Địa chỉ truy cập / Endpoint |
| :--- | :--- | :--- | :--- |
| **PostgreSQL** | Docker Container | `5432` | `localhost:5432` (DB: `asseta_db`) |
| **Redis** | Docker Container | `6379` | `localhost:6379` |
| **Backend API** | .NET 8 LTS Core | `5000` | `http://localhost:5000/api/v1` |
| **Swagger UI** | OpenApi Spec | `5000` | `http://localhost:5000/swagger` |
| **Frontend Web** | React 18 + Vite | `5173` | `http://localhost:5173` |
| **Mobile App** | Flutter 3.x | Đa nền tảng | Windows App / Chrome / Android / iOS |

---

## 3. Các Bước Khởi Chạy Chi Tiết Bằng Terminal

Mở các cửa sổ Terminal (PowerShell hoặc Bash) riêng biệt cho từng thành phần bên dưới:

```
Terminal 1: Docker Database & Cache (Chạy ngầm)
Terminal 2: Backend API Server (.NET 8)
Terminal 3: Frontend Web Client (React + Vite)
Terminal 4: Mobile App (Flutter)
```

---

### Bước 1: Khởi động Hạ tầng Cơ sở Dữ liệu (Docker: PostgreSQL & Redis)

Mở **Terminal 1**, điều hướng về thư mục gốc `asseta-monorepo` và chạy lệnh:

```powershell
# Di chuyển đến thư mục gốc monorepo
cd c:\DevFlutter\asseta-monorepo

# Khởi động PostgreSQL 16 và Redis 7 chạy nền
docker compose -f docker/docker-compose.yml up -d postgres redis

# Kiểm tra trạng thái các container đang chạy
docker ps
```

> **Kết quả mong đợi:** Cả hai container `asseta-postgres` (port `5432`) và `asseta-redis` (port `6379`) đều ở trạng thái `Up (healthy)`.

---

### Bước 2: Cập Nhật Database & Chạy Backend API Server (.NET 8)

Mở **Terminal 2**, điều hướng vào thư mục `backend`:

```powershell
# 1. Di chuyển vào thư mục backend
cd c:\DevFlutter\asseta-monorepo\backend

# 2. Khôi phục các thư viện NuGet phụ thuộc
dotnet restore

# 3. Đồng bộ và cập nhật Migration vào database PostgreSQL (19 bảng)
dotnet ef database update --project src/Asseta.Infrastructure --startup-project src/Asseta.Api

# 4. Khởi chạy máy chủ Backend API trên cổng 5000
dotnet run --project src/Asseta.Api --urls=http://localhost:5000
```

> **Xác thực máy chủ Backend hoạt động:**
>
> - Kiểm tra Health Check: Mở trình duyệt truy cập `http://localhost:5000/` &rarr; Nhận JSON: `{"service":"Asseta.Api","status":"Healthy"}`.
> - Khám phá API tương tác: Truy cập `http://localhost:5000/swagger` để xem toàn bộ tài liệu API từ Module 1 đến Module 5.

---

### Bước 3: Khởi chạy Ứng dụng Frontend Web (React + Vite)

Mở **Terminal 3**, điều hướng vào thư mục `frontend-web`:

```powershell
# 1. Di chuyển vào thư mục frontend-web
cd c:\DevFlutter\asseta-monorepo\frontend-web

# 2. Cài đặt các thư viện npm (chỉ cần chạy lần đầu hoặc khi cập nhật package)
npm install

# 3. Khởi chạy máy chủ phát triển Vite
npm run dev
```

> **Truy cập ứng dụng Web:**
>
> - Terminal sẽ hiển thị đường link cục bộ: `http://localhost:5173/`.
> - Nhấp vào đường link để mở ứng dụng Web Asseta trên trình duyệt, sẵn sàng tương tác với Backend `:5000`.

---

### Bước 4: Khởi chạy Ứng dụng Mobile App (Flutter)

Mở **Terminal 4**, điều hướng vào thư mục `mobile-app`:

```powershell
# 1. Di chuyển vào thư mục mobile-app
cd c:\DevFlutter\asseta-monorepo\mobile-app

# 2. Tải toàn bộ packages Flutter / Dart
khi 
# 3. Liệt kê danh sách các thiết bị / môi trường có thể chạy
flutter devices
```

Tùy theo nhu cầu trải nghiệm, bạn chọn 1 trong các chế độ chạy sau:

#### Cách 4.1: Chạy trực tiếp dưới dạng Ứng dụng Desktop Windows (Khuyên dùng - Nhanh nhất)

```powershell
flutter run -d windows
```

#### Cách 4.2: Chạy trên trình duyệt Web (Chrome)

```powershell
flutter run -d chrome
```

#### Cách 4.3: Chạy trên Android Emulator hoặc Máy Thật

1. Khởi động thiết bị giả lập Android từ Android Studio (hoặc cắm cáp máy thật và bật chế độ USB Debugging).
2. Kiểm tra mã thiết bị bằng `flutter devices`.
3. Khởi chạy:

```powershell
flutter run -d <device_id>
```

> **Lưu ý cấu hình IP cho Android Emulator:**  
> Nếu chạy trên Android Emulator, mạng nội bộ của emulator trỏ về máy tính host qua địa chỉ `10.0.2.2`. Bạn có thể cập nhật `baseUrl` trong file [main.dart](file:///c:/DevFlutter/asseta-monorepo/mobile-app/lib/main.dart) thành:
>
> ```dart
> baseUrl: 'http://10.0.2.2:5000/api/v1'
> ```

---

## 4. Các Lệnh Kiểm Thử Tự Động Toàn Bộ Monorepo (Test Suite)

Asseta tuân thủ chính sách **Zero Mock Data** với 100% dữ liệu mã hóa thực tế. Bạn có thể chạy các bộ kiểm thử tự động bất kỳ lúc nào:

### 1. Kiểm thử Backend Unit Tests (.NET 8 - 96 tests)

```powershell
cd c:\DevFlutter\asseta-monorepo\backend
dotnet test tests/Asseta.UnitTests
```

### 2. Kiểm thử Frontend Web Unit Tests (Vitest - 26 tests)

```powershell
cd c:\DevFlutter\asseta-monorepo\frontend-web
npm test
```

### 3. Kiểm thử Mobile App Unit Tests (Flutter - 51 tests)

```powershell
cd c:\DevFlutter\asseta-monorepo\mobile-app
flutter test
```

### 4. Kiểm thử Web-App E2E Integration Test (Gọi mạng thực tế tới Backend :5000)

*Yêu cầu Backend API `:5000` đang chạy ở Bước 2.*

```powershell
cd c:\DevFlutter\asseta-monorepo\frontend-web
npx vitest run src/__integration_tests__/webAppE2EIntegration.test.ts
```

> **Kết quả mong đợi:** Toàn bộ **178 / 178 tests** đều đạt trạng thái **PASS (100%)**.

---

## 5. Hướng Dẫn Tắt / Dừng Hệ Thống (Shutdown)

Khi hoàn tất phiên làm việc, bạn có thể tắt các dịch vụ theo các bước:

1. **Dừng Frontend Web & Backend API & Mobile:**
   - Nhấn `Ctrl + C` trên các cửa sổ terminal tương ứng.
2. **Dừng hạ tầng cơ sở dữ liệu Docker:**

```powershell
cd c:\DevFlutter\asseta-monorepo
docker compose -f docker/docker-compose.yml down
```

*(Nếu muốn xóa cả volume dữ liệu để làm sạch hoàn toàn: thêm cờ `-v`: `docker compose -f docker/docker-compose.yml down -v`)*

---

## 6. Xử Lý Các Vấn Đề Thường Gặp (Troubleshooting)

| Vấn đề gặp phải | Nguyên nhân khả dĩ | Cách khắc phục |
| :--- | :--- | :--- |
| **Lỗi cổng 5000 bị chiếm dụng** (`Address already in use`) | Tiến trình backend cũ vẫn đang chạy ngầm | Trong PowerShell chạy lệnh tìm và hủy tiến trình: <br>`Get-Process -Id (Get-NetTCPConnection -LocalPort 5000).OwningProcess \| Stop-Process -Force` |
| **Lỗi kết nối PostgreSQL** (`Failed to connect to 127.0.0.1:5432`) | Docker container chưa khởi động hoặc chưa sẵn sàng | Chạy `docker ps` kiểm tra container. Nếu chưa có, chạy lại `docker compose -f docker/docker-compose.yml up -d postgres`. |
| **Lỗi thiếu gói .NET EF Tools** | Máy chưa cài đặt công cụ EF Core toàn cục | Chạy lệnh: `dotnet tool install --global dotnet-ef` |
| **Lỗi `flutter: command not found`** | Chưa đưa Flutter SDK vào biến môi trường PATH | Thêm thư mục `flutter/bin` vào biến môi trường hệ thống `Path`. |
| **CORS Error khi gọi API từ Web** | Backend chặn origin | `Asseta.Api` đã bật cấu hình `AllowAll` trong `Program.cs`. Đảm bảo backend đã khởi động đúng cổng 5000. |

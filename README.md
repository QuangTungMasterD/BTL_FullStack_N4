# Course Management System - Fullstack Microservices

## Kiến Trúc

### Backend Services
- **API Gateway** (Port 5000)
  - JWT authentication
  - Rate limiting (120 req/min)
  - Path-based routing
  - Swagger UI

- **StudentAttendanceService** (Port 5001)
  - Quản lý hồ sơ học viên
  - Điểm danh (check-in)
  - Đăng ký khóa học (enrollment)
  - Kết quả học tập (learning results)
  - Tính toán % chuyên cần

- **AuthService** (Port 5002)
  - JWT token generation
  - Token validation
  - Shared secret key

- **CourseScheduleService** (Port 5003)
  - Quản lý lịch học
  - Quản lý khóa học

### Frontend
- **VueJS 3 + Vuetify 3** (Port 3000)
  - Pinia state management
  - Vue Router navigation
  - Axios HTTP client
  - Responsive layout

### Database
- **SQL Server** (Port 1433)
  - Database per service architecture
  - StudentAttendanceDb
  - CourseScheduleDb

## Yêu Cầu Chức Năng

### ✅ Hồ Sơ Học Viên
- [x] Thông tin cá nhân (Họ tên, Email)
- [x] Lịch sử khóa học đã/đang học
- [x] Kết quả học tập

### ✅ Đăng Ký Khóa Học
- [x] Đăng ký khóa học mới
- [x] Quản lý trạng thái (Active/Completed/Dropped)
- [x] Endpoint hoàn thành khóa học

### ✅ Điểm Danh
- [x] Check-in theo buổi học
- [x] Báo cáo điểm danh toàn diện
- [x] Tính % chuyên cần

### ✅ Nhập Điểm Kiểm Tra
- [x] Nhập điểm giữa kỳ (Midterm)
- [x] Nhập điểm cuối kỳ (Final)
- [x] Tính xếp loại (A, B, C, D, F)

## Yêu Cầu Kỹ Thuật

### ✅ API Gateway
- [x] ASP.NET Core (YARP)
- [x] JWT authentication
- [x] Rate limiting

### ✅ Frontend
- [x] VueJS 3
- [x] Vuetify 3
- [x] Pinia state management
- [x] Vue Router

### ✅ Database
- [x] SQL Server
- [x] Database per service

### ✅ Authentication
- [x] JWT Bearer Token
- [x] Shared secret key

### ✅ API Documentation
- [x] Swagger/OpenAPI per service

### ✅ Containerization
- [x] Docker containers per service
- [x] docker-compose orchestration

## Chạy Ứng Dụng

### Với Docker Compose
```bash
docker-compose up -d
```

Services sẽ chạy tại:
- Frontend: http://localhost:3000
- API Gateway: http://localhost:5000
- StudentAttendanceService: http://localhost:5001
- AuthService: http://localhost:5002
- CourseScheduleService: http://localhost:5003

### Phát Triển Cục Bộ

#### Backend
```bash
cd backend/StudentAttendanceService
dotnet restore
dotnet run
```

#### Frontend
```bash
cd front-end
npm install
npm run dev
```

## API Endpoints

### Auth Service
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh` - Refresh token

### Student Attendance Service
- `GET /api/students` - Lấy danh sách học viên
- `GET /api/students/{id}/profile` - Hồ sơ học viên
- `POST /api/enrollments` - Đăng ký khóa học
- `PUT /api/enrollments/{id}/complete` - Hoàn thành khóa học
- `POST /api/attendances/checkin` - Điểm danh
- `GET /api/attendances/report/{studentId}` - Báo cáo điểm danh
- `POST /api/learningresults` - Nhập điểm
- `GET /api/learningresults/student/{studentId}` - Lấy điểm học viên

## Cấu Hình JWT
```json
{
  "Jwt": {
    "Key": "VerySecretSharedJwtKey123!",
    "Issuer": "AttendanceAuth",
    "ExpirationMinutes": 60
  }
}
```

## Cơ Sở Dữ Liệu

### StudentAttendanceService (Connection String)
```
Server=sqlserver;Database=StudentAttendanceDb;User Id=sa;Password=Shared@Password123;
```

### Tables
- Students (Id, FullName, Email)
- Enrollments (Id, StudentId, CourseScheduleId, EnrollmentDate, Status)
- Attendances (Id, StudentId, CheckInTime, Status)
- LearningResults (Id, EnrollmentId, ExamType, Score, Grade, RecordedDate)

## Lưu Ý
- Tất cả services chia sẻ JWT key cho validation
- Rate limiting: 120 requests/phút per IP
- Database migration auto-setup qua EF Core
- CORS enabled cho frontend calls

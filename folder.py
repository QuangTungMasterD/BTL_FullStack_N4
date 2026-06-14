import os
import json
import shutil
from pathlib import Path

# Đường dẫn đến thư mục backend của bạn
BACKEND_PATH = Path(r"C:\Full_Stack\BTL_FullStack_N4\backend")

# Danh sách các file CẦN CÓ sau khi phát triển
REQUIRED_FILES = {
    # AuthService
    "AuthService/Program.cs": True,
    "AuthService/appsettings.json": True,
    "AuthService/Dockerfile": True,
    "AuthService/AuthService.csproj": True,
    "AuthService/Controllers/AuthController.cs": True,
    "AuthService/Data/AppDbContext.cs": True,
    "AuthService/Models/User.cs": True,
    "AuthService/DTOs/LoginRequestDto.cs": True,
    "AuthService/DTOs/LoginResponseDto.cs": True,
    "AuthService/DTOs/RegisterRequestDto.cs": True,
    "AuthService/DTOs/RegisterResponseDto.cs": True,
    "AuthService/DTOs/UserInfoDto.cs": True,
    "AuthService/Services/AuthService.cs": True,
    "AuthService/Services/IAuthService.cs": True,
    
    # StudentAttendanceService - Models
    "StudentAttendanceService/Models/Student.cs": True,
    "StudentAttendanceService/Models/Lecturer.cs": True,
    "StudentAttendanceService/Models/Course.cs": True,
    "StudentAttendanceService/Models/Department.cs": True,
    "StudentAttendanceService/Models/Enrollment.cs": True,
    "StudentAttendanceService/Models/Attendance.cs": True,
    "StudentAttendanceService/Models/LearningResult.cs": True,
    "StudentAttendanceService/Models/Announcement.cs": True,
    
    # StudentAttendanceService - DTOs Student
    "StudentAttendanceService/DTOs/Student/StudentStatsDto.cs": True,
    "StudentAttendanceService/DTOs/Student/CurrentCourseDto.cs": True,
    "StudentAttendanceService/DTOs/Student/UpcomingClassDto.cs": True,
    "StudentAttendanceService/DTOs/Student/RecentGradeDto.cs": True,
    "StudentAttendanceService/DTOs/Student/AttendanceSummaryDto.cs": True,
    "StudentAttendanceService/DTOs/Student/GpaHistoryDto.cs": True,
    "StudentAttendanceService/DTOs/Student/CourseGradeDto.cs": True,
    "StudentAttendanceService/DTOs/Student/GradeDetailDto.cs": True,
    
    # StudentAttendanceService - DTOs Lecturer
    "StudentAttendanceService/DTOs/Lecturer/LecturerCourseDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/LecturerStudentDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/TodayScheduleDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/RecentAttendanceDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/TopStudentDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/AttendanceStudentDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/AttendanceHistoryDto.cs": True,
    "StudentAttendanceService/DTOs/Lecturer/StudentGradeDto.cs": True,
    
    # StudentAttendanceService - DTOs Admin
    "StudentAttendanceService/DTOs/Admin/AdminStatisticsDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/EnrollmentTrendDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/DepartmentStatDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/RecentActivityDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/CreateStudentAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/UpdateStudentAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/CreateLecturerAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/UpdateLecturerAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/CreateCourseAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/UpdateCourseAdminDto.cs": True,
    "StudentAttendanceService/DTOs/Admin/CourseStatDto.cs": True,
    
    # StudentAttendanceService - Controllers
    "StudentAttendanceService/Controllers/StudentController.cs": True,
    "StudentAttendanceService/Controllers/LecturerController.cs": True,
    "StudentAttendanceService/Controllers/AdminController.cs": True,
    
    # StudentAttendanceService - Core
    "StudentAttendanceService/Data/AppDbContext.cs": True,
    "StudentAttendanceService/Program.cs": True,
    "StudentAttendanceService/appsettings.json": True,
    "StudentAttendanceService/Dockerfile": True,
    "StudentAttendanceService/StudentAttendanceService.csproj": True,
}

# Danh sách các file CŨ cần XÓA (không bao gồm gateway vì gateway đang ở front-end)
OLD_FILES_TO_REMOVE = [
    # Controllers cũ
    "StudentAttendanceService/Controllers/AttendancesController.cs",
    "StudentAttendanceService/Controllers/EnrollmentsController.cs",
    "StudentAttendanceService/Controllers/LearningResultsController.cs",
    "StudentAttendanceService/Controllers/StudentsController.cs",
    
    # DTOs cũ
    "StudentAttendanceService/DTOs/AttendanceDto.cs",
    "StudentAttendanceService/DTOs/AttendanceReportDto.cs",
    "StudentAttendanceService/DTOs/CheckInDto.cs",
    "StudentAttendanceService/DTOs/CreateEnrollmentDto.cs",
    "StudentAttendanceService/DTOs/CreateLearningResultDto.cs",
    "StudentAttendanceService/DTOs/CreateStudentDto.cs",
    "StudentAttendanceService/DTOs/EnrollmentDto.cs",
    "StudentAttendanceService/DTOs/LearningResultDto.cs",
    "StudentAttendanceService/DTOs/StudentDto.cs",
    "StudentAttendanceService/DTOs/StudentProfileDto.cs",
    
    # Repositories cũ
    "StudentAttendanceService/Repositories/AttendanceRepository.cs",
    "StudentAttendanceService/Repositories/EnrollmentRepository.cs",
    "StudentAttendanceService/Repositories/LearningResultRepository.cs",
    "StudentAttendanceService/Repositories/StudentRepository.cs",
    "StudentAttendanceService/Repositories/IAttendanceRepository.cs",
    "StudentAttendanceService/Repositories/IEnrollmentRepository.cs",
    "StudentAttendanceService/Repositories/ILearningResultRepository.cs",
    "StudentAttendanceService/Repositories/IStudentRepository.cs",
    
    # Services cũ
    "StudentAttendanceService/Services/AttendanceService.cs",
    "StudentAttendanceService/Services/EnrollmentService.cs",
    "StudentAttendanceService/Services/LearningResultService.cs",
    "StudentAttendanceService/Services/StudentService.cs",
    "StudentAttendanceService/Services/IAttendanceService.cs",
    "StudentAttendanceService/Services/IEnrollmentService.cs",
    "StudentAttendanceService/Services/ILearningResultService.cs",
    "StudentAttendanceService/Services/IStudentService.cs",
    "StudentAttendanceService/Services/CourseScheduleServiceClient.cs",
    "StudentAttendanceService/Services/ICourseScheduleServiceClient.cs",
    
    # Mappings cũ
    "StudentAttendanceService/Mappings/AutoMapperProfile.cs",
    
    # Thư mục TestEf
    "StudentAttendanceService/TestEf/",
]

# Danh sách thư mục cần có
REQUIRED_FOLDERS = [
    "AuthService",
    "AuthService/Controllers",
    "AuthService/Data",
    "AuthService/Models",
    "AuthService/DTOs",
    "AuthService/Services",
    "StudentAttendanceService",
    "StudentAttendanceService/Models",
    "StudentAttendanceService/Controllers",
    "StudentAttendanceService/Data",
    "StudentAttendanceService/DTOs",
    "StudentAttendanceService/DTOs/Student",
    "StudentAttendanceService/DTOs/Lecturer",
    "StudentAttendanceService/DTOs/Admin",
]

def check_structure():
    """Kiểm tra cấu trúc và trả về kết quả"""
    print("=" * 80)
    print("KIỂM TRA CẤU TRÚC BACK-END")
    print("=" * 80)
    print(f"Đường dẫn kiểm tra: {BACKEND_PATH}")
    print()
    
    if not BACKEND_PATH.exists():
        print(f"❌ LỖI: Không tìm thấy thư mục backend tại {BACKEND_PATH}")
        return False, None, None, None
    
    print("✅ Thư mục backend tồn tại")
    print()
    
    # 1. Kiểm tra các file cần có
    print("-" * 80)
    print("1. KIỂM TRA FILE CẦN CÓ:")
    print("-" * 80)
    
    missing_files = []
    existing_files = []
    
    for file_path in REQUIRED_FILES.keys():
        full_path = BACKEND_PATH / file_path
        if full_path.exists():
            existing_files.append(file_path)
            print(f"   ✅ {file_path}")
        else:
            missing_files.append(file_path)
            print(f"   ❌ THIẾU: {file_path}")
    
    print()
    print(f"   📊 Tổng số file cần có: {len(REQUIRED_FILES)}")
    print(f"   📁 Đã có: {len(existing_files)}")
    print(f"   ❌ Thiếu: {len(missing_files)}")
    print()
    
    # 2. Kiểm tra các file cũ cần xóa
    print("-" * 80)
    print("2. FILE CŨ CẦN XÓA:")
    print("-" * 80)
    
    old_files_exist = []
    for file_path in OLD_FILES_TO_REMOVE:
        full_path = BACKEND_PATH / file_path
        if full_path.exists():
            old_files_exist.append(file_path)
            print(f"   ⚠️ {file_path}")
    
    if not old_files_exist:
        print("   ✅ Không có file cũ nào")
    
    print()
    print(f"   📊 Số file cũ: {len(old_files_exist)}")
    print()
    
    # 3. Kiểm tra cấu trúc thư mục
    print("-" * 80)
    print("3. KIỂM TRA THƯ MỤC:")
    print("-" * 80)
    
    missing_folders = []
    for folder in REQUIRED_FOLDERS:
        full_path = BACKEND_PATH / folder
        if full_path.exists() and full_path.is_dir():
            print(f"   ✅ {folder}/")
        else:
            missing_folders.append(folder)
            print(f"   ❌ THIẾU: {folder}/")
    
    print()
    print(f"   📊 Số thư mục thiếu: {len(missing_folders)}")
    print()
    
    return True, missing_files, old_files_exist, missing_folders

def auto_fix(missing_folders, old_files_exist):
    """Tự động tạo thư mục và xóa file cũ"""
    print("\n" + "=" * 80)
    print("AUTO FIX - TỰ ĐỘNG SỬA LỖI")
    print("=" * 80)
    
    # Tạo thư mục thiếu
    if missing_folders:
        print("\n📁 ĐANG TẠO THƯ MỤC THIẾU...")
        for folder in missing_folders:
            folder_path = BACKEND_PATH / folder
            folder_path.mkdir(parents=True, exist_ok=True)
            print(f"   ✅ Đã tạo: {folder}/")
    else:
        print("\n📁 Không cần tạo thêm thư mục nào!")
    
    # Xóa file cũ
    if old_files_exist:
        print("\n🗑️ ĐANG XÓA FILE CŨ...")
        for file_path in old_files_exist:
            full_path = BACKEND_PATH / file_path
            try:
                if full_path.is_dir():
                    shutil.rmtree(full_path)
                    print(f"   ✅ Đã xóa thư mục: {file_path}")
                else:
                    full_path.unlink()
                    print(f"   ✅ Đã xóa file: {file_path}")
            except Exception as e:
                print(f"   ❌ Lỗi khi xóa {file_path}: {e}")
    else:
        print("\n🗑️ Không có file cũ cần xóa!")
    
    print("\n✅ HOÀN TẤT AUTO FIX!")
    print("=" * 80)

def create_missing_file(file_path, content):
    """Tạo file mới với nội dung"""
    full_path = BACKEND_PATH / file_path
    full_path.parent.mkdir(parents=True, exist_ok=True)
    with open(full_path, "w", encoding="utf-8") as f:
        f.write(content)
    print(f"   ✅ Đã tạo: {file_path}")

def create_iauthservice():
    """Tạo file IAuthService.cs"""
    content = '''using System.Threading.Tasks;
using AuthService.Models;

namespace AuthService.Services
{
    public interface IAuthService
    {
        string GenerateToken(int userId, string email, string fullName, string role);
    }
}'''
    create_missing_file("AuthService/Services/IAuthService.cs", content)

def create_missing_files(missing_files):
    """Tạo các file còn thiếu (nếu có nội dung mẫu)"""
    print("\n" + "=" * 80)
    print("📝 TẠO FILE CÒN THIẾU")
    print("=" * 80)
    
    for file_path in missing_files:
        if "IAuthService.cs" in file_path:
            create_iauthservice()
        else:
            print(f"   ⚠️ Chưa có nội dung mẫu cho: {file_path}")

def main():
    # Kiểm tra cấu trúc
    success, missing_files, old_files_exist, missing_folders = check_structure()
    
    if not success:
        return
    
    # Hỏi người dùng có muốn tự động sửa không
    print("\n" + "=" * 80)
    print("🔧 TÙY CHỌN XỬ LÝ")
    print("=" * 80)
    print("1. Tự động tạo thư mục và xóa file cũ")
    print("2. Chỉ xem báo cáo, không tự động sửa")
    
    choice = input("\nNhập lựa chọn (1/2): ").strip()
    
    if choice == "1":
        # Tự động tạo thư mục và xóa file cũ
        auto_fix(missing_folders, old_files_exist)
        
        # Tạo các file còn thiếu (chỉ IAuthService.cs hiện tại)
        if missing_files:
            create_missing_files(missing_files)
        
        # Kiểm tra lại sau khi fix
        print("\n" + "=" * 80)
        print("KIỂM TRA LẠI SAU KHI FIX")
        print("=" * 80)
        success2, missing_files2, old_files_exist2, missing_folders2 = check_structure()
        
        if not missing_files2 and not missing_folders2:
            print("\n🎉 CHÚC MỪNG! CẤU TRÚC BACK-END ĐÃ HOÀN CHỈNH!")
        else:
            print("\n⚠️ VẪN CÒN THIẾU FILE, VUI LÒNG KIỂM TRA LẠI!")
            if missing_files2:
                print(f"   Thiếu file: {missing_files2}")
            if missing_folders2:
                print(f"   Thiếu thư mục: {missing_folders2}")
    else:
        # Chỉ hiển thị báo cáo
        print("\n📋 BÁO CÁO TỔNG HỢP:")
        print(f"   - File cần tạo: {len(missing_files)}")
        print(f"   - File cần xóa: {len(old_files_exist)}")
        print(f"   - Thư mục cần tạo: {len(missing_folders)}")
        
        if missing_files:
            print("\n   FILE CẦN TẠO:")
            for f in missing_files:
                print(f"      - {f}")
        
        if old_files_exist:
            print("\n   FILE CẦN XÓA:")
            for f in old_files_exist:
                print(f"      - {f}")
        
        if missing_folders:
            print("\n   THƯ MỤC CẦN TẠO:")
            for f in missing_folders:
                print(f"      - {f}")
    
    print("\n💡 HƯỚNG DẪN:")
    print("   - Chạy lại script nếu cần kiểm tra lại")
    print("   - Đảm bảo các service (AuthService, StudentAttendanceService) có thể build được")

if __name__ == "__main__":
    main()
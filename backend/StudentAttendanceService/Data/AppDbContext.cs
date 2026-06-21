using Microsoft.EntityFrameworkCore;
using StudentAttendanceService.Models;

namespace StudentAttendanceService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Lecturer> Lecturers { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<LearningResult> LearningResults { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Specialization> Specializations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Lecturer)
                .WithMany(l => l.Courses)
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // THÊM: Cấu hình quan hệ Course - Specialization
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Specialization)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SpecializationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany()
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LearningResult>()
                .HasOne(r => r.Enrollment)
                .WithMany(e => e.LearningResults)
                .HasForeignKey(r => r.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho Specialization
            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.HasIndex(e => e.SpecializationName).IsUnique();
                entity.Property(e => e.SpecializationName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descrt).HasMaxLength(500);
            });

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Email = "admin@example.com", 
                    Password = "admin123", 
                    FullName = "Trần Văn Admin", 
                    Role = "ADMIN",
                    Phone = "0912345678",
                    Faculty = "Phòng Đào tạo",
                    Title = "Quản trị viên",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Code = "CNTT", Name = "Công nghệ thông tin", StudentCount = 486, LecturerCount = 35, Head = "PGS.TS Trần Văn X" },
                new Department { Id = 2, Code = "QTKD", Name = "Quản trị kinh doanh", StudentCount = 325, LecturerCount = 28, Head = "TS. Nguyễn Thị Y" },
                new Department { Id = 3, Code = "NNKT", Name = "Ngôn ngữ Anh", StudentCount = 218, LecturerCount = 20, Head = "ThS. Lê Văn Z" }
            );

            // Seed Specializations
            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { Id = 1, SpecializationName = "Khoa học máy tính", Descrt = "Chuyên ngành nghiên cứu và phát triển phần mềm", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { Id = 2, SpecializationName = "Kỹ thuật phần mềm", Descrt = "Chuyên ngành phát triển ứng dụng và hệ thống", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { Id = 3, SpecializationName = "Quản trị kinh doanh", Descrt = "Chuyên ngành quản lý và điều hành doanh nghiệp", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { Id = 4, SpecializationName = "Ngôn ngữ Anh", Descrt = "Chuyên ngành ngôn ngữ và văn hóa Anh", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Specialization { Id = 5, SpecializationName = "Trí tuệ nhân tạo", Descrt = "Chuyên ngành AI và Machine Learning", IsActive = true, CreatedAt = DateTime.UtcNow }
            );

            // Seed Lecturers
            modelBuilder.Entity<Lecturer>().HasData(
                new Lecturer { Id = 1, LecturerId = "GV001", FullName = "PGS.TS Trần Văn X", Email = "tranvanx@university.edu.vn", Phone = "0912345678", Faculty = "CNTT", Title = "Phó Giáo sư", Specialization = "Khoa học máy tính", Status = "Active" },
                new Lecturer { Id = 2, LecturerId = "GV002", FullName = "TS. Nguyễn Thị Y", Email = "nguyenthiy@university.edu.vn", Phone = "0912345679", Faculty = "QTKD", Title = "Tiến sĩ", Specialization = "Quản trị chiến lược", Status = "Active" },
                new Lecturer { Id = 3, LecturerId = "GV003", FullName = "ThS. Lê Văn Z", Email = "levanz@university.edu.vn", Phone = "0912345680", Faculty = "NNKT", Title = "Thạc sĩ", Specialization = "Ngôn ngữ học", Status = "Active" }
            );

            // Seed Courses (cập nhật thêm SpecializationId)
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Code = "CS101", Name = "Nhập môn lập trình", Credits = 3, Faculty = "CNTT", Semester = "Fall 2024", LecturerId = 1, SpecializationId = 1, Schedule = "Thứ 2, 13:30-16:30", Room = "A101", MaxStudents = 60 },
                new Course { Id = 2, Code = "CS201", Name = "Cấu trúc dữ liệu", Credits = 4, Faculty = "CNTT", Semester = "Fall 2024", LecturerId = 1, SpecializationId = 1, Schedule = "Thứ 3, 08:00-11:00", Room = "B202", MaxStudents = 60 },
                new Course { Id = 3, Code = "CS301", Name = "Cơ sở dữ liệu", Credits = 3, Faculty = "CNTT", Semester = "Fall 2024", LecturerId = 2, SpecializationId = 2, Schedule = "Thứ 4, 13:30-16:30", Room = "C303", MaxStudents = 60 }
            );

            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, StudentId = "2021001234", FullName = "Nguyễn Văn A", Email = "nguyenvana@university.edu.vn", Phone = "0901234567", Faculty = "CNTT", Major = "Khoa học máy tính", Class = "K63-CLC", Year = 2021, Status = "Active" },
                new Student { Id = 2, StudentId = "2021001235", FullName = "Trần Thị B", Email = "tranthib@university.edu.vn", Phone = "0901234568", Faculty = "CNTT", Major = "Khoa học máy tính", Class = "K63-CLC", Year = 2021, Status = "Active" },
                new Student { Id = 3, StudentId = "2021001236", FullName = "Lê Văn C", Email = "levanc@university.edu.vn", Phone = "0901234569", Faculty = "QTKD", Major = "Quản trị kinh doanh", Class = "K63-QTKD", Year = 2021, Status = "Active" }
            );

            // Seed Announcements
            modelBuilder.Entity<Announcement>().HasData(
                new Announcement { Id = 1, Title = "Thông báo lịch thi cuối kỳ", Content = "Lịch thi cuối kỳ học kỳ Fall 2024 đã được công bố...", Date = DateTime.UtcNow.AddDays(-5), Author = "Phòng Đào tạo", Priority = "high", TargetRole = "ALL" },
                new Announcement { Id = 2, Title = "Đăng ký học phần học kỳ Spring 2025", Content = "Thời gian đăng ký học phần bắt đầu từ ngày 15/12/2024...", Date = DateTime.UtcNow.AddDays(-8), Author = "Phòng Đào tạo", Priority = "medium", TargetRole = "STUDENT" }
            );
        }
    }
}
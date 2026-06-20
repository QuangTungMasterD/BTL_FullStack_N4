using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceService.Data;
using StudentAttendanceService.DTOs.Admin;
using StudentAttendanceService.Models;

namespace StudentAttendanceService.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Dashboard Statistics

        // GET: api/admin/statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalLecturers = await _context.Lecturers.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalDepartments = await _context.Departments.CountAsync();
            
            var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Active");
            
            var attendances = await _context.Attendances.ToListAsync();
            var averageAttendance = attendances.Count > 0 
                ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                : 0;
            
            var completedEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Completed");
            var completionRate = activeEnrollments + completedEnrollments > 0
                ? Math.Round((double)completedEnrollments / (activeEnrollments + completedEnrollments) * 100, 0)
                : 0;

            var stats = new AdminStatisticsDto
            {
                TotalStudents = totalStudents,
                TotalLecturers = totalLecturers,
                TotalCourses = totalCourses,
                TotalDepartments = totalDepartments,
                ActiveEnrollments = activeEnrollments,
                AverageAttendance = averageAttendance,
                CompletionRate = completionRate,
                StudentGrowth = 12.5
            };

            return Ok(stats);
        }

        // GET: api/admin/enrollment-trend
        [HttpGet("enrollment-trend")]
        public async Task<IActionResult> GetEnrollmentTrend([FromQuery] int year = 2024)
        {
            var months = new[] { "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
            var trend = new List<EnrollmentTrendDto>();
            var random = new Random();

            foreach (var month in months)
            {
                trend.Add(new EnrollmentTrendDto
                {
                    Label = month,
                    Value = random.Next(200, 500)
                });
            }

            return Ok(trend);
        }

        // GET: api/admin/department-stats
        [HttpGet("department-stats")]
        public async Task<IActionResult> GetDepartmentStats()
        {
            var departments = await _context.Departments.ToListAsync();
            var totalStudents = await _context.Students.CountAsync();

            var stats = departments.Select(d => new DepartmentStatDto
            {
                Name = d.Name,
                Count = d.StudentCount,
                Percent = Math.Round((double)d.StudentCount / totalStudents * 100, 1),
                Color = GetDepartmentColor(d.Code)
            }).ToList();

            return Ok(stats);
        }

        // GET: api/admin/recent-activities
        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities()
        {
            var activities = new List<RecentActivityDto>();

            // Get recent enrollments
            var recentEnrollments = await _context.Enrollments
                .OrderByDescending(e => e.EnrollmentDate)
                .Take(5)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();

            foreach (var enrollment in recentEnrollments)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = enrollment.Id,
                    Action = $"Sinh viên {enrollment.Student?.FullName} đăng ký khóa {enrollment.Course?.Name}",
                    CreatedAt = enrollment.EnrollmentDate,
                    Type = "enrollment",
                    UserName = enrollment.Student?.FullName
                });
            }

            // Get recent grade updates
            var recentGrades = await _context.LearningResults
                .OrderByDescending(r => r.RecordedDate)
                .Take(5)
                .Include(r => r.Enrollment)
                .ThenInclude(e => e!.Student)
                .ToListAsync();

            foreach (var grade in recentGrades)
            {
                activities.Add(new RecentActivityDto
                {
                    Id = grade.Id,
                    Action = $"Cập nhật điểm {grade.ExamType} cho {grade.Enrollment?.Student?.FullName}",
                    CreatedAt = grade.RecordedDate,
                    Type = "grade",
                    UserName = grade.Enrollment?.Student?.FullName
                });
            }

            return Ok(activities.OrderByDescending(a => a.CreatedAt).Take(10));
        }

        #endregion

        #region Student Management (CRUD)

        // GET: api/admin/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] string? search, [FromQuery] string? faculty, [FromQuery] string? status)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.FullName.Contains(search) || s.StudentId.Contains(search) || s.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                query = query.Where(s => s.Faculty == faculty);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            var students = await query
                .OrderBy(s => s.StudentId)
                .Select(s => new
                {
                    s.Id,
                    s.StudentId,
                    s.FullName,
                    s.Email,
                    s.Phone,
                    s.Faculty,
                    s.Major,
                    s.Class,
                    s.Status
                })
                .ToListAsync();

            return Ok(new { items = students, total = students.Count });
        }

        // GET: api/admin/students/{id}
        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return Ok(student);
        }

        // POST: api/admin/students
        [HttpPost("students")]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudentAdminDto dto)
        {
            var existing = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == dto.StudentId || s.Email == dto.Email);
            if (existing != null)
                return BadRequest(new { message = "Student ID or Email already exists" });

            var student = new Student
            {
                StudentId = dto.StudentId,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Faculty = dto.Faculty,
                Major = dto.Major,
                Class = dto.Class,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student created successfully", student });
        }

        // PUT: api/admin/students/{id}
        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentAdminDto dto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            student.FullName = dto.FullName;
            student.Email = dto.Email;
            student.Phone = dto.Phone;
            student.Faculty = dto.Faculty;
            student.Major = dto.Major;
            student.Class = dto.Class;
            student.Status = dto.Status;
            student.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Student updated successfully", student });
        }

        // DELETE: api/admin/students/{id}
        [HttpDelete("students/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.StudentId == id);
            if (hasEnrollments)
                return BadRequest(new { message = "Cannot delete student with active enrollments" });

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student deleted successfully" });
        }

        #endregion

        #region Lecturer Management (CRUD)

        // GET: api/admin/lecturers
        [HttpGet("lecturers")]
        public async Task<IActionResult> GetLecturers([FromQuery] string? search, [FromQuery] string? faculty, [FromQuery] string? title)
        {
            var query = _context.Lecturers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.FullName.Contains(search) || l.LecturerId.Contains(search) || l.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                query = query.Where(l => l.Faculty == faculty);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(l => l.Title == title);
            }

            var lecturers = await query
                .Select(l => new
                {
                    l.Id,
                    l.LecturerId,
                    l.FullName,
                    l.Email,
                    l.Phone,
                    l.Faculty,
                    l.Title,
                    l.Specialization,
                    l.Status,
                    Courses = _context.Courses.Count(c => c.LecturerId == l.Id && c.Semester == "Fall 2024")
                })
                .ToListAsync();

            return Ok(new { items = lecturers, total = lecturers.Count });
        }

        // GET: api/admin/lecturers/{id}
        [HttpGet("lecturers/{id}")]
        public async Task<IActionResult> GetLecturerById(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            return Ok(lecturer);
        }

        // GET: api/admin/lecturers/{id}/courses
        [HttpGet("lecturers/{id}/courses")]
        public async Task<IActionResult> GetLecturerCourses(int id)
        {
            var courses = await _context.Courses
                .Where(c => c.LecturerId == id && c.Semester == "Fall 2024")
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Credits,
                    Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active"),
                    c.MaxStudents
                })
                .ToListAsync();

            return Ok(courses);
        }

        // POST: api/admin/lecturers
        [HttpPost("lecturers")]
        public async Task<IActionResult> CreateLecturer([FromBody] CreateLecturerAdminDto dto)
        {
            var existing = await _context.Lecturers.FirstOrDefaultAsync(l => l.LecturerId == dto.LecturerId || l.Email == dto.Email);
            if (existing != null)
                return BadRequest(new { message = "Lecturer ID or Email already exists" });

            var lecturer = new Lecturer
            {
                LecturerId = dto.LecturerId,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Faculty = dto.Faculty,
                Title = dto.Title,
                Specialization = dto.Specialization,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lecturer created successfully", lecturer });
        }

        // PUT: api/admin/lecturers/{id}
        [HttpPut("lecturers/{id}")]
        public async Task<IActionResult> UpdateLecturer(int id, [FromBody] UpdateLecturerAdminDto dto)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            lecturer.FullName = dto.FullName;
            lecturer.Email = dto.Email;
            lecturer.Phone = dto.Phone;
            lecturer.Faculty = dto.Faculty;
            lecturer.Title = dto.Title;
            lecturer.Specialization = dto.Specialization;
            lecturer.Status = dto.Status;
            lecturer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Lecturer updated successfully", lecturer });
        }

        // DELETE: api/admin/lecturers/{id}
        [HttpDelete("lecturers/{id}")]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer == null) return NotFound();

            var hasCourses = await _context.Courses.AnyAsync(c => c.LecturerId == id);
            if (hasCourses)
                return BadRequest(new { message = "Cannot delete lecturer with assigned courses" });

            _context.Lecturers.Remove(lecturer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Lecturer deleted successfully" });
        }

        #endregion

        #region Course Management (CRUD)

        // GET: api/admin/courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses([FromQuery] string? search, [FromQuery] string? faculty, [FromQuery] string? semester)
        {
            var query = _context.Courses
                .Include(c => c.Lecturer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                query = query.Where(c => c.Faculty == faculty);
            }

            if (!string.IsNullOrEmpty(semester))
            {
                query = query.Where(c => c.Semester == semester);
            }

            var courses = await query
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Credits,
                    c.Faculty,
                    c.Semester,
                    Lecturer = c.Lecturer != null ? c.Lecturer.FullName : "Chưa phân công",
                    c.Schedule,
                    c.Room,
                    Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active"),
                    c.MaxStudents
                })
                .ToListAsync();

            return Ok(new { items = courses, total = courses.Count });
        }

        // GET: api/admin/courses/{id}
        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();

            return Ok(course);
        }

        // POST: api/admin/courses
        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseAdminDto dto)
        {
            var existing = await _context.Courses.FirstOrDefaultAsync(c => c.Code == dto.Code);
            if (existing != null)
                return BadRequest(new { message = "Course code already exists" });

            var lecturer = await _context.Lecturers.FindAsync(dto.LecturerId);
            if (lecturer == null)
                return BadRequest(new { message = "Lecturer not found" });

            var course = new Course
            {
                Code = dto.Code,
                Name = dto.Name,
                Credits = dto.Credits,
                Faculty = dto.Faculty,
                Semester = dto.Semester,
                LecturerId = dto.LecturerId,
                Schedule = dto.Schedule,
                Room = dto.Room,
                MaxStudents = dto.MaxStudents,
                CreatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course created successfully", course });
        }

        // PUT: api/admin/courses/{id}
        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdateCourseAdminDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var lecturer = await _context.Lecturers.FindAsync(dto.LecturerId);
            if (lecturer == null)
                return BadRequest(new { message = "Lecturer not found" });

            course.Name = dto.Name;
            course.Credits = dto.Credits;
            course.Faculty = dto.Faculty;
            course.Semester = dto.Semester;
            course.LecturerId = dto.LecturerId;
            course.Schedule = dto.Schedule;
            course.Room = dto.Room;
            course.MaxStudents = dto.MaxStudents;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Course updated successfully", course });
        }

        // DELETE: api/admin/courses/{id}
        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.CourseId == id);
            if (hasEnrollments)
                return BadRequest(new { message = "Cannot delete course with active enrollments" });

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course deleted successfully" });
        }

        #endregion

        #region Attendance Management (Admin)

        // GET: api/admin/attendance/overall
        [HttpGet("attendance/overall")]
        public async Task<IActionResult> GetOverallAttendance([FromQuery] string? semester)
        {
            var attendances = await _context.Attendances.ToListAsync();
            var totalStudents = await _context.Students.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();

            var presentRate = attendances.Count > 0 
                ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                : 0;
            var absentRate = attendances.Count > 0 
                ? Math.Round((double)attendances.Count(a => a.Status == "Absent") / attendances.Count * 100, 0)
                : 0;
            var lateRate = attendances.Count > 0 
                ? Math.Round((double)attendances.Count(a => a.Status == "Late") / attendances.Count * 100, 0)
                : 0;

            return Ok(new
            {
                TotalStudents = totalStudents,
                PresentRate = presentRate,
                AbsentRate = absentRate,
                LateRate = lateRate
            });
        }

        // GET: api/admin/attendance/by-faculty
        [HttpGet("attendance/by-faculty")]
        public async Task<IActionResult> GetAttendanceByFaculty([FromQuery] string? semester)
        {
            var faculties = await _context.Departments.ToListAsync();
            var result = new List<object>();

            foreach (var faculty in faculties)
            {
                var students = await _context.Students.Where(s => s.Faculty == faculty.Name).ToListAsync();
                var studentIds = students.Select(s => s.Id).ToList();
                
                var attendances = await _context.Attendances
                    .Where(a => studentIds.Contains(a.StudentId))
                    .ToListAsync();

                var present = attendances.Count > 0 
                    ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                    : 0;
                var absent = attendances.Count > 0 
                    ? Math.Round((double)attendances.Count(a => a.Status == "Absent") / attendances.Count * 100, 0)
                    : 0;
                var late = attendances.Count > 0 
                    ? Math.Round((double)attendances.Count(a => a.Status == "Late") / attendances.Count * 100, 0)
                    : 0;

                result.Add(new { name = faculty.Name, present, absent, late });
            }

            // Thêm await Task.CompletedTask để loại bỏ warning
            await Task.CompletedTask;
            
            return Ok(result);
        }

        // GET: api/admin/profile
        [HttpGet("profile")]
        public IActionResult GetProfile()  // Bỏ async, không cần await
        {
            // Lấy thông tin từ JWT token claims
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Trả về thông tin từ claims (không cần database)
            var result = new
            {
                FullName = fullName ?? "Admin",
                AdminId = "ADM001",
                Email = email,
                Phone = "0912345678",
                Faculty = "Phòng Đào tạo",
                Title = role ?? "ADMIN",
                Role = role ?? "ADMIN",
                Address = "Hà Nội, Việt Nam"
            };

            return Ok(result);
        }

        // GET: api/admin/attendance/by-course
        [HttpGet("attendance/by-course")]
        public async Task<IActionResult> GetAttendanceByCourse([FromQuery] string? faculty, [FromQuery] string? search)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(faculty))
                query = query.Where(c => c.Faculty == faculty);

            var courses = await query.ToListAsync();
            var result = new List<CourseStatDto>();

            foreach (var course in courses)
            {
                var attendances = await _context.Attendances
                    .Where(a => a.CourseId == course.Id)
                    .ToListAsync();

                var attendanceRate = attendances.Count > 0 
                    ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                    : 0;

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == course.Id && e.Status == "Active")
                    .ToListAsync();

                var grades = await _context.LearningResults
                    .Where(r => r.Enrollment != null && enrollments.Select(e => e.Id).Contains(r.EnrollmentId))
                    .ToListAsync();

                var averageGrade = grades.Count > 0 ? Math.Round(grades.Average(r => r.Score ?? 0), 1) : 0;

                if (string.IsNullOrEmpty(search) || course.Name.Contains(search) || course.Code.Contains(search))
                {
                    result.Add(new CourseStatDto
                    {
                        Id = course.Id,
                        Code = course.Code,
                        Name = course.Name,
                        Faculty = course.Faculty,
                        Credits = course.Credits,
                        Enrolled = enrollments.Count,
                        MaxStudents = course.MaxStudents,
                        AttendanceRate = attendanceRate,
                        AverageGrade = averageGrade
                    });
                }
            }

            return Ok(result);
        }

        #endregion

        #region Grade Management (Admin)

        // GET: api/admin/grades/overall
        [HttpGet("grades/overall")]
        public async Task<IActionResult> GetGradesOverall([FromQuery] string? semester)
        {
            var allGrades = await _context.LearningResults.ToListAsync();
            var averageGPA = allGrades.Count > 0 ? Math.Round(allGrades.Average(r => r.Score ?? 0) / 2.5, 2) : 0;
            
            var passed = allGrades.Count(r => (r.Score ?? 0) >= 5);
            var passRate = allGrades.Count > 0 ? Math.Round((double)passed / allGrades.Count * 100, 0) : 0;
            
            var excellent = allGrades.Count(r => (r.Score ?? 0) >= 8.5);
            var excellentRate = allGrades.Count > 0 ? Math.Round((double)excellent / allGrades.Count * 100, 0) : 0;

            return Ok(new
            {
                AverageGPA = averageGPA,
                PassRate = passRate,
                ExcellentRate = excellentRate,
                TotalGrades = allGrades.Count
            });
        }

        // GET: api/admin/grades/distribution
        [HttpGet("grades/distribution")]
        public async Task<IActionResult> GetGradeDistribution([FromQuery] string? semester)
        {
            var grades = await _context.LearningResults.ToListAsync();
            var distribution = new[]
            {
                new { Label = "A (8.5-10)", Count = grades.Count(g => (g.Score ?? 0) >= 8.5), Percentage = 0, Color = "#10b981" },
                new { Label = "B (7.0-8.4)", Count = grades.Count(g => (g.Score ?? 0) >= 7 && (g.Score ?? 0) < 8.5), Percentage = 0, Color = "#3b82f6" },
                new { Label = "C (5.5-6.9)", Count = grades.Count(g => (g.Score ?? 0) >= 5.5 && (g.Score ?? 0) < 7), Percentage = 0, Color = "#f59e0b" },
                new { Label = "D (4.0-5.4)", Count = grades.Count(g => (g.Score ?? 0) >= 4 && (g.Score ?? 0) < 5.5), Percentage = 0, Color = "#ef4444" },
                new { Label = "F (<4.0)", Count = grades.Count(g => (g.Score ?? 0) < 4), Percentage = 0, Color = "#8b5cf6" }
            };

            var total = distribution.Sum(d => d.Count);
            var result = distribution.Select(d => new { d.Label, d.Count, Percentage = total > 0 ? Math.Round((double)d.Count / total * 100, 0) : 0, d.Color });

            return Ok(result);
        }

        // GET: api/admin/grades/by-faculty
        [HttpGet("grades/by-faculty")]
        public async Task<IActionResult> GetGradesByFaculty([FromQuery] string? semester)
        {
            var faculties = await _context.Departments.ToListAsync();
            var result = new List<object>();

            foreach (var faculty in faculties)
            {
                var students = await _context.Students.Where(s => s.Faculty == faculty.Name).ToListAsync();
                var studentIds = students.Select(s => s.Id).ToList();
                
                var enrollments = await _context.Enrollments
                    .Where(e => studentIds.Contains(e.StudentId))
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                var allScores = new List<double>();
                foreach (var enrollment in enrollments)
                {
                    var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                    if (finalResult != null && finalResult.Score.HasValue)
                        allScores.Add(finalResult.Score.Value);
                }

                var averageGPA = allScores.Count > 0 ? Math.Round(allScores.Average() / 2.5, 2) : 0;
                var rank = averageGPA >= 3.6 ? "Xuất sắc" : averageGPA >= 3.2 ? "Giỏi" : averageGPA >= 2.5 ? "Khá" : "Trung bình";

                result.Add(new { name = faculty.Name, gpa = averageGPA, color = GetDepartmentColor(faculty.Code), rank });
            }

            return Ok(result);
        }

        // GET: api/admin/grades/students
        [HttpGet("grades/students")]
        public async Task<IActionResult> GetStudentGradesList([FromQuery] string? faculty, [FromQuery] string? search, [FromQuery] string? rank)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(faculty))
                query = query.Where(s => s.Faculty == faculty);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.FullName.Contains(search) || s.StudentId.Contains(search));

            var students = await query.ToListAsync();
            var result = new List<object>();

            foreach (var student in students)
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Completed")
                    .Include(e => e.Course)
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                var totalCredits = enrollments.Sum(e => e.Course?.Credits ?? 0);
                var totalPoints = 0.0;
                
                foreach (var enrollment in enrollments)
                {
                    var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                    var gradePoint = finalResult != null && finalResult.Score.HasValue ? finalResult.Score.Value / 2.5 : 0;
                    totalPoints += gradePoint * (enrollment.Course?.Credits ?? 0);
                }

                var gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
                var studentRank = gpa >= 3.6 ? "Xuất sắc" : gpa >= 3.2 ? "Giỏi" : gpa >= 2.5 ? "Khá" : gpa >= 2.0 ? "Trung bình" : "Yếu";

                if (string.IsNullOrEmpty(rank) || rank == studentRank)
                {
                    result.Add(new
                    {
                        student.Id,
                        student.StudentId,
                        student.FullName,
                        student.Faculty,
                        Gpa = gpa,
                        Credits = totalCredits,
                        Rank = studentRank
                    });
                }
            }

            return Ok(result);
        }

        // GET: api/admin/grades/students/{id}/details
        [HttpGet("grades/students/{id}/details")]
        public async Task<IActionResult> GetStudentGradeDetails(int id)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == id)
                .Include(e => e.Course)
                .Include(e => e.LearningResults)
                .ToListAsync();

            var details = new List<object>();
            foreach (var enrollment in enrollments)
            {
                var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                var finalScore = finalResult?.Score ?? 0;

                details.Add(new
                {
                    CourseId = enrollment.Course?.Id,
                    Code = enrollment.Course?.Code,
                    CourseName = enrollment.Course?.Name,
                    Score = finalScore,
                    LetterGrade = finalResult?.Grade ?? "Chưa có"
                });
            }

            return Ok(details);
        }

        #endregion

        #region Reports

        // GET: api/admin/reports/academic
        [HttpGet("reports/academic")]
        public async Task<IActionResult> GetAcademicReport()
        {
            var allGrades = await _context.LearningResults.ToListAsync();
            var averageGPA = allGrades.Count > 0 ? Math.Round(allGrades.Average(r => r.Score ?? 0) / 2.5, 2) : 0;
            
            var passed = allGrades.Count(r => (r.Score ?? 0) >= 5);
            var passRate = allGrades.Count > 0 ? Math.Round((double)passed / allGrades.Count * 100, 0) : 0;
            
            var excellent = allGrades.Count(r => (r.Score ?? 0) >= 8.5);
            var excellentRate = allGrades.Count > 0 ? Math.Round((double)excellent / allGrades.Count * 100, 0) : 0;

            var faculties = await _context.Departments.ToListAsync();
            var byFaculty = new List<object>();

            foreach (var faculty in faculties)
            {
                var students = await _context.Students.Where(s => s.Faculty == faculty.Name).ToListAsync();
                var studentIds = students.Select(s => s.Id).ToList();
                
                var enrollments = await _context.Enrollments
                    .Where(e => studentIds.Contains(e.StudentId))
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                var scores = new List<double>();
                foreach (var enrollment in enrollments)
                {
                    var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                    if (finalResult != null && finalResult.Score.HasValue)
                        scores.Add(finalResult.Score.Value);
                }

                var avgScore = scores.Count > 0 ? Math.Round(scores.Average(), 1) : 0;
                byFaculty.Add(new { name = faculty.Name, average = avgScore, color = GetDepartmentColor(faculty.Code) });
            }

            return Ok(new { averageGPA, passRate, excellentRate, byFaculty });
        }

        // GET: api/admin/reports/attendance
        [HttpGet("reports/attendance")]
        public async Task<IActionResult> GetAttendanceReport()
        {
            var allAttendances = await _context.Attendances.ToListAsync();
            var overall = allAttendances.Count > 0 
                ? Math.Round((double)allAttendances.Count(a => a.Status == "Present") / allAttendances.Count * 100, 0)
                : 0;

            // Sửa: Lấy courses và attendances riêng, không dùng include Attendances
            var courses = await _context.Courses.ToListAsync();
            
            var courseRates = new List<object>();
            foreach (var course in courses)
            {
                var courseAttendances = allAttendances.Where(a => a.CourseId == course.Id).ToList();
                var rate = courseAttendances.Count > 0 
                    ? Math.Round((double)courseAttendances.Count(a => a.Status == "Present") / courseAttendances.Count * 100, 0)
                    : 0;
                
                courseRates.Add(new { name = course.Name, rate = rate });
            }

            var bestCourse = courseRates.OrderByDescending(c => c.GetType().GetProperty("rate")?.GetValue(c, null)).FirstOrDefault();
            var worstCourse = courseRates.OrderBy(c => c.GetType().GetProperty("rate")?.GetValue(c, null)).FirstOrDefault();

            return Ok(new
            {
                overall,
                bestCourse = bestCourse != null ? bestCourse.GetType().GetProperty("name")?.GetValue(bestCourse, null) : "N/A",
                worstCourse = worstCourse != null ? worstCourse.GetType().GetProperty("name")?.GetValue(worstCourse, null) : "N/A",
                courses = courseRates
            });
        }

        // GET: api/admin/reports/enrollment
        [HttpGet("reports/enrollment")]
        public async Task<IActionResult> GetEnrollmentReport()
        {
            var totalEnrollments = await _context.Enrollments.CountAsync();
            
            var courses = await _context.Courses
                .Include(c => c.Enrollments)
                .ToListAsync();

            var popularCourse = courses
                .OrderByDescending(c => c.Enrollments.Count)
                .FirstOrDefault();

            var students = await _context.Students.ToListAsync();
            var totalStudents = students.Count;
            
            var enrollmentsByStudent = await _context.Enrollments
                .GroupBy(e => e.StudentId)
                .Select(g => g.Count())
                .ToListAsync();

            var averagePerStudent = totalStudents > 0 ? Math.Round((double)totalEnrollments / totalStudents, 1) : 0;

            var trend = new[]
            {
                new { semester = "Fall 2022", value = 1250 },
                new { semester = "Spring 2023", value = 1420 },
                new { semester = "Fall 2023", value = 1680 },
                new { semester = "Spring 2024", value = 1850 },
                new { semester = "Fall 2024", value = 2120 }
            };

            return Ok(new
            {
                totalEnrollments,
                popularCourse = popularCourse != null ? $"{popularCourse.Name} ({popularCourse.Enrollments.Count} SV)" : "N/A",
                averagePerStudent,
                trend
            });
        }

        #endregion

        #region Course Management - Additional Stats

// GET: api/admin/courses/stats
[HttpGet("courses/stats")]
public async Task<IActionResult> GetCourseStatistics()
{
    var totalCourses = await _context.Courses.CountAsync();
    var totalCredits = await _context.Courses.SumAsync(c => c.Credits);
    var activeCourses = await _context.Courses.CountAsync(c => c.Semester == "Fall 2024");
    
    var totalEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Active");
    var courseEnrollments = await _context.Enrollments
        .Where(e => e.Status == "Active")
        .GroupBy(e => e.CourseId)
        .Select(g => new { CourseId = g.Key, Count = g.Count() })
        .ToListAsync();
    
    var popularCourse = await _context.Courses
        .Select(c => new
        {
            c.Name,
            Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active")
        })
        .OrderByDescending(c => c.Enrolled)
        .FirstOrDefaultAsync();

    return Ok(new
    {
        totalCourses,
        totalCredits,
        activeCourses,
        totalEnrollments,
        popularCourseName = popularCourse?.Name ?? "N/A",
        popularCourseEnrollment = popularCourse?.Enrolled ?? 0
    });
}

// GET: api/admin/courses/enrollment-stats
[HttpGet("courses/enrollment-stats")]
public async Task<IActionResult> GetCourseEnrollmentStats([FromQuery] string? semester)
{
    var courses = await _context.Courses
        .Where(c => string.IsNullOrEmpty(semester) || c.Semester == semester)
        .Select(c => new
        {
            c.Id,
            c.Code,
            c.Name,
            c.Faculty,
            c.Credits,
            Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active"),
            c.MaxStudents,
            Semester = c.Semester
        })
        .OrderByDescending(c => c.Enrolled)
        .ToListAsync();

    return Ok(courses);
}

#endregion

#region Semester Management

// GET: api/admin/semesters
[HttpGet("semesters")]
public async Task<IActionResult> GetSemesters()
{
    var semesters = await _context.Courses
        .Select(c => c.Semester)
        .Where(s => s != null)
        .Distinct()
        .OrderByDescending(s => s)
        .ToListAsync();

    // Add default semesters if none exist
    var defaultSemesters = new List<string> { "Fall 2024", "Spring 2024", "Summer 2024", "Fall 2023" };
    var allSemesters = semesters.Union(defaultSemesters).Distinct().OrderByDescending(s => s).ToList();

    return Ok(allSemesters);
}

// GET: api/admin/semesters/current
[HttpGet("semesters/current")]
public async Task<IActionResult> GetCurrentSemester()
{
    var currentSemester = "Fall 2024";
    var latestSemester = await _context.Courses
        .Select(c => c.Semester)
        .Where(s => s != null)
        .OrderByDescending(s => s)
        .FirstOrDefaultAsync();
    
    if (!string.IsNullOrEmpty(latestSemester))
        currentSemester = latestSemester;

    return Ok(new { semester = currentSemester });
}

#endregion

#region Grade Management - Enhanced

// GET: api/admin/grades/course-stats
[HttpGet("grades/course-stats")]
public async Task<IActionResult> GetCourseGradeStats([FromQuery] string? faculty, [FromQuery] string? semester)
{
    var query = _context.Courses.AsQueryable();
    
    if (!string.IsNullOrEmpty(faculty))
        query = query.Where(c => c.Faculty == faculty);
    
    if (!string.IsNullOrEmpty(semester))
        query = query.Where(c => c.Semester == semester);

    var courses = await query.ToListAsync();
    var result = new List<object>();

    foreach (var course in courses)
    {
        var enrollments = await _context.Enrollments
            .Where(e => e.CourseId == course.Id && e.Status == "Completed")
            .Include(e => e.LearningResults)
            .ToListAsync();

        var scores = new List<double>();
        foreach (var enrollment in enrollments)
        {
            var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
            if (finalResult != null && finalResult.Score.HasValue)
                scores.Add(finalResult.Score.Value);
        }

        var averageScore = scores.Count > 0 ? Math.Round(scores.Average(), 1) : 0;
        var letterGrade = averageScore >= 8.5 ? "A" : averageScore >= 7.0 ? "B" : averageScore >= 5.5 ? "C" : averageScore >= 4.0 ? "D" : "F";
        var passCount = scores.Count(s => s >= 5);
        var passRate = scores.Count > 0 ? Math.Round((double)passCount / scores.Count * 100, 0) : 0;

        result.Add(new
        {
            course.Id,
            course.Code,
            course.Name,
            course.Faculty,
            course.Credits,
            Enrolled = enrollments.Count,
            AverageScore = averageScore,
            LetterGrade = letterGrade,
            PassRate = passRate,
            course.Semester
        });
    }

    return Ok(result);
}

// GET: api/admin/grades/student-rankings
[HttpGet("grades/student-rankings")]
public async Task<IActionResult> GetStudentRankings([FromQuery] string? faculty, [FromQuery] string? semester, [FromQuery] int top = 10)
{
    var students = await _context.Students.ToListAsync();
    var rankings = new List<object>();

    foreach (var student in students)
    {
        if (!string.IsNullOrEmpty(faculty) && student.Faculty != faculty)
            continue;

        var enrollments = await _context.Enrollments
            .Where(e => e.StudentId == student.Id && e.Status == "Completed")
            .Include(e => e.Course)
            .Include(e => e.LearningResults)
            .ToListAsync();

        var totalCredits = enrollments.Sum(e => e.Course?.Credits ?? 0);
        var totalPoints = 0.0;
        
        foreach (var enrollment in enrollments)
        {
            var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
            var gradePoint = finalResult != null && finalResult.Score.HasValue ? finalResult.Score.Value / 2.5 : 0;
            totalPoints += gradePoint * (enrollment.Course?.Credits ?? 0);
        }

        var gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        
        rankings.Add(new
        {
            student.Id,
            student.StudentId,
            student.FullName,
            student.Faculty,
            GPA = gpa,
            TotalCredits = totalCredits,
            Rank = gpa >= 3.6 ? "Xuất sắc" : gpa >= 3.2 ? "Giỏi" : gpa >= 2.5 ? "Khá" : gpa >= 2.0 ? "Trung bình" : "Yếu"
        });
    }

    var sortedRankings = rankings.OrderByDescending(r => r.GetType().GetProperty("GPA")?.GetValue(r, null)).Take(top);
    return Ok(sortedRankings);
}

#endregion

#region Dashboard - Enhanced Stats

// GET: api/admin/dashboard/enrollment-by-course
[HttpGet("dashboard/enrollment-by-course")]
public async Task<IActionResult> GetEnrollmentByCourse([FromQuery] string? semester)
{
    var courses = await _context.Courses
        .Where(c => string.IsNullOrEmpty(semester) || c.Semester == semester)
        .Select(c => new
        {
            c.Id,
            c.Name,
            c.Code,
            Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active"),
            c.MaxStudents
        })
        .OrderByDescending(c => c.Enrolled)
        .Take(10)
        .ToListAsync();

    return Ok(courses);
}

// GET: api/admin/dashboard/attendance-trend
[HttpGet("dashboard/attendance-trend")]
public async Task<IActionResult> GetAttendanceTrend([FromQuery] int months = 6)
{
    var trend = new List<object>();
    var now = DateTime.UtcNow;
    
    for (int i = months - 1; i >= 0; i--)
    {
        var month = now.AddMonths(-i);
        var monthName = $"Tháng {month.Month}";
        
        var attendances = await _context.Attendances
            .Where(a => a.Date.Month == month.Month && a.Date.Year == month.Year)
            .ToListAsync();
        
        var rate = attendances.Count > 0 
            ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
            : 0;
        
        trend.Add(new { month = monthName, rate = rate });
    }
    
    return Ok(trend);
}

#endregion

        #region Private Methods

        private string GetDepartmentColor(string code)
        {
            return code switch
            {
                "CNTT" => "#3b82f6",
                "QTKD" => "#10b981",
                "NNKT" => "#f59e0b",
                _ => "#8b5cf6"
            };
        }

        #endregion

        #region Dashboard - Enhanced Stats (Bổ sung)

// GET: api/admin/dashboard/enrollment-by-semester
[HttpGet("dashboard/enrollment-by-semester")]
public async Task<IActionResult> GetEnrollmentBySemester()
{
    var enrollmentsBySemester = await _context.Enrollments
        .GroupBy(e => e.EnrollmentDate.Year + "-" + e.EnrollmentDate.Month)
        .Select(g => new
        {
            Period = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Period)
        .ToListAsync();

    // Format lại cho biểu đồ (lấy 6 tháng gần nhất)
    var result = new List<object>();
    var now = DateTime.UtcNow;
    
    for (int i = 5; i >= 0; i--)
    {
        var month = now.AddMonths(-i);
        var monthName = $"Tháng {month.Month}";
        var period = $"{month.Year}-{month.Month}";
        
        var count = enrollmentsBySemester
            .Where(x => x.Period == period)
            .Select(x => x.Count)
            .FirstOrDefault();
        
        result.Add(new { label = monthName, value = count });
    }
    
    return Ok(result);
}

// GET: api/admin/dashboard/enrollment-by-month-year
[HttpGet("dashboard/enrollment-by-month-year")]
public async Task<IActionResult> GetEnrollmentByMonthYear([FromQuery] int year = 2024)
{
    var enrollments = await _context.Enrollments
        .Where(e => e.EnrollmentDate.Year == year)
        .GroupBy(e => e.EnrollmentDate.Month)
        .Select(g => new
        {
            Month = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Month)
        .ToListAsync();

    var months = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", 
                         "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
    
    var result = new List<object>();
    foreach (var month in months.Select((name, index) => new { name, index = index + 1 }))
    {
        var count = enrollments.Where(x => x.Month == month.index).Select(x => x.Count).FirstOrDefault();
        result.Add(new { label = month.name, value = count });
    }
    
    return Ok(result);
}

// GET: api/admin/dashboard/student-distribution
[HttpGet("dashboard/student-distribution")]
public async Task<IActionResult> GetStudentDistribution()
{
    var departments = await _context.Departments.ToListAsync();
    var totalStudents = await _context.Students.CountAsync();
    
    if (totalStudents == 0)
    {
        // Return mock data if no students
        return Ok(new[]
        {
            new { name = "Công nghệ thông tin", count = 486, percent = 39.0, color = "#3b82f6" },
            new { name = "Quản trị kinh doanh", count = 325, percent = 26.0, color = "#10b981" },
            new { name = "Ngôn ngữ Anh", count = 218, percent = 17.5, color = "#f59e0b" },
            new { name = "Điện tử viễn thông", count = 128, percent = 10.3, color = "#8b5cf6" },
            new { name = "Khác", count = 91, percent = 7.2, color = "#ef4444" }
        });
    }
    
    var result = departments.Select(d => new
    {
        name = d.Name,
        count = d.StudentCount,
        percent = Math.Round((double)d.StudentCount / totalStudents * 100, 1),
        color = GetDepartmentColor(d.Code)
    }).OrderByDescending(x => x.percent).ToList();
    
    return Ok(result);
}

// GET: api/admin/dashboard/statistics-overview
[HttpGet("dashboard/statistics-overview")]
public async Task<IActionResult> GetStatisticsOverview()
{
    var totalStudents = await _context.Students.CountAsync();
    var totalLecturers = await _context.Lecturers.CountAsync();
    var totalCourses = await _context.Courses.CountAsync();
    var totalDepartments = await _context.Departments.CountAsync();
    
    var activeEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Active");
    var completedEnrollments = await _context.Enrollments.CountAsync(e => e.Status == "Completed");
    
    var attendances = await _context.Attendances.ToListAsync();
    var averageAttendance = attendances.Count > 0 
        ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
        : 0;
    
    var completionRate = activeEnrollments + completedEnrollments > 0
        ? Math.Round((double)completedEnrollments / (activeEnrollments + completedEnrollments) * 100, 0)
        : 0;
    
    // Calculate growth (mock for now, can be calculated from historical data)
    var studentGrowth = 12.5;
    
    return Ok(new
    {
        totalStudents,
        totalLecturers,
        totalCourses,
        totalDepartments,
        activeEnrollments,
        averageAttendance,
        completionRate,
        studentGrowth
    });
}

#endregion
    }
}
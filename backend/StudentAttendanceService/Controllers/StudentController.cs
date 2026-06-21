using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceService.Data;
using StudentAttendanceService.DTOs.Student;
using StudentAttendanceService.Models;

namespace StudentAttendanceService.Controllers
{
    [Authorize(Roles = "STUDENT")]
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentController> _logger;

        public StudentController(AppDbContext context, ILogger<StudentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private Helper Methods

        private async Task<Student?> GetStudentByEmail()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;
            return await _context.Students.FirstOrDefaultAsync(s => s.Email == email);
        }

        #endregion

        #region Dashboard APIs

        // GET: api/student/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy thống kê học viên...");

                var student = await GetStudentByEmail();
                if (student == null)
                {
                    _logger.LogWarning("⚠️ Không tìm thấy học viên");
                    return NotFound(new { message = "Student not found" });
                }

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Course)
                    .ToListAsync();

                var learningResults = await _context.LearningResults
                    .Where(r => r.Enrollment != null && r.Enrollment.StudentId == student.Id)
                    .ToListAsync();

                var attendances = await _context.Attendances
                    .Where(a => a.StudentId == student.Id)
                    .ToListAsync();

                var gpa = CalculateGPA(learningResults, enrollments);
                var creditsEarned = enrollments.Where(e => e.Status == "Completed").Sum(e => e.Course?.Credits ?? 0);
                var attendanceRate = attendances.Count > 0
                    ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                    : 0;

                var stats = new StudentStatsDto
                {
                    Gpa = gpa,
                    GpaTrend = 0.15,
                    CreditsEarned = creditsEarned,
                    RemainingCredits = 132 - creditsEarned,
                    Attendance = (int)attendanceRate,
                    CurrentCourses = enrollments.Count(e => e.Status == "Active"),
                    Rank = GetRank(gpa)
                };

                _logger.LogInformation($"✅ Thống kê học viên: GPA={gpa}, Credits={creditsEarned}, Attendance={attendanceRate}%");
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thống kê học viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê học viên" });
            }
        }

        // GET: api/student/courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetCurrentCourses()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy danh sách khóa học hiện tại...");

                var student = await GetStudentByEmail();
                if (student == null) return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ThenInclude(c => c!.Lecturer)
                    .ToListAsync();

                var attendances = await _context.Attendances
                    .Where(a => a.StudentId == student.Id)
                    .ToListAsync();

                var courses = enrollments.Select(e => new CurrentCourseDto
                {
                    Id = e.Course!.Id,
                    CourseName = e.Course.Name,
                    Code = e.Course.Code,
                    Schedule = e.Course.Schedule ?? "Chưa cập nhật",
                    Room = e.Course.Room ?? "Chưa cập nhật",
                    Credits = e.Course.Credits,
                    Lecturer = e.Course.Lecturer?.FullName ?? "Chưa cập nhật",
                    Attendance = (int)(attendances.Where(a => a.CourseId == e.Course.Id).Count() > 0
                        ? (double)attendances.Count(a => a.CourseId == e.Course.Id && a.Status == "Present") /
                          attendances.Count(a => a.CourseId == e.Course.Id) * 100
                        : 0)
                }).ToList();

                _logger.LogInformation($"✅ Lấy {courses.Count} khóa học hiện tại");
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy danh sách khóa học: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách khóa học" });
            }
        }

        // GET: api/student/upcoming-classes
        [HttpGet("upcoming-classes")]
        public async Task<IActionResult> GetUpcomingClasses()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy lịch học sắp tới...");

                var student = await GetStudentByEmail();
                if (student == null) return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ThenInclude(c => c!.Lecturer)
                    .ToListAsync();

                var upcoming = enrollments.Select(e => new UpcomingClassDto
                {
                    Id = e.Course!.Id,
                    CourseName = e.Course.Name,
                    Day = e.Course.Schedule?.Split(',')[0]?.Trim() ?? "Chưa cập nhật",
                    Time = e.Course.Schedule ?? "Chưa cập nhật",
                    Room = e.Course.Room ?? "Chưa cập nhật",
                    Lecturer = e.Course.Lecturer?.FullName ?? "Chưa cập nhật"
                }).Take(5).ToList();

                _logger.LogInformation($"✅ Lấy {upcoming.Count} lịch học sắp tới");
                return Ok(upcoming);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy lịch học sắp tới: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy lịch học sắp tới" });
            }
        }

        // GET: api/student/recent-grades
        [HttpGet("recent-grades")]
        public async Task<IActionResult> GetRecentGrades()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy điểm gần đây...");

                var student = await GetStudentByEmail();
                if (student == null) return NotFound(new { message = "Student not found" });

                var grades = await _context.LearningResults
                    .Where(r => r.Enrollment != null && r.Enrollment.StudentId == student.Id)
                    .OrderByDescending(r => r.RecordedDate)
                    .Take(5)
                    .Select(r => new RecentGradeDto
                    {
                        Id = r.Id,
                        CourseName = r.Enrollment!.Course!.Name,
                        ExamType = r.ExamType,
                        Score = r.Score ?? 0,
                        LetterGrade = r.Grade,
                        RecordedDate = r.RecordedDate
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {grades.Count} điểm gần đây");
                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm gần đây: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy điểm gần đây" });
            }
        }

        // GET: api/student/announcements
        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy thông báo...");

                var announcements = await _context.Announcements
                    .Where(a => a.TargetRole == "ALL" || a.TargetRole == "STUDENT")
                    .OrderByDescending(a => a.Date)
                    .Take(5)
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {announcements.Count} thông báo");
                return Ok(announcements);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thông báo: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thông báo" });
            }
        }

        // GET: api/student/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var result = new
                {
                    student.Id,
                    student.StudentId,
                    student.FullName,
                    student.Email,
                    student.Phone,
                    student.Faculty,
                    student.Major,
                    student.Class,
                    Address = student.Address ?? "Chưa cập nhật",
                    Year = student.Year ?? DateTime.Now.Year - 3
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thông tin học viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin học viên" });
            }
        }

        // GET: api/student/schedule
        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy lịch học từ {startDate} đến {endDate}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Không tìm thấy học viên" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ThenInclude(c => c!.Lecturer)
                    .ToListAsync();

                if (enrollments.Count == 0)
                {
                    _logger.LogWarning("⚠️ Học viên chưa đăng ký khóa học nào");
                    return Ok(new List<object>());
                }

                var courseIds = enrollments.Select(e => e.CourseId).ToList();
                var courses = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .Include(c => c.Lecturer)
                    .ToListAsync();

                var schedules = new List<object>();

                foreach (var course in courses)
                {
                    if (string.IsNullOrEmpty(course.Schedule))
                        continue;

                    var parts = course.Schedule.Split(',');
                    if (parts.Length != 2) continue;

                    var dayName = parts[0].Trim();
                    var timeRange = parts[1].Trim();

                    var today = DateTime.Today;
                    var dayOfWeek = GetDayOfWeek(dayName);
                    var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7;
                    if (daysUntil == 0) daysUntil = 7;

                    var classDate = today.AddDays(daysUntil);

                    var timeParts = timeRange.Split('-');
                    if (timeParts.Length != 2) continue;

                    var startTime = DateTime.Parse(timeParts[0].Trim());
                    var endTime = DateTime.Parse(timeParts[1].Trim());

                    var startDateTime = classDate.Date.Add(startTime.TimeOfDay);
                    var endDateTime = classDate.Date.Add(endTime.TimeOfDay);

                    if (!string.IsNullOrEmpty(startDate) && DateTime.TryParse(startDate, out var start))
                    {
                        if (startDateTime < start) continue;
                    }
                    if (!string.IsNullOrEmpty(endDate) && DateTime.TryParse(endDate, out var end))
                    {
                        if (startDateTime > end) continue;
                    }

                    schedules.Add(new
                    {
                        Id = course.Id,
                        CourseId = course.Id,
                        CourseName = course.Name,
                        ClassName = course.Name,
                        StartTime = startDateTime,
                        EndTime = endDateTime,
                        Room = course.Room ?? "Chưa cập nhật",
                        RoomName = course.Room ?? "Chưa cập nhật",
                        Lecturer = course.Lecturer?.FullName ?? "Chưa cập nhật",
                        TeacherName = course.Lecturer?.FullName ?? "Chưa cập nhật",
                        Credits = course.Credits,
                        Code = course.Code
                    });
                }

                var result = schedules
                    .OrderBy(s => s.GetType().GetProperty("StartTime")?.GetValue(s, null))
                    .ToList();

                _logger.LogInformation($"✅ Lấy {result.Count} buổi học");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy lịch học: {ex.Message}");
                _logger.LogError($"❌ Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi khi lấy lịch học", error = ex.Message });
            }
        }

        #endregion

        #region Attendance Management APIs

        // GET: api/student/attendance/overall
        [HttpGet("attendance/overall")]
        public async Task<IActionResult> GetOverallAttendance()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy thống kê tổng quan điểm danh...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var attendances = await _context.Attendances
                    .Where(a => a.StudentId == student.Id)
                    .ToListAsync();

                var total = attendances.Count;
                var present = attendances.Count(a => a.Status == "Present");
                var absent = attendances.Count(a => a.Status == "Absent");
                var late = attendances.Count(a => a.Status == "Late");
                var percentage = total > 0 ? Math.Round((double)present / total * 100, 0) : 0;

                _logger.LogInformation($"✅ Thống kê điểm danh: Tổng={total}, Có mặt={present}, Vắng={absent}, Muộn={late}, Tỷ lệ={percentage}%");
                return Ok(new { total, present, absent, late, percentage });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thống kê điểm danh: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thống kê điểm danh" });
            }
        }

        // GET: api/student/attendance/summary
        [HttpGet("attendance/summary")]
        public async Task<IActionResult> GetAttendanceSummary([FromQuery] string? semester)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy tổng hợp điểm danh theo môn học...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                // Lấy tất cả enrollment đang active của student
                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ToListAsync();

                _logger.LogInformation($"📊 Tìm thấy {enrollments.Count} môn học đang học");

                var summary = new List<AttendanceSummaryDto>();
                foreach (var enrollment in enrollments)
                {
                    var attendances = await _context.Attendances
                        .Where(a => a.StudentId == student.Id && a.CourseId == enrollment.CourseId)
                        .ToListAsync();

                    var total = attendances.Count;
                    var present = attendances.Count(a => a.Status == "Present");
                    var absent = attendances.Count(a => a.Status == "Absent");
                    var late = attendances.Count(a => a.Status == "Late");
                    var percentage = total > 0 ? Math.Round((double)present / total * 100, 0) : 0;

                    summary.Add(new AttendanceSummaryDto
                    {
                        CourseId = enrollment.Course!.Id,
                        CourseName = enrollment.Course.Name,
                        CourseCode = enrollment.Course.Code,
                        TotalSessions = total,
                        Present = present,
                        Absent = absent,
                        Late = late,
                        Percentage = percentage,
                        Color = GetColorByPercentage(percentage)
                    });
                }

                _logger.LogInformation($"✅ Lấy tổng hợp điểm danh cho {summary.Count} môn học");
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy tổng hợp điểm danh: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy tổng hợp điểm danh" });
            }
        }

        // GET: api/student/attendance/records
        [HttpGet("attendance/records")]
        public async Task<IActionResult> GetAttendanceRecords([FromQuery] int? courseId, [FromQuery] string? status)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy lịch sử điểm danh với courseId={courseId}, status={status}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var query = _context.Attendances
                    .Where(a => a.StudentId == student.Id)
                    .AsQueryable();

                if (courseId.HasValue && courseId.Value > 0)
                {
                    query = query.Where(a => a.CourseId == courseId.Value);
                }

                if (!string.IsNullOrEmpty(status) && status != "all")
                {
                    query = query.Where(a => a.Status == status);
                }

                // Lấy danh sách course names
                var courseIds = await query.Select(a => a.CourseId).Distinct().ToListAsync();
                var courses = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Name);

                var records = await query
                    .OrderByDescending(a => a.Date)
                    .Select(a => new
                    {
                        a.Id,
                        a.Date,
                        CourseId = a.CourseId,
                        a.CheckInTime,
                        a.CheckOutTime,
                        a.Status,
                        a.Week
                    })
                    .ToListAsync();

                var result = records.Select(r => new
                {
                    r.Id,
                    r.Date,
                    CourseName = courses.ContainsKey(r.CourseId) ? courses[r.CourseId] : "Không xác định",
                    r.CheckInTime,
                    r.CheckOutTime,
                    r.Status,
                    r.Week
                }).ToList();

                _logger.LogInformation($"✅ Lấy {result.Count} bản ghi điểm danh");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy lịch sử điểm danh: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy lịch sử điểm danh" });
            }
        }

        #endregion

        #region Course Enrollment APIs

        // GET: api/student/enrollments
        [HttpGet("enrollments")]
        public async Task<IActionResult> GetEnrollments()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy danh sách đăng ký khóa học...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id)
                    .Include(e => e.Course)
                    .Select(e => new
                    {
                        e.Id,
                        e.Course!.Code,
                        e.Course.Name,
                        e.Course.Credits,
                        e.Course.Schedule,
                        e.Course.Room,
                        e.Status,
                        e.EnrollmentDate,
                        CompletedDate = e.CompletedDate
                    })
                    .OrderByDescending(e => e.EnrollmentDate)
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {enrollments.Count} đăng ký khóa học");
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy danh sách đăng ký: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách đăng ký" });
            }
        }

        // POST: api/student/enrollments
        [HttpPost("enrollments")]
        public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseRequest request)
        {
            try
            {
                _logger.LogInformation($"📊 Đang đăng ký khóa học {request.CourseId}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var existing = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == request.CourseId && e.Status == "Active");

                if (existing != null)
                    return BadRequest(new { message = "Đã đăng ký khóa học này" });

                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null)
                    return NotFound(new { message = "Không tìm thấy khóa học" });

                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = request.CourseId,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = "Active"
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Đăng ký khóa học thành công");
                return Ok(new { message = "Đăng ký thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi đăng ký khóa học: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi đăng ký khóa học" });
            }
        }

        // DELETE: api/student/enrollments/{id}
        [HttpDelete("enrollments/{id}")]
        public async Task<IActionResult> UnregisterCourse(int id)
        {
            try
            {
                _logger.LogInformation($"📊 Đang hủy đăng ký khóa học {id}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.Id == id && e.StudentId == student.Id);

                if (enrollment == null)
                    return NotFound(new { message = "Không tìm thấy đăng ký" });

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Hủy đăng ký khóa học thành công");
                return Ok(new { message = "Hủy đăng ký thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi hủy đăng ký khóa học: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi hủy đăng ký khóa học" });
            }
        }

        #endregion

        #region Grade APIs

        // GET: api/student/grades
        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy tất cả điểm của học viên...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var grades = await _context.LearningResults
                    .Where(r => r.Enrollment != null && r.Enrollment.StudentId == student.Id)
                    .Include(r => r.Enrollment)
                    .ThenInclude(e => e!.Course)
                    .OrderByDescending(r => r.RecordedDate)
                    .Select(r => new
                    {
                        r.Id,
                        CourseName = r.Enrollment!.Course!.Name,
                        CourseCode = r.Enrollment!.Course!.Code,
                        r.ExamType,
                        r.Score,
                        r.Grade,
                        r.RecordedDate
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {grades.Count} điểm của học viên");
                return Ok(grades);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy điểm" });
            }
        }

        // GET: api/student/gpa
        [HttpGet("gpa")]
        public async Task<IActionResult> GetGPA()
        {
            try
            {
                _logger.LogInformation("🔄 Đang tính GPA của học viên...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Course)
                    .ToListAsync();

                var learningResults = await _context.LearningResults
                    .Where(r => r.Enrollment != null && r.Enrollment.StudentId == student.Id)
                    .ToListAsync();

                var gpa = CalculateGPA(learningResults, enrollments);
                var totalCredits = enrollments.Where(e => e.Status == "Completed").Sum(e => e.Course?.Credits ?? 0);
                var rank = GetRank(gpa);

                _logger.LogInformation($"✅ GPA={gpa}, Tổng tín chỉ={totalCredits}, Xếp loại={rank}");
                return Ok(new { gpa, totalCredits, rank });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi tính GPA: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi tính GPA" });
            }
        }

        #endregion

        #region Course Enrollment APIs - Extended

        // GET: api/student/courses/available
        [HttpGet("courses/available")]
        public async Task<IActionResult> GetAvailableCourses(
            [FromQuery] string? search,
            [FromQuery] string? faculty,
            [FromQuery] int? credits)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy danh sách khóa học có sẵn...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                // Lấy danh sách course ID đã đăng ký
                var registeredCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Select(e => e.CourseId)
                    .ToListAsync();

                // Query các khóa học chưa đăng ký
                var query = _context.Courses
                    .Where(c => !registeredCourseIds.Contains(c.Id))
                    .Include(c => c.Lecturer)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));
                }

                if (!string.IsNullOrEmpty(faculty) && faculty != "all")
                {
                    query = query.Where(c => c.Faculty == faculty);
                }

                if (credits.HasValue && credits.Value > 0)
                {
                    query = query.Where(c => c.Credits == credits.Value);
                }

                var courses = await query
                    .Select(c => new
                    {
                        c.Id,
                        c.Code,
                        c.Name,
                        c.Credits,
                        c.Schedule,
                        c.Room,
                        c.MaxStudents,
                        Lecturer = c.Lecturer != null ? c.Lecturer.FullName : "Chưa cập nhật",
                        Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active")
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Tìm thấy {courses.Count} khóa học có sẵn");
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy khóa học có sẵn: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy khóa học có sẵn" });
            }
        }

        // GET: api/student/courses/registered
        [HttpGet("courses/registered")]
        public async Task<IActionResult> GetRegisteredCourses()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy danh sách khóa học đã đăng ký...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ThenInclude(c => c!.Lecturer)
                    .Select(e => new
                    {
                        e.Course!.Id,
                        e.Course.Code,
                        e.Course.Name,
                        e.Course.Credits,
                        e.Course.Schedule,
                        e.Course.Room,
                        Lecturer = e.Course.Lecturer != null ? e.Course.Lecturer.FullName : "Chưa cập nhật",
                        e.EnrollmentDate,
                        Status = e.Status
                    })
                    .OrderBy(e => e.EnrollmentDate)
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {enrollments.Count} khóa học đã đăng ký");
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy khóa học đã đăng ký: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        // POST: api/student/courses/register
        [HttpPost("courses/register")]
        public async Task<IActionResult> RegisterCourseStudent([FromBody] RegisterCourseRequest request)
        {
            try
            {
                _logger.LogInformation($"📊 Đang đăng ký khóa học {request.CourseId}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                // Check if already registered
                var existing = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == request.CourseId && e.Status == "Active");

                if (existing != null)
                    return BadRequest(new { message = "Bạn đã đăng ký khóa học này" });

                var course = await _context.Courses.FindAsync(request.CourseId);
                if (course == null)
                    return NotFound(new { message = "Không tìm thấy khóa học" });

                // Check max students
                var enrolledCount = await _context.Enrollments
                    .CountAsync(e => e.CourseId == request.CourseId && e.Status == "Active");

                if (enrolledCount >= (course.MaxStudents > 0 ? course.MaxStudents : 40))
                    return BadRequest(new { message = "Khóa học đã đủ số lượng sinh viên" });

                var enrollment = new Enrollment
                {
                    StudentId = student.Id,
                    CourseId = request.CourseId,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = "Active"
                    // Bỏ CreatedAt = DateTime.UtcNow nếu model không có property này
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Đăng ký khóa học thành công");
                return Ok(new { message = "Đăng ký thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi đăng ký khóa học: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi đăng ký khóa học" });
            }
        }

        // DELETE: api/student/courses/register/{courseId}
        [HttpDelete("courses/register/{courseId}")]
        public async Task<IActionResult> UnregisterCourseStudent(int courseId)
        {
            try
            {
                _logger.LogInformation($"📊 Đang hủy đăng ký khóa học {courseId}...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId && e.Status == "Active");

                if (enrollment == null)
                    return NotFound(new { message = "Không tìm thấy đăng ký" });

                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Hủy đăng ký khóa học thành công");
                return Ok(new { message = "Hủy đăng ký thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi hủy đăng ký khóa học: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi hủy đăng ký khóa học" });
            }
        }

        // GET: api/student/timetable
        [HttpGet("timetable")]
        public async Task<IActionResult> GetTimetable()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy thời khóa biểu...");

                var student = await GetStudentByEmail();
                if (student == null)
                    return NotFound(new { message = "Student not found" });

                var enrollments = await _context.Enrollments
                    .Where(e => e.StudentId == student.Id && e.Status == "Active")
                    .Include(e => e.Course)
                    .ToListAsync();

                var timetable = enrollments
                    .Where(e => e.Course != null && !string.IsNullOrEmpty(e.Course.Schedule))
                    .Select(e => new
                    {
                        e.Course!.Id,
                        CourseName = e.Course.Name,
                        Day = e.Course.Schedule.Split(',')[0]?.Trim() ?? "Chưa cập nhật",
                        Time = e.Course.Schedule.Split(',').Length > 1
                            ? e.Course.Schedule.Split(',')[1]?.Trim() ?? "Chưa cập nhật"
                            : "Chưa cập nhật",
                        Room = e.Course.Room ?? "Chưa cập nhật"
                    })
                    .OrderBy(t => t.Day)
                    .ToList();

                _logger.LogInformation($"✅ Lấy {timetable.Count} lịch học");
                return Ok(timetable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thời khóa biểu: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        #endregion

        #region Helper Methods

        private DayOfWeek GetDayOfWeek(string dayName)
        {
            return dayName switch
            {
                "Thứ 2" => DayOfWeek.Monday,
                "Thứ 3" => DayOfWeek.Tuesday,
                "Thứ 4" => DayOfWeek.Wednesday,
                "Thứ 5" => DayOfWeek.Thursday,
                "Thứ 6" => DayOfWeek.Friday,
                "Thứ 7" => DayOfWeek.Saturday,
                "Chủ nhật" => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };
        }

        private double CalculateGPA(List<LearningResult> results, List<Enrollment> enrollments)
        {
            if (!results.Any()) return 0;
            var totalPoints = results.Sum(r => (r.Score ?? 0) * (r.Weight ?? 0.25));
            return Math.Round(totalPoints, 2);
        }

        private string GetRank(double gpa)
        {
            if (gpa >= 3.6) return "Xuất sắc";
            if (gpa >= 3.2) return "Giỏi";
            if (gpa >= 2.5) return "Khá";
            if (gpa >= 2.0) return "Trung bình";
            return "Yếu";
        }

        private string GetColorByPercentage(double percentage)
        {
            if (percentage >= 90) return "#10b981";
            if (percentage >= 75) return "#3b82f6";
            if (percentage >= 60) return "#f59e0b";
            return "#ef4444";
        }

        #endregion
    }

    #region Request DTOs

    public class RegisterCourseRequest
    {
        public int CourseId { get; set; }
    }

    #endregion
}
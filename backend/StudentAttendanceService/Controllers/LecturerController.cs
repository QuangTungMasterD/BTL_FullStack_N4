using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceService.Data;
using StudentAttendanceService.DTOs.Lecturer;
using StudentAttendanceService.Models;

namespace StudentAttendanceService.Controllers
{
    [Authorize(Roles = "LECTURER")]
    [ApiController]
    [Route("api/lecturer")]
    public class LecturerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LecturerController> _logger;

        public LecturerController(AppDbContext context, ILogger<LecturerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private Helper Methods

        private async Task<Lecturer?> GetLecturerByEmail()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;
            return await _context.Lecturers.FirstOrDefaultAsync(l => l.Email == email);
        }

        #endregion

        #region Dashboard APIs

        // GET: api/lecturer/courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetTeachingCourses()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy danh sách khóa học giảng dạy...");

                var lecturer = await GetLecturerByEmail();
                if (lecturer == null) return Unauthorized();

                var courses = await _context.Courses
                    .Where(c => c.LecturerId == lecturer.Id)
                    .Include(c => c.Enrollments)
                    .Select(c => new LecturerCourseDto
                    {
                        Id = c.Id,
                        CourseName = c.Name,
                        Code = c.Code,
                        Credits = c.Credits,
                        StudentCount = c.Enrollments.Count(e => e.Status == "Active" || e.Status == "Completed"),
                        Schedule = c.Schedule ?? "Chưa cập nhật",
                        Room = c.Room ?? "Chưa cập nhật",
                        Faculty = c.Faculty,
                        Semester = c.Semester
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {courses.Count} khóa học giảng dạy");
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy khóa học giảng dạy: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy khóa học giảng dạy" });
            }
        }

        // GET: api/lecturer/today-schedule
        [HttpGet("today-schedule")]
        public async Task<IActionResult> GetTodaySchedule()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy lịch dạy hôm nay...");

                var lecturer = await GetLecturerByEmail();
                if (lecturer == null) return Unauthorized();

                var today = DateTime.Now.DayOfWeek.ToString();
                var dayMap = new Dictionary<string, string>
                {
                    { "Monday", "Thứ 2" }, { "Tuesday", "Thứ 3" }, { "Wednesday", "Thứ 4" },
                    { "Thursday", "Thứ 5" }, { "Friday", "Thứ 6" }, { "Saturday", "Thứ 7" }, { "Sunday", "Chủ nhật" }
                };

                var todayVietnamese = dayMap[today];

                var courses = await _context.Courses
                    .Where(c => c.LecturerId == lecturer.Id && c.Schedule != null && c.Schedule.Contains(todayVietnamese))
                    .Include(c => c.Enrollments)
                    .Select(c => new TodayScheduleDto
                    {
                        Id = c.Id,
                        CourseName = c.Name,
                        Time = c.Schedule ?? "Chưa cập nhật",
                        Room = c.Room ?? "Chưa cập nhật",
                        StudentCount = c.Enrollments.Count(e => e.Status == "Active" || e.Status == "Completed")
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {courses.Count} lịch dạy hôm nay");
                return Ok(courses);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy lịch dạy hôm nay: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy lịch dạy hôm nay" });
            }
        }

        // GET: api/lecturer/recent-attendance
        [HttpGet("recent-attendance")]
        public async Task<IActionResult> GetRecentAttendance()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy điểm danh gần đây...");

                var lecturer = await GetLecturerByEmail();
                if (lecturer == null) return Unauthorized();

                var courseIds = await _context.Courses
                    .Where(c => c.LecturerId == lecturer.Id)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (courseIds.Count == 0)
                {
                    _logger.LogWarning("⚠️ Giảng viên chưa có khóa học nào");
                    return Ok(new List<RecentAttendanceDto>());
                }

                var attendances = await _context.Attendances
                    .Where(a => courseIds.Contains(a.CourseId))
                    .Include(a => a.Course)
                    .OrderByDescending(a => a.Date)
                    .Take(50)
                    .ToListAsync();

                if (attendances.Count == 0)
                {
                    _logger.LogWarning("⚠️ Chưa có dữ liệu điểm danh");
                    return Ok(new List<RecentAttendanceDto>());
                }

                var groupedByCourse = attendances
                    .GroupBy(a => a.CourseId)
                    .Select(g => new
                    {
                        CourseId = g.Key,
                        CourseName = g.FirstOrDefault()?.Course?.Name ?? "Không xác định",
                        Total = g.Count(),
                        Present = g.Count(a => a.Status == "Present"),
                        Absent = g.Count(a => a.Status == "Absent"),
                        Late = g.Count(a => a.Status == "Late"),
                        LastDate = g.Max(a => a.Date)
                    })
                    .OrderByDescending(x => x.LastDate)
                    .Take(5)
                    .ToList();

                var result = new List<RecentAttendanceDto>();
                foreach (var item in groupedByCourse)
                {
                    var rate = item.Total > 0 ? Math.Round((double)item.Present / item.Total * 100, 0) : 0;

                    result.Add(new RecentAttendanceDto
                    {
                        Id = item.CourseId,
                        CourseName = item.CourseName,
                        Date = item.LastDate.ToString("dd/MM/yyyy"),
                        PresentCount = item.Present,
                        TotalCount = item.Total,
                        Rate = rate,
                        Status = rate >= 85 ? "present" : "absent"
                    });
                }

                _logger.LogInformation($"✅ Lấy {result.Count} điểm danh gần đây");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm danh gần đây: {ex.Message}");
                _logger.LogError($"❌ Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi khi lấy điểm danh gần đây", error = ex.Message });
            }
        }

        // GET: api/lecturer/average-grade
        [HttpGet("average-grade")]
        public async Task<IActionResult> GetAverageGrade()
        {
            try
            {
                _logger.LogInformation("🔄 Đang lấy điểm trung bình...");

                var lecturer = await GetLecturerByEmail();
                if (lecturer == null) return Unauthorized();

                var courseIds = await _context.Courses
                    .Where(c => c.LecturerId == lecturer.Id)
                    .Select(c => c.Id)
                    .ToListAsync();

                if (courseIds.Count == 0)
                {
                    _logger.LogWarning("⚠️ Giảng viên chưa có khóa học nào");
                    return Ok(new { average = 0 });
                }

                var enrollments = await _context.Enrollments
                    .Where(e => courseIds.Contains(e.CourseId) && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                if (enrollments.Count == 0)
                {
                    _logger.LogWarning("⚠️ Không có học viên nào");
                    return Ok(new { average = 0 });
                }

                var allScores = new List<double>();
                foreach (var enrollment in enrollments)
                {
                    var learningResults = enrollment.LearningResults.ToList();
                    if (learningResults.Any())
                    {
                        var finalResult = learningResults.FirstOrDefault(r => r.ExamType == "Final");
                        if (finalResult != null && finalResult.Score.HasValue)
                        {
                            allScores.Add(finalResult.Score.Value);
                        }
                        else
                        {
                            double totalWeightedScore = 0;
                            double totalWeight = 0;
                            foreach (var result in learningResults)
                            {
                                if (result.Score.HasValue)
                                {
                                    var weight = result.Weight ?? 1.0;
                                    totalWeightedScore += result.Score.Value * weight;
                                    totalWeight += weight;
                                }
                            }
                            if (totalWeight > 0)
                            {
                                allScores.Add(totalWeightedScore / totalWeight);
                            }
                        }
                    }
                }

                var average = allScores.Count > 0 ? Math.Round(allScores.Average(), 1) : 0;

                _logger.LogInformation($"✅ Điểm trung bình: {average}");
                return Ok(new { average });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm trung bình: {ex.Message}");
                return Ok(new { average = 0 });
            }
        }

        // GET: api/lecturer/top-students
        [HttpGet("top-students")]
        public async Task<IActionResult> GetTopStudents([FromQuery] int courseId)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy top học viên cho khóa {courseId}...");

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    _logger.LogWarning($"⚠️ Không tìm thấy khóa học ID {courseId}");
                    return Ok(new List<TopStudentDto>());
                }

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                if (enrollments.Count == 0)
                {
                    _logger.LogWarning($"⚠️ Không có học viên nào trong khóa {courseId}");
                    return Ok(new List<TopStudentDto>());
                }

                var topStudents = new List<TopStudentDto>();
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Student == null) continue;

                    double averageScore = 0;
                    var learningResults = enrollment.LearningResults.ToList();

                    if (learningResults.Any())
                    {
                        var finalResult = learningResults.FirstOrDefault(r => r.ExamType == "Final");
                        if (finalResult != null && finalResult.Score.HasValue)
                        {
                            averageScore = finalResult.Score.Value;
                        }
                        else
                        {
                            double totalWeightedScore = 0;
                            double totalWeight = 0;
                            foreach (var rs in learningResults)
                            {
                                if (rs.Score.HasValue)
                                {
                                    var weight = rs.Weight ?? 1.0;
                                    totalWeightedScore += rs.Score.Value * weight;
                                    totalWeight += weight;
                                }
                            }
                            averageScore = totalWeight > 0 ? totalWeightedScore / totalWeight : 0;
                        }
                    }

                    topStudents.Add(new TopStudentDto
                    {
                        Id = enrollment.Student.Id,
                        Name = enrollment.Student.FullName,
                        StudentId = enrollment.Student.StudentId,
                        AverageScore = Math.Round(averageScore, 1)
                    });
                }

                var result = topStudents
                    .OrderByDescending(s => s.AverageScore)
                    .Take(5)
                    .ToList();

                _logger.LogInformation($"✅ Lấy {result.Count} top học viên");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy top học viên: {ex.Message}");
                _logger.LogError($"❌ Stack trace: {ex.StackTrace}");
                return Ok(new List<TopStudentDto>());
            }
        }

        #endregion

        #region Student Management APIs

        // GET: api/lecturer/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] int courseId, [FromQuery] string? search)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy danh sách học viên cho khóa {courseId}...");

                var query = _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .Include(e => e.LearningResults)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(e => e.Student != null &&
                        (e.Student.FullName.Contains(search) || e.Student.StudentId.Contains(search)));
                }

                var enrollments = await query.ToListAsync();
                var students = new List<LecturerStudentDto>();

                foreach (var enrollment in enrollments)
                {
                    var attendances = await _context.Attendances
                        .Where(a => a.StudentId == enrollment.StudentId && a.CourseId == courseId)
                        .ToListAsync();

                    var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                    var averageGrade = finalResult?.Score ?? enrollment.LearningResults.Average(r => r.Score ?? 0);
                    var attendanceRate = attendances.Count > 0
                        ? Math.Round((double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100, 0)
                        : 0;

                    students.Add(new LecturerStudentDto
                    {
                        Id = enrollment.Student!.Id,
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Email = enrollment.Student.Email,
                        Phone = enrollment.Student.Phone,
                        Faculty = enrollment.Student.Faculty,
                        Class = enrollment.Student.Class,
                        Attendance = attendanceRate,
                        AverageGrade = Math.Round(averageGrade, 1),
                        Rank = GetRankByScore(averageGrade)
                    });
                }

                _logger.LogInformation($"✅ Lấy {students.Count} học viên");
                return Ok(new { items = students, total = students.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy danh sách học viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách học viên" });
            }
        }

        // GET: api/lecturer/students/{id}/grades
        [HttpGet("students/{id}/grades")]
        public async Task<IActionResult> GetStudentGrades(int id)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy điểm của học viên ID {id}...");

                var learningResults = await _context.LearningResults
                    .Where(r => r.Enrollment != null && r.Enrollment.StudentId == id)
                    .Include(r => r.Enrollment)
                    .ThenInclude(e => e!.Course)
                    .Select(r => new
                    {
                        r.Id,
                        CourseName = r.Enrollment != null && r.Enrollment.Course != null ? r.Enrollment.Course.Name : "Không xác định",
                        r.ExamType,
                        r.Score,
                        r.Grade
                    })
                    .ToListAsync();

                _logger.LogInformation($"✅ Lấy {learningResults.Count} điểm của học viên");
                return Ok(learningResults);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm học viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy điểm học viên" });
            }
        }

        #endregion

        #region Attendance Management APIs

        // GET: api/lecturer/attendance/students
        [HttpGet("attendance/students")]
        public async Task<IActionResult> GetAttendanceStudents([FromQuery] int courseId, [FromQuery] string? date)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy danh sách học viên cho khóa {courseId} để điểm danh...");

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    _logger.LogWarning($"⚠️ Không tìm thấy khóa học ID {courseId}");
                    return Ok(new List<AttendanceStudentDto>());
                }

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .ToListAsync();

                if (enrollments.Count == 0)
                {
                    _logger.LogWarning($"⚠️ Không có học viên nào trong khóa {courseId}");
                    return Ok(new List<AttendanceStudentDto>());
                }

                DateTime attendanceDate = DateTime.UtcNow.Date;
                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var parsedDate))
                {
                    attendanceDate = parsedDate.Date;
                }

                var students = new List<AttendanceStudentDto>();
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Student == null) continue;

                    var existingAttendance = await _context.Attendances
                        .FirstOrDefaultAsync(a => a.StudentId == enrollment.StudentId
                            && a.CourseId == courseId
                            && a.Date.Date == attendanceDate);

                    students.Add(new AttendanceStudentDto
                    {
                        Id = enrollment.Student.Id,
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Status = existingAttendance?.Status ?? "present",
                        CheckInTime = existingAttendance?.CheckInTime?.ToString("HH:mm") ?? "",
                        Note = existingAttendance?.Note ?? ""
                    });
                }

                _logger.LogInformation($"✅ Lấy {students.Count} học viên để điểm danh");
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy danh sách học viên điểm danh: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách học viên" });
            }
        }

        // GET: api/lecturer/attendance/history
        [HttpGet("attendance/history")]
        public async Task<IActionResult> GetAttendanceHistory([FromQuery] int courseId)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy lịch sử điểm danh cho khóa {courseId}...");

                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    _logger.LogWarning($"⚠️ Không tìm thấy khóa học ID {courseId}");
                    return Ok(new List<AttendanceHistoryDto>());
                }

                var attendances = await _context.Attendances
                    .Where(a => a.CourseId == courseId)
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();

                if (attendances.Count == 0)
                {
                    _logger.LogWarning($"⚠️ Không có dữ liệu điểm danh cho khóa {courseId}");
                    return Ok(new List<AttendanceHistoryDto>());
                }

                var totalStudentsInCourse = await _context.Enrollments
                    .CountAsync(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"));

                var groupedByDate = attendances
                    .GroupBy(a => a.Date.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Total = totalStudentsInCourse,
                        Present = g.Count(a => a.Status == "Present"),
                        Absent = g.Count(a => a.Status == "Absent"),
                        Late = g.Count(a => a.Status == "Late")
                    })
                    .OrderByDescending(x => x.Date)
                    .Take(10)
                    .ToList();

                var result = new List<AttendanceHistoryDto>();
                int index = 1;
                foreach (var item in groupedByDate)
                {
                    var rate = item.Total > 0 ? Math.Round((double)item.Present / item.Total * 100, 0) : 0;

                    result.Add(new AttendanceHistoryDto
                    {
                        Id = index++,
                        Date = item.Date.ToString("dd/MM/yyyy"),
                        Session = $"Buổi học ngày {item.Date:dd/MM/yyyy}",
                        Total = item.Total,
                        Present = item.Present,
                        Absent = item.Absent,
                        Late = item.Late,
                        Rate = rate
                    });
                }

                _logger.LogInformation($"✅ Lấy {result.Count} lịch sử điểm danh");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy lịch sử điểm danh: {ex.Message}");
                _logger.LogError($"❌ Stack trace: {ex.StackTrace}");

                var mockData = new List<AttendanceHistoryDto>
                {
                    new AttendanceHistoryDto
                    {
                        Id = 1,
                        Date = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy"),
                        Session = "Buổi 1 - Tuần 12",
                        Total = 25,
                        Present = 20,
                        Absent = 3,
                        Late = 2,
                        Rate = 80
                    },
                    new AttendanceHistoryDto
                    {
                        Id = 2,
                        Date = DateTime.Now.AddDays(-14).ToString("dd/MM/yyyy"),
                        Session = "Buổi 2 - Tuần 11",
                        Total = 25,
                        Present = 22,
                        Absent = 2,
                        Late = 1,
                        Rate = 88
                    },
                    new AttendanceHistoryDto
                    {
                        Id = 3,
                        Date = DateTime.Now.AddDays(-21).ToString("dd/MM/yyyy"),
                        Session = "Buổi 1 - Tuần 11",
                        Total = 25,
                        Present = 18,
                        Absent = 5,
                        Late = 2,
                        Rate = 72
                    }
                };
                return Ok(mockData);
            }
        }

        // POST: api/lecturer/attendance/save
        [HttpPost("attendance/save")]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequest request)
        {
            try
            {
                _logger.LogInformation($"📊 Đang lưu điểm danh cho khóa {request.CourseId}, ngày {request.Date}");

                var attendanceDate = DateTime.UtcNow.Date;
                if (!string.IsNullOrEmpty(request.Date) && DateTime.TryParse(request.Date, out var parsedDate))
                {
                    attendanceDate = parsedDate.Date;
                }

                foreach (var student in request.Records)
                {
                    var existingAttendance = await _context.Attendances
                        .FirstOrDefaultAsync(a => a.StudentId == student.Id
                            && a.CourseId == request.CourseId
                            && a.Date.Date == attendanceDate);

                    if (existingAttendance != null)
                    {
                        existingAttendance.Status = student.Status;
                        existingAttendance.CheckInTime = !string.IsNullOrEmpty(student.CheckInTime)
                            ? DateTime.ParseExact(student.CheckInTime, "HH:mm", null)
                            : null;
                        existingAttendance.Note = student.Note;
                    }
                    else
                    {
                        var attendance = new Attendance
                        {
                            StudentId = student.Id,
                            CourseId = request.CourseId,
                            Date = attendanceDate,
                            Status = student.Status,
                            CheckInTime = !string.IsNullOrEmpty(student.CheckInTime)
                                ? DateTime.ParseExact(student.CheckInTime, "HH:mm", null)
                                : null,
                            Note = student.Note,
                            Week = GetCurrentWeek()
                        };
                        _context.Attendances.Add(attendance);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Lưu điểm danh thành công cho {request.Records.Count} học viên");

                return Ok(new { message = "Attendance saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lưu điểm danh: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lưu điểm danh", error = ex.Message });
            }
        }

        #endregion

        #region Grade Management APIs

        // GET: api/lecturer/grades/students
        [HttpGet("grades/students")]
        public async Task<IActionResult> GetGradeStudents([FromQuery] int courseId)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy danh sách học viên để nhập điểm cho khóa {courseId}...");

                // Kiểm tra khóa học tồn tại
                var course = await _context.Courses.FindAsync(courseId);
                if (course == null)
                {
                    _logger.LogWarning($"⚠️ Không tìm thấy khóa học ID {courseId}");
                    return Ok(new List<StudentGradeDto>());
                }

                // Lấy danh sách học viên đăng ký khóa học
                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                if (enrollments.Count == 0)
                {
                    _logger.LogWarning($"⚠️ Không có học viên nào trong khóa {courseId}");
                    return Ok(new List<StudentGradeDto>());
                }

                var students = new List<StudentGradeDto>();
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Student == null) continue;

                    // Lấy tất cả LearningResults của học viên
                    var learningResults = enrollment.LearningResults.ToList();

                    // Tìm điểm theo từng loại
                    var midtermResult = learningResults.FirstOrDefault(r => r.ExamType == "Midterm");
                    var finalResult = learningResults.FirstOrDefault(r => r.ExamType == "Final");
                    var projectResult = learningResults.FirstOrDefault(r => r.ExamType == "Project");
                    var quiz1Result = learningResults.FirstOrDefault(r => r.ExamType == "Quiz1");
                    var quiz2Result = learningResults.FirstOrDefault(r => r.ExamType == "Quiz2");

                    students.Add(new StudentGradeDto
                    {
                        Id = enrollment.Student.Id,
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Email = enrollment.Student.Email,
                        Phone = enrollment.Student.Phone,
                        Faculty = enrollment.Student.Faculty,
                        Class = enrollment.Student.Class,
                        // Điểm theo từng loại
                        MidtermScore = midtermResult?.Score ?? 0,
                        FinalScore = finalResult?.Score ?? 0,
                        ProjectScore = projectResult?.Score ?? 0,
                        Quiz1Score = quiz1Result?.Score ?? 0,
                        Quiz2Score = quiz2Result?.Score ?? 0,
                        // Điểm chữ theo từng loại
                        MidtermGrade = midtermResult?.Grade ?? "Chưa có",
                        FinalGrade = finalResult?.Grade ?? "Chưa có",
                        ProjectGrade = projectResult?.Grade ?? "Chưa có",
                        Quiz1Grade = quiz1Result?.Grade ?? "Chưa có",
                        Quiz2Grade = quiz2Result?.Grade ?? "Chưa có",
                        // GPA và xếp loại tổng
                        GPA = CalculateGPA(learningResults),
                        Rank = GetRankByScore(CalculateGPA(learningResults) * 2.5)
                    });
                }

                _logger.LogInformation($"✅ Lấy {students.Count} học viên để nhập điểm");
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy danh sách học viên nhập điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách học viên" });
            }
        }

        // GET: api/lecturer/grades
        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades([FromQuery] int courseId, [FromQuery] string examType)
        {
            try
            {
                _logger.LogInformation($"🔄 Đang lấy điểm loại {examType} cho khóa {courseId}...");

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                var students = new List<StudentGradeDto>();
                foreach (var enrollment in enrollments)
                {
                    if (enrollment.Student == null) continue;

                    var existingResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == examType);
                    students.Add(new StudentGradeDto
                    {
                        Id = enrollment.Student.Id,
                        StudentId = enrollment.Student.StudentId,
                        FullName = enrollment.Student.FullName,
                        Score = existingResult?.Score ?? 0,
                        LetterGrade = existingResult?.Grade ?? "Chưa có",
                        Rank = GetRankByScore(existingResult?.Score ?? 0),
                        Note = existingResult?.Comment,
                        ExamType = examType
                    });
                }

                _logger.LogInformation($"✅ Lấy điểm cho {students.Count} học viên");
                return Ok(students);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy điểm" });
            }
        }

        // POST: api/lecturer/grades
        [HttpPost("grades")]
        public async Task<IActionResult> SaveGrade([FromBody] SaveGradeRequest request)
        {
            try
            {
                _logger.LogInformation($"📊 Đang lưu điểm cho học viên {request.StudentId}, khóa {request.CourseId}");

                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);

                if (enrollment == null)
                    return BadRequest(new { message = "Học viên chưa đăng ký khóa học này" });

                var existingResult = await _context.LearningResults
                    .FirstOrDefaultAsync(r => r.EnrollmentId == enrollment.Id && r.ExamType == request.ExamType);

                if (existingResult != null)
                {
                    existingResult.Score = request.Score;
                    existingResult.MaxScore = request.MaxScore;
                    existingResult.Weight = request.Weight;
                    existingResult.Grade = CalculateLetterGrade(request.Score);
                    existingResult.Comment = request.Note;
                }
                else
                {
                    var result = new LearningResult
                    {
                        EnrollmentId = enrollment.Id,
                        ExamType = request.ExamType,
                        Score = request.Score,
                        MaxScore = request.MaxScore,
                        Weight = request.Weight,
                        Grade = CalculateLetterGrade(request.Score),
                        Comment = request.Note,
                        RecordedDate = DateTime.UtcNow
                    };
                    _context.LearningResults.Add(result);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Lưu điểm thành công cho học viên {request.StudentId}");
                return Ok(new { message = "Grade saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lưu điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lưu điểm" });
            }
        }

        // POST: api/lecturer/grades/save
        [HttpPost("grades/save")]
        public async Task<IActionResult> SaveMultipleGrades([FromBody] SaveMultipleGradesRequest request)
        {
            try
            {
                _logger.LogInformation($"📊 Đang lưu nhiều điểm cho khóa {request.CourseId}, loại {request.ExamType}");

                if (request.Grades == null || request.Grades.Count == 0)
                {
                    return BadRequest(new { message = "Không có dữ liệu điểm để lưu" });
                }

                var examType = request.ExamType;
                var courseId = request.CourseId;

                foreach (var grade in request.Grades)
                {
                    // Tìm enrollment của học viên trong khóa học
                    var enrollment = await _context.Enrollments
                        .FirstOrDefaultAsync(e => e.StudentId == grade.StudentId && e.CourseId == courseId);

                    if (enrollment == null)
                    {
                        _logger.LogWarning($"⚠️ Không tìm thấy enrollment cho học viên {grade.StudentId} trong khóa {courseId}");
                        continue;
                    }

                    // Tìm LearningResult hiện có
                    var existingResult = await _context.LearningResults
                        .FirstOrDefaultAsync(r => r.EnrollmentId == enrollment.Id && r.ExamType == examType);

                    if (existingResult != null)
                    {
                        // Cập nhật điểm hiện có
                        existingResult.Score = grade.Score;
                        existingResult.Grade = CalculateLetterGrade(grade.Score);
                        existingResult.Comment = grade.Note;
                        existingResult.RecordedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        // Tạo mới LearningResult
                        var result = new LearningResult
                        {
                            EnrollmentId = enrollment.Id,
                            ExamType = examType,
                            Score = grade.Score,
                            MaxScore = 10,
                            Weight = GetExamWeight(examType),
                            Grade = CalculateLetterGrade(grade.Score),
                            Comment = grade.Note,
                            RecordedDate = DateTime.UtcNow
                        };
                        _context.LearningResults.Add(result);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"✅ Lưu điểm thành công cho {request.Grades.Count} học viên");

                return Ok(new { message = "Grades saved successfully", count = request.Grades.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lưu nhiều điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lưu điểm", error = ex.Message });
            }
        }


        // Helper method để tính GPA
        private double CalculateGPA(List<LearningResult> results)
        {
            if (results == null || results.Count == 0) return 0;

            double totalWeightedScore = 0;
            double totalWeight = 0;

            foreach (var result in results)
            {
                if (result.Score.HasValue)
                {
                    var weight = result.Weight ?? 1.0;
                    totalWeightedScore += result.Score.Value * weight;
                    totalWeight += weight;
                }
            }

            return totalWeight > 0 ? Math.Round(totalWeightedScore / totalWeight / 2.5, 2) : 0;
        }

        // Helper method để lấy trọng số theo loại điểm
        private double GetExamWeight(string examType)
        {
            return examType switch
            {
                "Midterm" => 0.25,
                "Final" => 0.40,
                "Project" => 0.20,
                "Quiz1" => 0.10,
                "Quiz2" => 0.10,
                _ => 0.25
            };
        }

        // GET: api/lecturer/grades/export
        [HttpGet("grades/export")]
        public async Task<IActionResult> ExportGrades([FromQuery] int courseId, [FromQuery] string examType)
        {
            try
            {
                _logger.LogInformation($"📊 Đang xuất điểm cho khóa {courseId}, loại {examType}");

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .Include(e => e.LearningResults)
                    .ToListAsync();

                var csv = "Mã học viên,Họ tên,Điểm,Điểm chữ,Ghi chú\n";
                foreach (var enrollment in enrollments)
                {
                    var learningResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == examType);
                    var score = learningResult?.Score ?? 0;
                    var grade = learningResult?.Grade ?? "Chưa có";
                    var comment = learningResult?.Comment ?? "";

                    csv += $"{enrollment.Student?.StudentId},{enrollment.Student?.FullName},{score},{grade},{comment}\n";
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"grades_{courseId}_{examType}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi xuất điểm: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi xuất điểm" });
            }
        }

        // GET: api/lecturer/students/export
        [HttpGet("students/export")]
        public async Task<IActionResult> ExportStudents([FromQuery] int courseId)
        {
            try
            {
                _logger.LogInformation($"📊 Đang xuất danh sách học viên cho khóa {courseId}");

                var enrollments = await _context.Enrollments
                    .Where(e => e.CourseId == courseId && (e.Status == "Active" || e.Status == "Completed"))
                    .Include(e => e.Student)
                    .ToListAsync();

                var csv = "Mã học viên,Họ tên,Email,Điện thoại,Lớp,Khóa học\n";
                foreach (var enrollment in enrollments)
                {
                    csv += $"{enrollment.Student?.StudentId},{enrollment.Student?.FullName},{enrollment.Student?.Email},{enrollment.Student?.Phone},{enrollment.Student?.Class},{enrollment.Student?.Faculty}\n";
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"students_{courseId}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi xuất danh sách học viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi xuất danh sách học viên" });
            }
        }

        // GET: api/lecturer/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var lecturer = await GetLecturerByEmail();
                if (lecturer == null)
                    return NotFound(new { message = "Lecturer not found" });

                var result = new
                {
                    lecturer.Id,
                    lecturer.LecturerId,
                    lecturer.FullName,
                    lecturer.Email,
                    lecturer.Phone,
                    lecturer.Faculty,
                    lecturer.Title,
                    lecturer.Specialization,
                    Address = "Chưa cập nhật"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Lỗi lấy thông tin giảng viên: {ex.Message}");
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin giảng viên" });
            }
        }

        #endregion

        #region Private Methods        
        private int GetCurrentWeek()
        {
            var startDate = new DateTime(2024, 9, 1);
            var daysSinceStart = (DateTime.UtcNow - startDate).Days;
            return (daysSinceStart / 7) + 1;
        }

        private string CalculateLetterGrade(double score)
        {
            if (score >= 8.5) return "A";
            if (score >= 7.0) return "B";
            if (score >= 5.5) return "C";
            if (score >= 4.0) return "D";
            return "F";
        }

        private string GetRankByScore(double score)
        {
            if (score >= 8.5) return "Xuất sắc";
            if (score >= 7.0) return "Giỏi";
            if (score >= 5.5) return "Khá";
            if (score >= 4.0) return "Trung bình";
            return "Yếu";
        }

        #endregion
    }

    #region Request DTOs

    public class SaveAttendanceRequest
    {
        public int CourseId { get; set; }
        public string? Date { get; set; }
        public List<AttendanceStudentRecord> Records { get; set; } = new();
    }

    public class AttendanceStudentRecord
    {
        public int Id { get; set; }
        public string Status { get; set; } = "present";
        public string? CheckInTime { get; set; }
        public string? Note { get; set; }
    }

    public class SaveGradeRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public double Score { get; set; }
        public double MaxScore { get; set; } = 10;
        public double Weight { get; set; } = 0.25;
        public string? Note { get; set; }
    }

    public class SaveMultipleGradesRequest
    {
        public int CourseId { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public List<StudentGradeRecord> Grades { get; set; } = new();
    }

    public class StudentGradeRecord
    {
        public int StudentId { get; set; }
        public double Score { get; set; }
        public string? Note { get; set; }
    }

    #endregion
}
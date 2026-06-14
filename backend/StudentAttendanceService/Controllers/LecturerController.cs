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
            var lecturer = await GetLecturerByEmail();
            if (lecturer == null) return Unauthorized();

            var courses = await _context.Courses
                .Where(c => c.LecturerId == lecturer.Id && c.Semester == "Fall 2024")
                .Include(c => c.Enrollments)
                .Select(c => new LecturerCourseDto
                {
                    Id = c.Id,
                    CourseName = c.Name,
                    Code = c.Code,
                    Credits = c.Credits,
                    StudentCount = c.Enrollments.Count(e => e.Status == "Active"),
                    Schedule = c.Schedule ?? "Chưa cập nhật",
                    Room = c.Room ?? "Chưa cập nhật",
                    Faculty = c.Faculty,
                    Semester = c.Semester
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/lecturer/today-schedule
        [HttpGet("today-schedule")]
        public async Task<IActionResult> GetTodaySchedule()
        {
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
                    StudentCount = c.Enrollments.Count(e => e.Status == "Active")
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/lecturer/recent-attendance
        [HttpGet("recent-attendance")]
        public async Task<IActionResult> GetRecentAttendance()
        {
            var lecturer = await GetLecturerByEmail();
            if (lecturer == null) return Unauthorized();

            var courses = await _context.Courses
                .Where(c => c.LecturerId == lecturer.Id)
                .Select(c => c.Id)
                .ToListAsync();

            var recentAttendances = await _context.Attendances
                .Where(a => courses.Contains(a.CourseId))
                .GroupBy(a => a.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    Total = g.Count(),
                    Present = g.Count(a => a.Status == "Present"),
                    Absent = g.Count(a => a.Status == "Absent"),
                    Late = g.Count(a => a.Status == "Late"),
                    LastDate = g.Max(a => a.Date)
                })
                .OrderByDescending(x => x.LastDate)
                .Take(5)
                .ToListAsync();

            var result = new List<RecentAttendanceDto>();
            foreach (var item in recentAttendances)
            {
                var course = await _context.Courses.FindAsync(item.CourseId);
                var rate = item.Total > 0 ? Math.Round((double)item.Present / item.Total * 100, 0) : 0;

                result.Add(new RecentAttendanceDto
                {
                    Id = item.CourseId,
                    CourseName = course?.Name ?? "Không xác định",
                    Date = item.LastDate.ToString("dd/MM/yyyy"),
                    PresentCount = item.Present,
                    TotalCount = item.Total,
                    Rate = rate,
                    Status = rate >= 85 ? "present" : "absent"
                });
            }

            return Ok(result);
        }

        // GET: api/lecturer/average-grade
        [HttpGet("average-grade")]
        public async Task<IActionResult> GetAverageGrade()
        {
            var lecturer = await GetLecturerByEmail();
            if (lecturer == null) return Unauthorized();

            var courses = await _context.Courses
                .Where(c => c.LecturerId == lecturer.Id)
                .Select(c => c.Id)
                .ToListAsync();

            var enrollments = await _context.Enrollments
                .Where(e => courses.Contains(e.CourseId) && e.Status == "Active")
                .Include(e => e.LearningResults)
                .ToListAsync();

            var allScores = new List<double>();
            foreach (var enrollment in enrollments)
            {
                var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                if (finalResult != null && finalResult.Score.HasValue)
                    allScores.Add(finalResult.Score.Value);
                else if (enrollment.LearningResults.Any())
                    allScores.Add(enrollment.LearningResults.Average(r => r.Score ?? 0));
            }

            var average = allScores.Count > 0 ? Math.Round(allScores.Average(), 1) : 0;
            return Ok(new { average });
        }

        // GET: api/lecturer/top-students
        [HttpGet("top-students")]
        public async Task<IActionResult> GetTopStudents([FromQuery] int courseId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .Include(e => e.LearningResults)
                .ToListAsync();

            var topStudents = new List<TopStudentDto>();
            foreach (var enrollment in enrollments)
            {
                var finalResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                var averageScore = finalResult?.Score ?? enrollment.LearningResults.Average(r => r.Score ?? 0);

                topStudents.Add(new TopStudentDto
                {
                    Id = enrollment.Student!.Id,
                    Name = enrollment.Student.FullName,
                    StudentId = enrollment.Student.StudentId,
                    AverageScore = Math.Round(averageScore, 1)
                });
            }

            return Ok(topStudents.OrderByDescending(s => s.AverageScore).Take(5));
        }

        #endregion

        #region Student Management APIs

        // GET: api/lecturer/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] int courseId, [FromQuery] string? search)
        {
            var query = _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
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

            return Ok(new { items = students, total = students.Count });
        }

        // GET: api/lecturer/students/{id}/grades
        [HttpGet("students/{id}/grades")]
        public async Task<IActionResult> GetStudentGrades(int id)
        {
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

            return Ok(learningResults);
        }

        #endregion

        #region Attendance Management APIs

        // GET: api/lecturer/attendance/students
        [HttpGet("attendance/students")]
        public async Task<IActionResult> GetAttendanceStudents([FromQuery] int courseId, [FromQuery] string? date)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .ToListAsync();

            var students = enrollments.Select(e => new AttendanceStudentDto
            {
                Id = e.Student!.Id,
                StudentId = e.Student.StudentId,
                FullName = e.Student.FullName,
                Status = "present",
                CheckInTime = DateTime.Now.ToString("HH:mm"),
                Note = ""
            }).ToList();

            return Ok(students);
        }

        // GET: api/lecturer/attendance/history
        [HttpGet("attendance/history")]
        public async Task<IActionResult> GetAttendanceHistory([FromQuery] int courseId)
        {
            var attendances = await _context.Attendances
                .Where(a => a.CourseId == courseId)
                .GroupBy(a => a.Date.Date)
                .Select(g => new AttendanceHistoryDto
                {
                    Id = g.Key.GetHashCode(),
                    Date = g.Key.ToString("dd/MM/yyyy"),
                    Session = $"Buổi học ngày {g.Key:dd/MM}",
                    Total = g.Count(),
                    Present = g.Count(a => a.Status == "Present"),
                    Absent = g.Count(a => a.Status == "Absent"),
                    Late = g.Count(a => a.Status == "Late"),
                    Rate = g.Count() > 0 ? Math.Round((double)g.Count(a => a.Status == "Present") / g.Count() * 100, 0) : 0
                })
                .OrderByDescending(h => h.Date)
                .ToListAsync();

            return Ok(attendances);
        }

        // POST: api/lecturer/attendance/save
        [HttpPost("attendance/save")]
        public async Task<IActionResult> SaveAttendance([FromBody] SaveAttendanceRequest request)
        {
            foreach (var student in request.Records)
            {
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.CourseId == request.CourseId && a.Date.Date == DateTime.UtcNow.Date);

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
                        Date = DateTime.UtcNow,
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
            return Ok(new { message = "Attendance saved successfully" });
        }

        #endregion

        #region Grade Management APIs

        // GET: api/lecturer/grades/students
        [HttpGet("grades/students")]
        public async Task<IActionResult> GetGradeStudents([FromQuery] int courseId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .Include(e => e.LearningResults)
                .ToListAsync();

            var students = new List<StudentGradeDto>();
            foreach (var enrollment in enrollments)
            {
                var existingResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == "Final");
                students.Add(new StudentGradeDto
                {
                    Id = enrollment.Student!.Id,
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Score = existingResult?.Score ?? 0,
                    LetterGrade = existingResult?.Grade ?? "Chưa có",
                    Rank = GetRankByScore(existingResult?.Score ?? 0),
                    Note = existingResult?.Comment
                });
            }

            return Ok(students);
        }

        // GET: api/lecturer/grades
        [HttpGet("grades")]
        public async Task<IActionResult> GetGrades([FromQuery] int courseId, [FromQuery] string examType)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .Include(e => e.LearningResults)
                .ToListAsync();

            var students = new List<StudentGradeDto>();
            foreach (var enrollment in enrollments)
            {
                var existingResult = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == examType);
                students.Add(new StudentGradeDto
                {
                    Id = enrollment.Student!.Id,
                    StudentId = enrollment.Student.StudentId,
                    FullName = enrollment.Student.FullName,
                    Score = existingResult?.Score ?? 0,
                    LetterGrade = existingResult?.Grade ?? "Chưa có",
                    Rank = GetRankByScore(existingResult?.Score ?? 0),
                    Note = existingResult?.Comment
                });
            }

            return Ok(students);
        }

        // POST: api/lecturer/grades
        [HttpPost("grades")]
        public async Task<IActionResult> SaveGrade([FromBody] SaveGradeRequest request)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);

            if (enrollment == null)
                return BadRequest(new { message = "Student not enrolled in this course" });

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
            return Ok(new { message = "Grade saved successfully" });
        }

        // POST: api/lecturer/grades/save
        [HttpPost("grades/save")]
        public async Task<IActionResult> SaveMultipleGrades([FromBody] SaveMultipleGradesRequest request)
        {
            foreach (var grade in request.Grades)
            {
                var enrollment = await _context.Enrollments
                    .FirstOrDefaultAsync(e => e.StudentId == grade.StudentId && e.CourseId == request.CourseId);

                if (enrollment == null) continue;

                var existingResult = await _context.LearningResults
                    .FirstOrDefaultAsync(r => r.EnrollmentId == enrollment.Id && r.ExamType == request.ExamType);

                if (existingResult != null)
                {
                    existingResult.Score = grade.Score;
                    existingResult.Grade = CalculateLetterGrade(grade.Score);
                    existingResult.Comment = grade.Note;
                }
                else
                {
                    var result = new LearningResult
                    {
                        EnrollmentId = enrollment.Id,
                        ExamType = request.ExamType,
                        Score = grade.Score,
                        MaxScore = 10,
                        Weight = 0.25,
                        Grade = CalculateLetterGrade(grade.Score),
                        Comment = grade.Note,
                        RecordedDate = DateTime.UtcNow
                    };
                    _context.LearningResults.Add(result);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Grades saved successfully" });
        }

        // GET: api/lecturer/grades/export
        [HttpGet("grades/export")]
        public async Task<IActionResult> ExportGrades([FromQuery] int courseId, [FromQuery] string examType)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .Include(e => e.LearningResults)
                .ToListAsync();

            var csv = "Mã sinh viên,Họ tên,Điểm,Điểm chữ,Ghi chú\n";
            foreach (var enrollment in enrollments)
            {
                var result = enrollment.LearningResults.FirstOrDefault(r => r.ExamType == examType);
                csv += $"{enrollment.Student?.StudentId},{enrollment.Student?.FullName},{result?.Score ?? 0},{result?.Grade ?? "Chưa có"},{result?.Comment}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"grades_{courseId}_{examType}.csv");
        }

        // GET: api/lecturer/students/export
        [HttpGet("students/export")]
        public async Task<IActionResult> ExportStudents([FromQuery] int courseId)
        {
            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId && e.Status == "Active")
                .Include(e => e.Student)
                .ToListAsync();

            var csv = "Mã sinh viên,Họ tên,Email,Điện thoại,Lớp,Khoa\n";
            foreach (var enrollment in enrollments)
            {
                csv += $"{enrollment.Student?.StudentId},{enrollment.Student?.FullName},{enrollment.Student?.Email},{enrollment.Student?.Phone},{enrollment.Student?.Class},{enrollment.Student?.Faculty}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"students_{courseId}.csv");
        }

        // GET: api/lecturer/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
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
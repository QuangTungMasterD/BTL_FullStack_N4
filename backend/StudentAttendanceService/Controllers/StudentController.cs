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

        // GET: api/student/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.Status == "Active")
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
                ? (double)attendances.Count(a => a.Status == "Present") / attendances.Count * 100 
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

            return Ok(stats);
        }

        // GET: api/student/courses
        [HttpGet("courses")]
        public async Task<IActionResult> GetCurrentCourses()
        {
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

            return Ok(courses);
        }

        // GET: api/student/upcoming-classes
        [HttpGet("upcoming-classes")]
        public async Task<IActionResult> GetUpcomingClasses()
        {
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
                Day = e.Course.Schedule?.Split(',')[0] ?? "Chưa cập nhật",
                Time = e.Course.Schedule ?? "Chưa cập nhật",
                Room = e.Course.Room ?? "Chưa cập nhật",
                Lecturer = e.Course.Lecturer?.FullName ?? "Chưa cập nhật"
            }).Take(5).ToList();

            return Ok(upcoming);
        }

        // GET: api/student/recent-grades
        [HttpGet("recent-grades")]
        public async Task<IActionResult> GetRecentGrades()
        {
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

            return Ok(grades);
        }

        // GET: api/student/announcements
        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            var announcements = await _context.Announcements
                .Where(a => a.TargetRole == "ALL" || a.TargetRole == "STUDENT")
                .OrderByDescending(a => a.Date)
                .Take(5)
                .ToListAsync();

            return Ok(announcements);
        }

        // GET: api/student/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
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

        // GET: api/student/enrolled-courses
        [HttpGet("enrolled-courses")]
        public async Task<IActionResult> GetEnrolledCourses()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.Status == "Active")
                .Include(e => e.Course)
                .ToListAsync();

            var courses = enrollments.Select(e => new
            {
                e.Id,
                e.Course!.Code,
                e.Course.Name,
                e.Course.Credits,
                e.Course.Schedule,
                e.Course.Room
            });

            return Ok(courses);
        }

        // GET: api/student/attendance/overall
        [HttpGet("attendance/overall")]
        public async Task<IActionResult> GetOverallAttendance()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var attendances = await _context.Attendances
                .Where(a => a.StudentId == student.Id)
                .ToListAsync();

            var total = attendances.Count;
            var present = attendances.Count(a => a.Status == "Present");
            var absent = attendances.Count(a => a.Status == "Absent");
            var late = attendances.Count(a => a.Status == "Late");
            var percentage = total > 0 ? Math.Round((double)present / total * 100, 0) : 0;

            return Ok(new { total, present, absent, late, percentage });
        }

        // GET: api/student/attendance/summary
        [HttpGet("attendance/summary")]
        public async Task<IActionResult> GetAttendanceSummary([FromQuery] string? semester)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.Status == "Active")
                .Include(e => e.Course)
                .ToListAsync();

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
                    TotalSessions = total,
                    Present = present,
                    Absent = absent,
                    Late = late,
                    Percentage = percentage,
                    Color = GetColorByPercentage(percentage)
                });
            }

            return Ok(summary);
        }

        // GET: api/student/attendance/records
        [HttpGet("attendance/records")]
        public async Task<IActionResult> GetAttendanceRecords([FromQuery] int? courseId)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var attendances = await _context.Attendances
                .Where(a => a.StudentId == student.Id)
                .ToListAsync();

            var courseIds = attendances.Select(a => a.CourseId).Distinct().ToList();
            var courses = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            if (courseId.HasValue)
            {
                attendances = attendances.Where(a => a.CourseId == courseId.Value).ToList();
            }

            var records = attendances
                .OrderByDescending(a => a.Date)
                .Select(a => new
                {
                    a.Id,
                    a.Date,
                    CourseName = courses.ContainsKey(a.CourseId) ? courses[a.CourseId] : "Không xác định",
                    a.CheckInTime,
                    a.CheckOutTime,
                    a.Status,
                    a.Week
                })
                .ToList();

            return Ok(records);
        }

        // GET: api/student/gpa/summary
        [HttpGet("gpa/summary")]
        public async Task<IActionResult> GetGpaSummary()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id)
                .Include(e => e.Course)
                .ToListAsync();

            var learningResults = await _context.LearningResults
                .Where(r => r.Enrollment != null && r.Enrollment.StudentId == student.Id)
                .ToListAsync();

            var gpa = CalculateGPA(learningResults, enrollments);
            var totalCredits = enrollments.Where(e => e.Status == "Completed").Sum(e => e.Course?.Credits ?? 0);
            var coursesCount = enrollments.Count;

            return Ok(new
            {
                Gpa = gpa,
                TotalCredits = totalCredits,
                CoursesCount = coursesCount,
                Rank = GetRank(gpa)
            });
        }

        // GET: api/student/gpa/history
        [HttpGet("gpa/history")]
        public IActionResult GetGpaHistory()
        {
            var history = new List<GpaHistoryDto>
            {
                new GpaHistoryDto { Semester = "Spring 2023", Gpa = 3.45, Credits = 18, Rank = "Good" },
                new GpaHistoryDto { Semester = "Fall 2023", Gpa = 3.52, Credits = 20, Rank = "Good" },
                new GpaHistoryDto { Semester = "Spring 2024", Gpa = 3.58, Credits = 16, Rank = "Good" },
                new GpaHistoryDto { Semester = "Fall 2024", Gpa = 3.65, Credits = 15, Rank = "Good" }
            };

            return Ok(history);
        }

        // GET: api/student/grades/courses
        [HttpGet("grades/courses")]
        public async Task<IActionResult> GetCourseGrades([FromQuery] string? semester)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id)
                .Include(e => e.Course)
                .ToListAsync();

            var grades = new List<CourseGradeDto>();
            foreach (var enrollment in enrollments)
            {
                var results = await _context.LearningResults
                    .Where(r => r.EnrollmentId == enrollment.Id)
                    .ToListAsync();

                var finalScore = results.Any(r => r.ExamType == "Final") 
                    ? results.First(r => r.ExamType == "Final").Score ?? 0
                    : results.Sum(r => (r.Score ?? 0) * (r.Weight ?? 0.25));

                var letterGrade = CalculateLetterGrade(finalScore);
                var gradePoint = CalculateGradePoint(letterGrade);

                grades.Add(new CourseGradeDto
                {
                    CourseId = enrollment.Course!.Id,
                    Code = enrollment.Course.Code,
                    CourseName = enrollment.Course.Name,
                    Credits = enrollment.Course.Credits,
                    FinalScore = finalScore,
                    LetterGrade = letterGrade,
                    GradePoint = gradePoint,
                    Status = enrollment.Status,
                    Rank = GetRank(gradePoint)
                });
            }

            return Ok(grades);
        }

        // GET: api/student/grades/details
        [HttpGet("grades/details")]
        public async Task<IActionResult> GetGradeDetails([FromQuery] int courseId)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

            if (enrollment == null)
                return Ok(new List<GradeDetailDto>());

            var results = await _context.LearningResults
                .Where(r => r.EnrollmentId == enrollment.Id)
                .ToListAsync();

            var details = results.Select(r => new GradeDetailDto
            {
                Id = r.Id,
                ExamType = r.ExamType,
                Score = r.Score ?? 0,
                MaxScore = r.MaxScore ?? 10,
                Weight = r.Weight ?? 0.25,
                Grade = r.Grade
            }).ToList();

            return Ok(details);
        }

        // GET: api/student/courses/available
        [HttpGet("courses/available")]
        public async Task<IActionResult> GetAvailableCourses([FromQuery] string? search, [FromQuery] string? faculty, [FromQuery] int? credits)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Code.Contains(search));

            if (!string.IsNullOrEmpty(faculty))
                query = query.Where(c => c.Faculty == faculty);

            if (credits.HasValue)
                query = query.Where(c => c.Credits == credits.Value);

            var courses = await query
                .Include(c => c.Lecturer)
                .Select(c => new
                {
                    c.Id,
                    c.Code,
                    c.Name,
                    c.Credits,
                    c.Faculty,
                    c.Semester,
                    Lecturer = c.Lecturer != null ? c.Lecturer.FullName : "Chưa cập nhật",
                    c.Schedule,
                    c.Room,
                    Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && e.Status == "Active"),
                    c.MaxStudents
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/student/courses/registered
        [HttpGet("courses/registered")]
        public async Task<IActionResult> GetRegisteredCourses()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.Status == "Active")
                .Include(e => e.Course)
                .Select(e => new
                {
                    e.Id,
                    e.Course!.Code,
                    e.Course.Name,
                    e.Course.Credits,
                    e.Course.Schedule,
                    e.EnrollmentDate
                })
                .ToListAsync();

            return Ok(enrollments);
        }

        // POST: api/student/courses/register
        [HttpPost("courses/register")]
        public async Task<IActionResult> RegisterCourse([FromBody] RegisterCourseRequest request)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var existing = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == request.CourseId && e.Status == "Active");

            if (existing != null)
                return BadRequest(new { message = "Already registered for this course" });

            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null)
                return NotFound(new { message = "Course not found" });

            var enrollment = new Enrollment
            {
                StudentId = student.Id,
                CourseId = request.CourseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered successfully" });
        }

        // DELETE: api/student/courses/register/{id}
        [HttpDelete("courses/register/{id}")]
        public async Task<IActionResult> UnregisterCourse(int id)
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == id && e.StudentId == student.Id);

            if (enrollment == null)
                return NotFound(new { message = "Enrollment not found" });

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Unregistered successfully" });
        }

        // GET: api/student/timetable
        [HttpGet("timetable")]
        public async Task<IActionResult> GetTimetable()
        {
            var student = await GetStudentByEmail();
            if (student == null) return NotFound(new { message = "Student not found" });

            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id && e.Status == "Active")
                .Include(e => e.Course)
                .ToListAsync();

            var timetable = enrollments.Select(e => new
            {
                e.Id,
                e.Course!.Name,
                Day = e.Course.Schedule?.Split(',')[0] ?? "Chưa cập nhật",
                Time = e.Course.Schedule ?? "Chưa cập nhật",
                e.Course.Room,
                Lecturer = e.Course.Lecturer != null ? e.Course.Lecturer.FullName : "Chưa cập nhật"
            }).ToList();

            return Ok(timetable);
        }

        #region Private Methods

        private double CalculateGPA(List<LearningResult> results, List<Enrollment> enrollments)
        {
            if (!results.Any()) return 0;
            var totalPoints = results.Sum(r => (r.Score ?? 0) * (r.Weight ?? 0.25));
            return Math.Round(totalPoints, 2);
        }

        private string CalculateLetterGrade(double score)
        {
            if (score >= 8.5) return "A";
            if (score >= 7.0) return "B";
            if (score >= 5.5) return "C";
            if (score >= 4.0) return "D";
            return "F";
        }

        private double CalculateGradePoint(string letterGrade)
        {
            return letterGrade switch
            {
                "A" => 4.0,
                "B+" => 3.5,
                "B" => 3.0,
                "C+" => 2.5,
                "C" => 2.0,
                "D+" => 1.5,
                "D" => 1.0,
                _ => 0.0
            };
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

    public class RegisterCourseRequest
    {
        public int CourseId { get; set; }
    }
}
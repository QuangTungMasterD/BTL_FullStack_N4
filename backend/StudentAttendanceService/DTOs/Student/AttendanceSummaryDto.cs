namespace StudentAttendanceService.DTOs.Student
{
    public class AttendanceSummaryDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseCode { get; set; }  // THÊM property này
        public int TotalSessions { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = "#3b82f6";
    }
}
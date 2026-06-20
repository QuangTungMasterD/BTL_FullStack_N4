namespace StudentAttendanceService.DTOs.Admin
{
    public class CourseStatDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Enrolled { get; set; }
        public int MaxStudents { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageGrade { get; set; }
    }
}
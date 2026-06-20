namespace StudentAttendanceService.DTOs.Student
{
    public class CurrentCourseDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int Attendance { get; set; }
        public int Credits { get; set; }
        public string Lecturer { get; set; } = string.Empty;
    }
}
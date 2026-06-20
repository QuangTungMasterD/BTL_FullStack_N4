namespace StudentAttendanceService.DTOs.Lecturer
{
    public class LecturerCourseDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int StudentCount { get; set; }
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
    }
}
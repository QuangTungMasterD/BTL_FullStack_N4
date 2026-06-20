namespace StudentAttendanceService.DTOs.Admin
{
    public class UpdateCourseAdminDto
    {
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Faculty { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public int LecturerId { get; set; }
        public string? Schedule { get; set; }
        public string? Room { get; set; }
        public int MaxStudents { get; set; } = 60;
    }
}
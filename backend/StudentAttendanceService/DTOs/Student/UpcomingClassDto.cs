namespace StudentAttendanceService.DTOs.Student
{
    public class UpcomingClassDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Day { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string Lecturer { get; set; } = string.Empty;
    }
}
namespace StudentAttendanceService.DTOs.Lecturer
{
    public class TodayScheduleDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int StudentCount { get; set; }
    }
}
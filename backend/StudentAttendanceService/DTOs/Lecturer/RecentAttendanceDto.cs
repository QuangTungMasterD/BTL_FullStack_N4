namespace StudentAttendanceService.DTOs.Lecturer
{
    public class RecentAttendanceDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public int PresentCount { get; set; }
        public int TotalCount { get; set; }
        public double Rate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
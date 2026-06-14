namespace StudentAttendanceService.DTOs.Lecturer
{
    public class TopStudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public double AverageScore { get; set; }
    }
}
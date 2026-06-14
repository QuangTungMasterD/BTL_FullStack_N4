namespace StudentAttendanceService.DTOs.Student
{
    public class RecentGradeDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public double Score { get; set; }
        public string? LetterGrade { get; set; }
        public DateTime RecordedDate { get; set; }
    }
}
namespace StudentAttendanceService.DTOs.Lecturer
{
    public class StudentGradeDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public double Score { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
namespace StudentAttendanceService.DTOs.Student
{
    public class CourseGradeDto
    {
        public int CourseId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public double FinalScore { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public double GradePoint { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
    }
}
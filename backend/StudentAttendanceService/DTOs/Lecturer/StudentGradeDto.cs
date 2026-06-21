namespace StudentAttendanceService.DTOs.Lecturer
{
    public class StudentGradeDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Class { get; set; }
        
        // Điểm theo từng loại
        public double Score { get; set; }
        public double MidtermScore { get; set; }
        public double FinalScore { get; set; }
        public double ProjectScore { get; set; }
        public double Quiz1Score { get; set; }
        public double Quiz2Score { get; set; }
        
        // Điểm chữ theo từng loại
        public string? LetterGrade { get; set; }
        public string? MidtermGrade { get; set; }
        public string? FinalGrade { get; set; }
        public string? ProjectGrade { get; set; }
        public string? Quiz1Grade { get; set; }
        public string? Quiz2Grade { get; set; }
        
        // GPA và xếp loại
        public double GPA { get; set; }
        public string? Rank { get; set; }
        public string? Note { get; set; }
        public string? ExamType { get; set; }
    }
}
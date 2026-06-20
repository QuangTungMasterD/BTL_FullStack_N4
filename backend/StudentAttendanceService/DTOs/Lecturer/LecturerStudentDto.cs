namespace StudentAttendanceService.DTOs.Lecturer
{
    public class LecturerStudentDto
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Class { get; set; }
        public double Attendance { get; set; }
        public double AverageGrade { get; set; }
        public string Rank { get; set; } = string.Empty;
    }
}
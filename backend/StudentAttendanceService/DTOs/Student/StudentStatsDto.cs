namespace StudentAttendanceService.DTOs.Student
{
    public class StudentStatsDto
    {
        public double Gpa { get; set; }
        public double GpaTrend { get; set; }
        public int CreditsEarned { get; set; }
        public int RemainingCredits { get; set; }
        public int Attendance { get; set; }
        public int CurrentCourses { get; set; }
        public string? Rank { get; set; }
    }
}
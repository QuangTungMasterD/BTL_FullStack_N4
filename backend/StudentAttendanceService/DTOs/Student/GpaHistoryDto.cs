namespace StudentAttendanceService.DTOs.Student
{
    public class GpaHistoryDto
    {
        public string Semester { get; set; } = string.Empty;
        public double Gpa { get; set; }
        public int Credits { get; set; }
        public string Rank { get; set; } = string.Empty;
    }
}
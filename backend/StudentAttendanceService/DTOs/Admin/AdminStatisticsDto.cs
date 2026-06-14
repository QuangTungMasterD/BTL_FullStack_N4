namespace StudentAttendanceService.DTOs.Admin
{
    public class AdminStatisticsDto
    {
        public int TotalStudents { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalDepartments { get; set; }
        public int ActiveEnrollments { get; set; }
        public double AverageAttendance { get; set; }
        public double CompletionRate { get; set; }
        public double StudentGrowth { get; set; }
    }
}
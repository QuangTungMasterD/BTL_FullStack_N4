namespace StudentAttendanceService.DTOs.Student
{
    public class GradeDetailDto
    {
        public int Id { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public double Score { get; set; }
        public double MaxScore { get; set; }
        public double Weight { get; set; }
        public double WeightedScore => Score * Weight;
        public string? Grade { get; set; }
    }
}
namespace StudentAttendanceService.DTOs.Lecturer
{
    public class AttendanceHistoryDto
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Session { get; set; } = string.Empty;
        public int Total { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public double Rate { get; set; }
    }
}
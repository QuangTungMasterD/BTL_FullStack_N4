namespace StudentAttendanceService.DTOs.Admin
{
    public class DepartmentStatDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percent { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
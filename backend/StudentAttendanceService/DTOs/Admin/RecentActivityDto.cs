namespace StudentAttendanceService.DTOs.Admin
{
    public class RecentActivityDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? UserName { get; set; }
    }
}
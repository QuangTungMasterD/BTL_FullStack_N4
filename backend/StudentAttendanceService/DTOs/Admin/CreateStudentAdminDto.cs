namespace StudentAttendanceService.DTOs.Admin
{
    public class CreateStudentAdminDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Major { get; set; }
        public string? Class { get; set; }
        public string Status { get; set; } = "Active";
    }
}
namespace StudentAttendanceService.DTOs.Admin
{
    public class CreateLecturerAdminDto
    {
        public string LecturerId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Title { get; set; }
        public string? Specialization { get; set; }
        public string Status { get; set; } = "Active";
    }
}
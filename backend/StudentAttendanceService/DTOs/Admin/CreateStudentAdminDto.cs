namespace StudentAttendanceService.DTOs.Admin
{
    public class CreateStudentAdminDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Faculty { get; set; }  // Khóa học
        public string? Major { get; set; }    // Chuyên ngành
        public string? Class { get; set; }    // Lớp
        public string Status { get; set; } = "Active";
    }
}
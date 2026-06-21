namespace StudentAttendanceService.DTOs.Admin
{
    public class UpdateSpecializationDto
    {
        public string SpecializationName { get; set; } = string.Empty;
        public string? Descrt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
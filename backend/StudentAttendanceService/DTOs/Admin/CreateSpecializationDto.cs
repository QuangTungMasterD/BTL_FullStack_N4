namespace StudentAttendanceService.DTOs.Admin
{
    public class CreateSpecializationDto
    {
        public string SpecializationName { get; set; } = string.Empty;
        public string? Descrt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
namespace AuthService.DTOs
{
    public class UserInfoDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        // Student specific
        public string? StudentId { get; set; }
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Major { get; set; }
        public string? Class { get; set; }
        
        // Lecturer specific
        public string? LecturerId { get; set; }
        public string? Title { get; set; }
    }
}
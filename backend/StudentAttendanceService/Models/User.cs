using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "STUDENT";
        
        public string? StudentId { get; set; }
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Major { get; set; }
        public string? Class { get; set; }
        public string? LecturerId { get; set; }
        public string? Title { get; set; }
        public string? Specialization { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
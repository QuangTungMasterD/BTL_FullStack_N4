using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty; // Demo only: store hashed password in real apps
        
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "STUDENT"; // ADMIN, LECTURER, STUDENT
        
        // Additional fields for student
        public string? StudentId { get; set; }
        public string? Phone { get; set; }
        public string? Faculty { get; set; }
        public string? Major { get; set; }
        public string? Class { get; set; }
        
        // Additional fields for lecturer
        public string? LecturerId { get; set; }
        public string? Title { get; set; } // Giáo sư, Tiến sĩ, Thạc sĩ
        public string? Specialization { get; set; }
        
        // Common
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
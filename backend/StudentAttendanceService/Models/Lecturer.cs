using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceService.Models
{
    public class Lecturer
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LecturerId { get; set; } = string.Empty; // GV001
        
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(100)]
        public string? Faculty { get; set; }
        
        [MaxLength(50)]
        public string? Title { get; set; } // Giáo sư, Tiến sĩ, Thạc sĩ
        
        [MaxLength(200)]
        public string? Specialization { get; set; }
        
        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
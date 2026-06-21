// Models/Specialization.cs
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceService.Models
{
    public class Specialization
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string SpecializationName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Descrt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<Course>? Courses { get; set; }
    }
}
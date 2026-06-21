using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendanceService.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty; // CS101
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int Credits { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Faculty { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Semester { get; set; } = string.Empty; // Fall 2024
        
        [Required]
        public int LecturerId { get; set; }
        
        // THÊM: Khóa ngoại đến Specialization
        public int? SpecializationId { get; set; }
        
        [MaxLength(200)]
        public string? Schedule { get; set; } // Thứ 2, 13:30-16:30
        
        [MaxLength(50)]
        public string? Room { get; set; }
        
        public int MaxStudents { get; set; } = 60;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("LecturerId")]
        public virtual Lecturer? Lecturer { get; set; }
        
        // THÊM: Navigation property cho Specialization
        [ForeignKey("SpecializationId")]
        public virtual Specialization? Specialization { get; set; }
        
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
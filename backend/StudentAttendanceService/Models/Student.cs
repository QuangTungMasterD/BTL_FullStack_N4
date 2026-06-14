using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendanceService.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string StudentId { get; set; } = string.Empty; // Mã sinh viên: 2021001234
        
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
        public string? Faculty { get; set; } // Khoa
        
        [MaxLength(100)]
        public string? Major { get; set; } // Chuyên ngành
        
        [MaxLength(50)]
        public string? Class { get; set; } // Lớp
        
        [MaxLength(200)]
        public string? Address { get; set; }
        
        public int? Year { get; set; } // Năm học
        
        [Required]
        public string Status { get; set; } = "Active"; // Active, Inactive, Graduated
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
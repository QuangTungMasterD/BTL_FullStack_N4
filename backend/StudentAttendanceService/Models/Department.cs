using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceService.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty; // CNTT
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public int StudentCount { get; set; }
        public int LecturerCount { get; set; }
        
        [MaxLength(200)]
        public string? Head { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceService.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [MaxLength(100)]
        public string? Author { get; set; }
        
        [MaxLength(20)]
        public string Priority { get; set; } = "medium"; // high, medium, low
        
        public string? TargetRole { get; set; } // ADMIN, LECTURER, STUDENT, ALL
    }
}
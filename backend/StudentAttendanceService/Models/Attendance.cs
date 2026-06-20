using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendanceService.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [Required]
        public string Status { get; set; } = "Present"; // Present, Absent, Late
        
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        public int Week { get; set; }
        
        public string? Note { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        
        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}
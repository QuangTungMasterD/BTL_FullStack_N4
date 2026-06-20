using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendanceService.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; } // Thay đổi từ CourseScheduleId sang CourseId

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Active"; // Active, Completed, Dropped

        public DateTime? CompletedDate { get; set; }
        
        public double? FinalGrade { get; set; }
        public string? LetterGrade { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
        
        public virtual ICollection<LearningResult> LearningResults { get; set; } = new List<LearningResult>();
    }
}
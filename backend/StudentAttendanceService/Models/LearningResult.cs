using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAttendanceService.Models
{
    public class LearningResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EnrollmentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ExamType { get; set; } = "Midterm";

        public double? Score { get; set; }
        public double? MaxScore { get; set; } = 10;
        public double? Weight { get; set; } = 0.25;

        [MaxLength(2)]
        public string? Grade { get; set; }

        public string? Comment { get; set; }

        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("EnrollmentId")]
        public virtual Enrollment? Enrollment { get; set; }
    }
}
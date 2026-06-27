using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("course_teachers")]
    public class CourseTeacher : BaseModel
    {
        [Column("course_id")]
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Entities
{
    [Table("classes")]
    public class Class : BaseModel
    {
        [Column("classname")]
        public required String ClassName { get; set; }

        [Column("max_student")]
        public int MaxStudent { get; set; }

        [Column("start_date")]
        public DateOnly StartDate { get; set; }

        [Column("end_date")]
        public DateOnly EndDate { get; set; }

        [Column("status")]
        public ClassStatus Status { get; set; }

        [Column("lesson")]
        public int Lesson { get; set; }

        [Column("course_id")]
        public int CourseId { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } 
            = new List<TeacherAssignment>();

        public virtual ICollection<ClassSession> ClassSessions { get; set; } 
            = new List<ClassSession>();
    }
}
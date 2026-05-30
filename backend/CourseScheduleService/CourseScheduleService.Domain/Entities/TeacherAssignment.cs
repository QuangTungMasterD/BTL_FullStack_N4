using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("teacher_assignments")]
    public class TeacherAssignment : BaseModel
    {

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Column("class_id")]
        [ForeignKey("Class")]
        public int ClassId { get; set; }

        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;

        public virtual ICollection<ClassSession> ClassSessions { get; set; } 
            = new List<ClassSession>();
    }
}
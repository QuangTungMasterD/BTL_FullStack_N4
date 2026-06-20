using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("teacher_specializations")]
    public class TeacherSpecialization : BaseModel
    {

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        [Column("specialization_id")]
        [ForeignKey("Specialization")]
        public int SpecializationId { get; set; }

        public virtual Teacher Teacher { get; set; } = null!;

        public virtual Specialization Specialization { get; set; } = null!;
    }
}
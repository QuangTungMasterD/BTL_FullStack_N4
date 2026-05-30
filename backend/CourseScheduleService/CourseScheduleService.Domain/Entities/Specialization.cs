using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("specializations")]
    public class Specialization : BaseModel
    {
        [Column("specialization_name")]
        public required String SpecializationName { get; set; }

        [Column("descrt")]
        public String Descrt { get; set; } = String.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

        public virtual ICollection<TeacherSpecialization> TeacherSpecializations { get; set; }
            = new List<TeacherSpecialization>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Entities
{
    [Table("courses")]
    public class Course : BaseModel
    {
        [Column("course_name")]
        public required String CourseName { get; set; }

        [Column("desct")]
        public String? Desct { get; set; }

        [Column("tuition_fee")]
        public Decimal TuitionFee { get; set; }

        [Column("level")]
        public CourseLevel Level { get; set; }

        [Column("lesson")]
        public int Lesson { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("is_active", TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<CourseTeacher> CourseTeachers { get; set; } = new List<CourseTeacher>();
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
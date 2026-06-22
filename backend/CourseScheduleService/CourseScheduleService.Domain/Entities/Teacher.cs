using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("teachers")]
    public class Teacher : BaseModel
    {
        [Column("user_id")]
        public int? UserId { get; set; }
        
        [Column("fullname")]
        public required String FullName { get; set; }

        [Column("email")]
        public required String Email { get; set; }

        [Column("phone")]
        public required String Phone { get; set; }

        [Column("yob")]
        public DateOnly YoB { get; set; }

        [Column("gender")]
        public bool Gender { get; set; }

        [Column("is_active", TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<TeacherSpecialization> TeacherSpecializations { get; set; } 
            = new List<TeacherSpecialization>();

        public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } 
            = new List<TeacherAssignment>();

        public virtual ICollection<ClassSession> ClassSessions { get; set; } 
            = new List<ClassSession>();
    }
}
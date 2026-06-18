using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Entities
{
    [Table("class_sessions")]
    public class ClassSession : BaseModel
    {
        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("lesson")]
        public int Lesson { get; set; }

        [Column("status")]
        public ClassSessionStatus Status { get; set; } = ClassSessionStatus.Scheduled;

        [Column("room_id")]
        [ForeignKey("Room")]
        public int RoomId { get; set; }

        [Column("teacher_assignment_id")]
        [ForeignKey("TeacherAssignment")]
        public int TeacherAssignmentId { get; set; }

        public virtual Room Room { get; set; } = null!;

        public virtual TeacherAssignment TeacherAssignment { get; set; } = null!;
    }
}
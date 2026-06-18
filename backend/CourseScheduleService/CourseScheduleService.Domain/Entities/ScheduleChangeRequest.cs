using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Domain.Entities
{
    [Table("schedule_change_requests")]
    public class ScheduleChangeRequest : BaseModel
    {
        [Column("class_session_id")]
        public int ClassSessionId { get; set; }

        [Column("teacher_id")]
        public int TeacherId { get; set; }

        [Column("request_type")]
        public string RequestType { get; set; } = "Change"; // Change, Cancel

        [Column("reason")]
        public string Reason { get; set; } = string.Empty;

        [Column("preferred_session")]
        public string? PreferredSession { get; set; }

        [Column("suggested_start_time")]
        public DateTime? SuggestedStartTime { get; set; }

        [Column("suggested_end_time")]
        public DateTime? SuggestedEndTime { get; set; }

        [Column("suggested_room_id")]
        public int? SuggestedRoomId { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Column("admin_note")]
        public string? AdminNote { get; set; }

        [Column("processed_at")]
        public DateTime? ProcessedAt { get; set; }

        [Column("processed_by")]
        public int? ProcessedBy { get; set; }

        [Column("suggested_date")]
        public DateOnly? SuggestedDate { get; set; }

        public virtual ClassSession ClassSession { get; set; } = null!;
        public virtual Teacher Teacher { get; set; } = null!;
        public virtual Room? SuggestedRoom { get; set; }
    }
}
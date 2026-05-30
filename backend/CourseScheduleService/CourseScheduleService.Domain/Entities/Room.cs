using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Entities
{
    [Table("rooms")]
    public class Room : BaseModel
    {
        [Column("room_name")]
        public required String RoomName { get; set; }

        [Column("room_type")]
        public RoomType RoomType { get; set; }

        [Column("descrt")]
        public String Descrt { get; set; } = String.Empty;

        [Column("status")]
        public RoomStatus Status { get; set; }

        public virtual ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.RoomDtos
{
    public class RoomResDto
    {
        public int Id { get; set; }
        public required String RoomName { get; set; }
        public RoomType RoomType { get; set; }
        public String Descrt { get; set; } = String.Empty;
        public RoomStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
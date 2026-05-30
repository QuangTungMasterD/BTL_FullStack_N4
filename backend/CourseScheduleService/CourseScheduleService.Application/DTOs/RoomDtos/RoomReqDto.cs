using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.RoomDtos
{
    public class RoomReqDto
    {
        public required String RoomName { get; set; }
        public RoomType RoomType { get; set; }
        public String Descrt { get; set; } = String.Empty;
        public RoomStatus Status { get; set; }
    }
}
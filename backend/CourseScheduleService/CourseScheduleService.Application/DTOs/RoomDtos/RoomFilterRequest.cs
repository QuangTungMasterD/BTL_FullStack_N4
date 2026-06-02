using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.RoomDtos
{
    public class RoomFilterRequest : PagedRequest
    {
        public RoomType? RoomType { get; set; }
        public RoomStatus? Status { get; set; }
    }
}
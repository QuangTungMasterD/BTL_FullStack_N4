using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassSessionDtos
{
    public class ClassSessionFilterRequest : PagedRequest
    {
        public int? RoomId { get; set; }
        public int? ClassId { get; set; }
        public int? TeacherId { get; set; }
        public ClassSessionStatus? Status { get; set; }
        public DateTime? StartTimeFrom { get; set; }
        public DateTime? StartTimeTo { get; set; }
    }
}
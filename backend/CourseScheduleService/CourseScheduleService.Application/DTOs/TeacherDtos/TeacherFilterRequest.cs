using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;

namespace CourseScheduleService.Application.DTOs.TeacherDtos
{
    public class TeacherFilterRequest : PagedRequest
    {
        public bool? IsActive { get; set; }
        public int? YoBFrom { get; set; }
        public int? YoBTo { get; set; }
    }
}
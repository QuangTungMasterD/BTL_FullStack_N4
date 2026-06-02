using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.CourseDtos
{
    public class CourseFilterRequest : PagedRequest
    {
        public int? SpecializationId { get; set; }
        public CourseLevel? Level { get; set; }
        public bool? IsActive { get; set; }
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.Events
{
    public class ClassOpenedEvent
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxStudent { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}
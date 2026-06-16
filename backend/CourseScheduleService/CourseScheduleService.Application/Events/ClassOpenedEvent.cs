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
        public int TotalLessons { get; set; }
        public List<ClassSessionInfo> Sessions { get; set; } = new();
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }

    public class ClassSessionInfo
    {
        public int SessionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int LessonNumber { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public int TeacherAssignmentId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }
}
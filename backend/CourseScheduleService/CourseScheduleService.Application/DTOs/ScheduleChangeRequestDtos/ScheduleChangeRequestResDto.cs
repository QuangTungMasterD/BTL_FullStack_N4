using System;

namespace CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos
{
    public class ScheduleChangeRequestResDto
    {
        public int Id { get; set; }
        public int ClassSessionId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? SuggestedStartTime { get; set; }
        public DateTime? SuggestedEndTime { get; set; }
        public int? SuggestedRoomId { get; set; }
        public string SuggestedRoomName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? AdminNote { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateOnly? SuggestedDate { get; set; }

        public string? PreferredSession { get; set; }

        // Thông tin buổi học gốc
        public DateTime OriginalStartTime { get; set; }
        public DateTime OriginalEndTime { get; set; }
        public int OriginalRoomId { get; set; }
        public string OriginalRoomName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
    }
}
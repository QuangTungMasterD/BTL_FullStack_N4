using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.CourseTeacherDtos
{
    public class CourseTeacherResDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int CourseId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
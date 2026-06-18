using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherAssignmentDtos
{
    public class TeacherAssignmentResDto
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
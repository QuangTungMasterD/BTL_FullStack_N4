using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherDtos
{
    public class TeacherResDto
    {
        public int Id { get; set; }
        public required String FullName { get; set; }
        public required String Email { get; set; }
        public required String Phone { get; set; }
        public DateOnly YoB { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
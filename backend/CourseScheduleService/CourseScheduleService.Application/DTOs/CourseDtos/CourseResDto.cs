using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.CourseDtos
{
    public class CourseResDto
    {
        public int Id { get; set; }
        public required String CourseName { get; set; }
        public required String Desct { get; set; }
        public Decimal TuitionFee { get; set; }
        public CourseLevel Level { get; set; }
        public int Lesson { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
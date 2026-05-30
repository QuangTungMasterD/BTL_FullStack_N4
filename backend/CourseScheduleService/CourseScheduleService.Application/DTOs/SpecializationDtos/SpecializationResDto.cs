using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.SpecializationDtos
{
    public class SpecializationResDto
    {
        public int Id { get; set; }
        public required String SpecializationName { get; set; }
        public String Descrt { get; set; } = String.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

    }
}
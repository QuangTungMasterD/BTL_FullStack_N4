using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherSpecializationDtos
{
    public class TeacherSpecializationReqDto
    {
        public int TeacherId { get; set; }
        public int SpecializationId { get; set; }
    }
}
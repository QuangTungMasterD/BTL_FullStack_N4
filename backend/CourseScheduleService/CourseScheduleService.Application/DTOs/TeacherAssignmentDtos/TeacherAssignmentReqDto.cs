using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherAssignmentDtos
{
    public class TeacherAssignmentReqDto
    {
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
    }
}
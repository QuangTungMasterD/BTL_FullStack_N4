using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassDtos
{
    public class ClassReqDto
    {
        public required String ClassName { get; set; }
        public int MaxStudent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ClassStatus Status { get; set; }
        public int Lesson { get; set; }
        public int CourseId { get; set; }
    }
}
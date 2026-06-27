using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.CourseTeacherDtos
{
    public class CourseTeacherReqDto
    {
        [Required(ErrorMessage = "Giáo viên không được để trống")]
        public int TeacherId { get; set; }
        
        [Required(ErrorMessage = "Khóa học không được để trống")]
        public int CourseId { get; set; }
    }
}
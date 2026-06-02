using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherAssignmentDtos
{
    public class TeacherAssignmentReqDto
    {
        [Required(ErrorMessage = "Giáo viên không được để trống")]
        public int TeacherId { get; set; }
        
        [Required(ErrorMessage = "Lớp học không được để trống")]
        public int ClassId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.TeacherSpecializationDtos
{
    public class TeacherSpecializationReqDto
    {
        [Required(ErrorMessage = "Giáo viên không được để trống")]
        public int TeacherId { get; set; }
        
        [Required(ErrorMessage = "Chuyên ngành không được để trống")]
        public int SpecializationId { get; set; }
    }
}
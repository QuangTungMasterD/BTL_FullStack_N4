using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.SpecializationDtos
{
    public class SpecializationReqDto
    {
        [Required(ErrorMessage = "Tên chuyên môn không được để  trống.")]
        public required String SpecializationName { get; set; }
        public String? Descrt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
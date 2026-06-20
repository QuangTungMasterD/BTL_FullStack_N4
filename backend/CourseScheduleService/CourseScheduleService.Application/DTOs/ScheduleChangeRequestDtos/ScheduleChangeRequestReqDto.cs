using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos
{
    public class ScheduleChangeRequestReqDto
    {
        [Required]
        public int ClassSessionId { get; set; }

        [Required]
        public string RequestType { get; set; } = "Change"; // Change, Cancel

        [Required]
        public string Reason { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui lòng chọn sáng hoặc chiều")]
        
        public string? PreferredSession { get; set; } // "Morning" hoặc "Afternoon"

        [Required(ErrorMessage = "Vui lòng chọn ngày")]
        public DateTime? SuggestedDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos
{
    public class AdminActionDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public string Action { get; set; } = "Approve"; // Approve, Reject

        public string? AdminNote { get; set; }
    }
}
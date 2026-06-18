using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassSessionDtos
{
    public class ClassSessionReqDto
    {
        [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "số tiết học không được để trống")]
        [Range(1, int.MaxValue, ErrorMessage = "số tiết học phải lớn hơn 0")]
        public int Lesson { get; set; }
        
        public ClassSessionStatus Status { get; set; } = ClassSessionStatus.Scheduled;

        [Required(ErrorMessage = "Phòng học không được để trống")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Phân công giáo viên không được để trống")]
        public int TeacherAssignmentId { get; set; }
    }
}
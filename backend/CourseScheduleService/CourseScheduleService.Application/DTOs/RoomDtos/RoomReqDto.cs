using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.RoomDtos
{
    public class RoomReqDto
    {
        [Required(ErrorMessage = "Tên phòng không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên phòng không được vượt quá 100 ký tự")]
        public required String RoomName { get; set; }

        [Required(ErrorMessage = "Loại phòng không được để trống")]
        public RoomType RoomType { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public String? Descrt { get; set; } = String.Empty;
        public RoomStatus Status { get; set; }
    }
}
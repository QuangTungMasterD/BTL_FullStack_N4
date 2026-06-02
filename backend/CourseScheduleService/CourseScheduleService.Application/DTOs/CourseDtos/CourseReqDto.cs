using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.CourseDtos
{
    public class CourseReqDto
    {
        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên khóa học không được vượt quá 100 ký tự")]
        [MinLength(3, ErrorMessage = "Tên khóa học phải có ít nhất 3 ký tự")]
        public required String CourseName { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public required String Desct { get; set; }

        [Required(ErrorMessage = "Học phí không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Học phí phải lớn hơn hoặc bằng 0")]
        public Decimal TuitionFee { get; set; }

        [Required(ErrorMessage = "Cấp độ không được để trống")]
        [Range(1, 5, ErrorMessage = "Cấp độ phải từ 1 đến 5")]
        public CourseLevel Level { get; set; }

        [Required(ErrorMessage = "Số buổi học không được để trống")]
        [Range(1, 200, ErrorMessage = "Số buổi học phải từ 1 đến 200")]
        public int Lesson { get; set; }
        public bool IsActive { get; set; } = true;

        public int? SpecializationId { get; set; }
    }
}
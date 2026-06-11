using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassDtos
{
  public class ClassReqDto
  {
    [Required(ErrorMessage = "Tên lớp không được để trống")]
    public required String ClassName { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số học viên tối đa phải lớn hơn 0")]
    public int MaxStudent { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
    public DateOnly EndDate { get; set; }
    public ClassStatus Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "số tiết học phải lớn hơn 0")]
    public int Lesson { get; set; }

    [Required(ErrorMessage = "Khóa học không được để trống")]
    public int CourseId { get; set; }
  }
}
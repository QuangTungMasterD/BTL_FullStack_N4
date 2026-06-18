using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassDtos
{
  public class ClassResDto
  {
    public int Id { get; set; }
    public required String ClassName { get; set; }
    public int MaxStudent { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ClassStatus Status { get; set; }
    public int Lesson { get; set; }
    public int CourseId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
  }
}
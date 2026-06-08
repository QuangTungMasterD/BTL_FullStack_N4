using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.DTOs.ClassDtos
{
  public class ClassFilterRequest : PagedRequest
  {
    public int? CourseId { get; set; }
    public ClassStatus? Status { get; set; }
    public DateOnly? StartDateFrom { get; set; }
    public DateOnly? StartDateTo { get; set; }
    public bool? IsDeleted {get; set; } = false;
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;

namespace CourseScheduleService.Application.DTOs.SpecializationDtos
{
  public class SpecializationFilterRequest : PagedRequest
  {
    public bool? IsActive { get; set; }
  }
}
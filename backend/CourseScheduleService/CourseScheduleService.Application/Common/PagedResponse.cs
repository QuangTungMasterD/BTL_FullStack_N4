using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.Common
{
  public class PagedResponse<T>
  {
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrev => Page > 1;
  }
}
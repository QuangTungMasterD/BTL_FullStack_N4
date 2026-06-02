using System.Collections.Generic;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
  public interface IClassRepository : IRepository<Class>
  {
    Task<Class?> IsClassNameExistsAsync(string className, int? excludeId = null);
    Task<IEnumerable<Class>> GetClassesByCourseAsync(int courseId);
    Task<(IEnumerable<Class> Data, int TotalRecords)> GetPagedClassesAsync(
        int page, int pageSize, string? search,
        int? courseId, ClassStatus? status,
        DateOnly? startDateFrom, DateOnly? startDateTo,
        string? sortBy, bool sortDesc
    );
  }
}
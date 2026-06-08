using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
  public interface ITeacherRepository : IRepository<Teacher>
  {
    Task<Teacher?> IsEmailExistsAsync(string email, int? excludeId = null);
    Task<Teacher?> IsPhoneExistsAsync(string phone, int? excludeId = null);
    Task<Teacher?> IsTeacherIdExistsAsync(int Id);

    Task<(IEnumerable<Teacher> Data, int TotalRecords)> GetPagedTeachersAsync(
        int page, int pageSize, string? search,
        bool? isActive, int? yoBFrom, int? yoBTo,
        string? sortBy, bool sortDesc
    );
  }
}
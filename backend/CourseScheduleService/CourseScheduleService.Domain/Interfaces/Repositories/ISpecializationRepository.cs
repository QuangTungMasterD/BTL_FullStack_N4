using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
  public interface ISpecializationRepository : IRepository<Specialization>
  {
    Task<Specialization?> IsSpecializationNameExistsAsync(string specializationName, int? Id = null);
    Task<(IEnumerable<Specialization> Data, int TotalRecords)> GetPagedSpecializationsAsync(
      int page, int pageSize, string? search,
      bool? isActive, string? sortBy, bool sortDesc
    );
  }
}
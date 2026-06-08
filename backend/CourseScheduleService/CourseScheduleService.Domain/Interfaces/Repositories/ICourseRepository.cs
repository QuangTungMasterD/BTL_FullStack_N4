using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
  public interface ICourseRepository : IRepository<Course>
  {
    Task<Course?> IsCourseNameExistsAsync(string courseName, int? Id = null);
    Task<Course?> IsCourseIdExistsAsync(int Id);
    Task<IEnumerable<Course>> GetCoursesBySpecializationAsync(int idSpecialization);
    Task<(IEnumerable<Course> Data, int TotalRecords)> GetPagedCoursesAsync(
        int page,
        int pageSize,
        string? search,
        int? specializationId,
        CourseLevel? level,
        bool? isActive,
        decimal? minFee,
        decimal? maxFee,
        string? sortBy,
        bool sortDesc
    );
  }
}
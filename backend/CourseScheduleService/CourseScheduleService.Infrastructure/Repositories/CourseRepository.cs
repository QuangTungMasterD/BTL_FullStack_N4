using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace CourseScheduleService.Infrastructure.Repositories
{
  public class CourseRepository : Repository<Course>, ICourseRepository
  {
    public CourseRepository(CourseScheduleDbContext context) : base(context)
    {
    }

    public async Task<Course?> IsCourseNameExistsAsync(string courseName, int? Id = null)
    {
      var query = _dbSet.Where(c => c.CourseName == courseName);
      if (Id.HasValue)
        query = query.Where(c => c.Id != Id.Value);
      return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Course>> GetCoursesBySpecializationAsync(int idSpecialization)
    {
      var query = _dbSet.Where(c => c.SpecializationId == idSpecialization);

      return await query.ToArrayAsync();
    }

    public async Task<(IEnumerable<Course> Data, int TotalRecords)> GetPagedCoursesAsync(
        int page, 
        int pageSize, 
        string? search, 
        int? specializationId,
        CourseLevel? level, 
        bool? isActive, 
        decimal? minFee, 
        decimal? maxFee,
        string? sortBy, 
        bool sortDesc) {
      var query = _dbSet.Where(c => !c.IsDeleted);

      if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(c => c.CourseName.Contains(search));

      if (specializationId.HasValue)
        query = query.Where(c => c.SpecializationId == specializationId);

      if (level.HasValue)
        query = query.Where(c => c.Level == level);

      if (isActive.HasValue)
        query = query.Where(c => c.IsActive == isActive);

      if (minFee.HasValue)
        query = query.Where(c => c.TuitionFee >= minFee);

      if (maxFee.HasValue)
        query = query.Where(c => c.TuitionFee <= maxFee);

      var totalRecords = await query.CountAsync();

      query = sortBy?.ToLower() switch
      {
        "coursename" => sortDesc ? query.OrderByDescending(c => c.CourseName)
                                 : query.OrderBy(c => c.CourseName),
        "tuitionfee" => sortDesc ? query.OrderByDescending(c => c.TuitionFee)
                                 : query.OrderBy(c => c.TuitionFee),
        "createdat" => sortDesc ? query.OrderByDescending(c => c.CreatedAt)
                                 : query.OrderBy(c => c.CreatedAt),
        _ => query.OrderByDescending(c => c.CreatedAt)
      };

      var data = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToArrayAsync();

      return (data, totalRecords);
    }
  }
}
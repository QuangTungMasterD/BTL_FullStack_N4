using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;
using CourseScheduleService.Domain.Entities;
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
  }
}
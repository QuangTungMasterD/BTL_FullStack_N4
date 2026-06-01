using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
  public class SpecializationRepository : Repository<Specialization>, ISpecializationRepository
  {
    public SpecializationRepository(CourseScheduleDbContext context) : base(context)
    {
    }

    public async Task<Specialization?> IsSpecializationNameExistsAsync(string specializationName, int? Id = null)
    {
      var query = _dbSet.Where(s => s.SpecializationName == specializationName);
        if (Id.HasValue)
            query = query.Where(s => s.Id != Id.Value);
        return await query.FirstOrDefaultAsync();
    }
  }
}
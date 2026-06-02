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

    public async Task<(IEnumerable<Specialization> Data, int TotalRecords)> GetPagedSpecializationsAsync(
        int page, int pageSize, string? search,
        bool? isActive, string? sortBy, bool sortDesc)
    {
      var query = _dbSet.Where(s => !s.IsDeleted);

      if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(s => s.SpecializationName.Contains(search));

      if (isActive.HasValue)
        query = query.Where(s => s.IsActive == isActive);

      var totalRecords = await query.CountAsync();

      query = sortBy?.ToLower() switch
      {
        "specializationname" => sortDesc ? query.OrderByDescending(s => s.SpecializationName)
                                         : query.OrderBy(s => s.SpecializationName),
        "createdat" => sortDesc ? query.OrderByDescending(s => s.CreatedAt)
                                         : query.OrderBy(s => s.CreatedAt),
        _ => query.OrderByDescending(s => s.CreatedAt)
      };

      var data = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToArrayAsync();

      return (data, totalRecords);
    }
  }
}
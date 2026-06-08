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
  public class TeacherRepository : Repository<Teacher>, ITeacherRepository
  {
    public TeacherRepository(CourseScheduleDbContext context) : base(context)
    {
    }

    public async Task<Teacher?> IsEmailExistsAsync(string email, int? excludeId = null)
    {
      return await _dbSet
            .Where(t => t.Email == email)
            .Where(t => excludeId == null || t.Id != excludeId)
            .FirstOrDefaultAsync();
    }

    public async Task<Teacher?> IsPhoneExistsAsync(string phone, int? excludeId = null)
    {
      return await _dbSet
            .Where(t => t.Phone == phone)
            .Where(t => excludeId == null || t.Id != excludeId)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Teacher> Data, int TotalRecords)> GetPagedTeachersAsync(
        int page, int pageSize, string? search,
        bool? isActive, int? yoBFrom, int? yoBTo,
        string? sortBy, bool sortDesc, bool? IsDeleted = false)
    {
      var query = _dbSet.Where(t => t.IsDeleted == IsDeleted);

      if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(t => t.FullName.Contains(search) || t.Email.Contains(search) || t.Phone.Contains(search));

      if (isActive.HasValue)
        query = query.Where(t => t.IsActive == isActive);

      if (yoBFrom.HasValue)
        query = query.Where(t => t.YoB.Year >= yoBFrom);

      if (yoBTo.HasValue)
        query = query.Where(t => t.YoB.Year <= yoBTo);

      var totalRecords = await query.CountAsync();

      query = sortBy?.ToLower() switch
      {
        "fullname" => sortDesc ? query.OrderByDescending(t => t.FullName)
                                : query.OrderBy(t => t.FullName),
        "yob" => sortDesc ? query.OrderByDescending(t => t.YoB)
                                : query.OrderBy(t => t.YoB),
        "createdat" => sortDesc ? query.OrderByDescending(t => t.CreatedAt)
                                : query.OrderBy(t => t.CreatedAt),
        _ => query.OrderByDescending(t => t.CreatedAt)
      };

      var data = await query
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToArrayAsync();

      return (data, totalRecords);
    }

    public async Task<Teacher?> IsTeacherIdExistsAsync(int Id)
    {
      return await _dbSet.FirstOrDefaultAsync(x =>
            EF.Property<int>(x, "Id") == Id);
    }
  }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
  public class ClassRepository : Repository<Class>, IClassRepository
  {
    public ClassRepository(CourseScheduleDbContext context) : base(context) { }

    public async Task<Class?> IsClassNameExistsAsync(string className, int? excludeId = null)
    {
      var query = _dbSet.Where(c => c.ClassName == className);
      if (excludeId.HasValue)
        query = query.Where(c => c.Id != excludeId.Value);
      return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Class>> GetClassesByCourseAsync(int courseId)
    {
      return await _dbSet
          .Where(c => c.CourseId == courseId && !c.IsDeleted)
          .ToArrayAsync();
    }

    // ClassRepository.cs
    public async Task<(IEnumerable<Class> Data, int TotalRecords)> GetPagedClassesAsync(
        int page, int pageSize, string? search,
        int? courseId, ClassStatus? status,
        DateOnly? startDateFrom, DateOnly? startDateTo,
        string? sortBy, bool sortDesc, bool? IsDeleted = false)
    {
      var query = _dbSet.Where(c => c.IsDeleted == IsDeleted);

      if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(c => c.ClassName.Contains(search));

      if (courseId.HasValue)
        query = query.Where(c => c.CourseId == courseId);

      if (status.HasValue)
        query = query.Where(c => c.Status == status);

      if (startDateFrom.HasValue)
        query = query.Where(c => c.StartDate >= startDateFrom);

      if (startDateTo.HasValue)
        query = query.Where(c => c.StartDate <= startDateTo);

      var totalRecords = await query.CountAsync();

      query = sortBy?.ToLower() switch
      {
        "classname" => sortDesc ? query.OrderByDescending(c => c.ClassName)
                                 : query.OrderBy(c => c.ClassName),
        "startdate" => sortDesc ? query.OrderByDescending(c => c.StartDate)
                                 : query.OrderBy(c => c.StartDate),
        "maxstudent" => sortDesc ? query.OrderByDescending(c => c.MaxStudent)
                                 : query.OrderBy(c => c.MaxStudent),
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

    public async Task<Class?> IsClassIdExistsAsync(int Id)
    {
      return await _dbSet.FirstOrDefaultAsync(x =>
            EF.Property<int>(x, "Id") == Id);
    }

    public async Task<IEnumerable<Class>> GetClassesByTeacherIdAsync(int teacherId)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted)
            .Where(c => c.TeacherAssignments.Any(ta => ta.TeacherId == teacherId && !ta.IsDeleted))
            .Include(c => c.TeacherAssignments) // tuỳ chọn, nếu cần thông tin teacher
            .ThenInclude(ta => ta.Teacher)
            .ToListAsync();
    }
  }
}
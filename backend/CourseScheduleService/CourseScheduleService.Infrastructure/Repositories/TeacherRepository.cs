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
  }
}
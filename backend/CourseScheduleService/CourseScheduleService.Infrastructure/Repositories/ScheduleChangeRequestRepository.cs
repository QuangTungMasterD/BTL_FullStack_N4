using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CourseScheduleService.Infrastructure.Repositories
{
    public class ScheduleChangeRequestRepository : Repository<ScheduleChangeRequest>, IScheduleChangeRequestRepository
    {
        public ScheduleChangeRequestRepository(CourseScheduleDbContext context) : base(context) {}

        public async Task<List<ScheduleChangeRequest>> GetByTeacherIdAsync(int teacherId)
        {
            return await _dbSet
                .Where(r => r.TeacherId == teacherId && !r.IsDeleted)
                .Include(r => r.ClassSession)
                .ThenInclude(cs => cs.Room)
                .Include(r => r.SuggestedRoom)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ScheduleChangeRequest>> GetPendingRequestsAsync()
        {
            return await _dbSet
                .Where(r => r.Status == "Pending" && !r.IsDeleted)
                .Include(r => r.ClassSession)
                    .ThenInclude(cs => cs.Room)
                .Include(r => r.ClassSession)
                    .ThenInclude(cs => cs.TeacherAssignment)
                    .ThenInclude(ta => ta.Teacher)
                .Include(r => r.Teacher)
                .Include(r => r.SuggestedRoom)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<ScheduleChangeRequest?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.ClassSession)
                    .ThenInclude(cs => cs.Room)
                .Include(r => r.ClassSession)
                    .ThenInclude(cs => cs.TeacherAssignment)
                    .ThenInclude(ta => ta.Teacher)
                .Include(r => r.Teacher)
                .Include(r => r.SuggestedRoom)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }
    }
}
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
    public class TeacherSpecializationRepository : Repository<TeacherSpecialization>, ITeacherSpecializationRepository
    {
        public TeacherSpecializationRepository(CourseScheduleDbContext context) : base(context) { }

        public async Task<TeacherSpecialization?> GetByTeacherAndSpecializationAsync(int teacherId, int specializationId)
        {
            return await _dbSet.FirstOrDefaultAsync(ts =>
                ts.TeacherId == teacherId &&
                ts.SpecializationId == specializationId &&
                !ts.IsDeleted
            );
        }

        public async Task<IEnumerable<TeacherSpecialization>> GetByTeacherIdAsync(int teacherId)
        {
            var specializations = await _dbSet.Where(ta => ta.TeacherId == teacherId).ToListAsync();
            return specializations;
        }
    }
}
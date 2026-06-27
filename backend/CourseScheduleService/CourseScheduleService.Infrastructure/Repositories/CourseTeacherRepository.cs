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
    public class CourseTeacherRepository : Repository<CourseTeacher>, ICourseTeacherRepository
    {
        public CourseTeacherRepository(CourseScheduleDbContext context) : base(context) { }

        public async Task<CourseTeacher?> GetByTeacherAndCourseAsync(int teacherId, int courseId)
        {
            return await _dbSet.FirstOrDefaultAsync(ts =>
                ts.TeacherId == teacherId &&
                ts.CourseId == courseId &&
                !ts.IsDeleted
            );
        }

        public async Task<IEnumerable<CourseTeacher>> GetByTeacherIdAsync(int teacherId)
        {
            var courseTeachers = await _dbSet.Where(ta => ta.TeacherId == teacherId).ToListAsync();
            return courseTeachers;
        }

        public async Task<IEnumerable<CourseTeacher>> GetByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Include(ct => ct.Teacher)
                .ThenInclude(t => t.TeacherAssignments)
                .Where(ts =>
                    ts.CourseId == courseId &&
                    !ts.IsDeleted
                ).ToListAsync();
        }
    }
}
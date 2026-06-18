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
    public class TeacherAssignmentRepository : Repository<TeacherAssignment>, ITeacherAssignmentRepository
    {
        public TeacherAssignmentRepository(CourseScheduleDbContext context) : base(context) { }

        public async Task<TeacherAssignment?> GetByTeacherAndClassAsync(int teacherId, int classId)
        {
            return await _dbSet.FirstOrDefaultAsync(ta =>
                ta.TeacherId == teacherId &&
                ta.ClassId == classId &&
                !ta.IsDeleted
            );
        }
    }
}
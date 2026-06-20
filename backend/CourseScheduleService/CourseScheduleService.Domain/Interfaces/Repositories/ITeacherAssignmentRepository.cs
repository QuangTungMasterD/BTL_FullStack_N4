using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface ITeacherAssignmentRepository : IRepository<TeacherAssignment>
    {
        Task<TeacherAssignment?> GetByTeacherAndClassAsync(int teacherId, int classId);
    }
}
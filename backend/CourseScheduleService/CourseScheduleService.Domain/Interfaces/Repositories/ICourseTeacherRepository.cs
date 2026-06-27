using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface ICourseTeacherRepository : IRepository<CourseTeacher>
    {
        Task<CourseTeacher?> GetByTeacherAndCourseAsync(int teacherId, int courseId);
        Task<IEnumerable<CourseTeacher>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<CourseTeacher>> GetByCourseIdAsync(int courseId);
    }
}
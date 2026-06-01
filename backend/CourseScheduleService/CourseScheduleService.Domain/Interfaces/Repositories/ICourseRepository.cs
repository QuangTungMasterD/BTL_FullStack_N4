using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> IsCourseNameExistsAsync(string courseName, int? Id = null);
        Task<IEnumerable<Course>> GetCoursesBySpecializationAsync(int idSpecialization);
    }
}
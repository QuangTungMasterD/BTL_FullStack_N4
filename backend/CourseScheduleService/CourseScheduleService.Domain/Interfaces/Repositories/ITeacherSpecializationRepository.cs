using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface ITeacherSpecializationRepository : IRepository<TeacherSpecialization>
    {
        Task<TeacherSpecialization?> GetByTeacherAndSpecializationAsync(int teacherId, int specializationId);
    }
}
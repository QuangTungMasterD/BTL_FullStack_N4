using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface IScheduleChangeRequestRepository : IRepository<ScheduleChangeRequest>
    {
        Task<List<ScheduleChangeRequest>> GetByTeacherIdAsync(int teacherId);
        Task<List<ScheduleChangeRequest>> GetPendingRequestsAsync();
        Task<ScheduleChangeRequest?> GetWithDetailsAsync(int id);
    }
}
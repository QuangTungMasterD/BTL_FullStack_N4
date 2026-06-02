using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface IClassSessionRepository : IRepository<ClassSession>
    {
        Task<(IEnumerable<ClassSession> Data, int TotalRecords)> GetPagedSessionsAsync(
        int page, int pageSize, string? search,
        int? roomId, int? classId, int? teacherId,
        ClassSessionStatus? status,
        DateTime? startTimeFrom, DateTime? startTimeTo,
        string? sortBy, bool sortDesc
        );
        Task<bool> IsRoomConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeId = null);
        Task<bool> IsTeacherConflictAsync(int teacherId, DateTime startTime, DateTime endTime, int? excludeId = null);
    }
}
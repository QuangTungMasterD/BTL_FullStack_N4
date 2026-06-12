using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
    public class ClassSessionRepository : Repository<ClassSession>, IClassSessionRepository
    {
        public ClassSessionRepository(CourseScheduleDbContext context) : base(context) { }

        public async Task<(IEnumerable<ClassSession> Data, int TotalRecords)> GetPagedSessionsAsync(
            int page, int pageSize, string? search,
            int? roomId,
            int? classId, int? teacherId,
            ClassSessionStatus? status,
            DateTime? startTimeFrom, DateTime? startTimeTo,
            string? sortBy, bool sortDesc)
        {
            var query = _dbSet
                .Include(s => s.TeacherAssignment)
                .Where(s => !s.IsDeleted);

            if (roomId.HasValue)
                query = query.Where(s => s.RoomId == roomId);

            if (classId.HasValue)
                query = query.Where(s => s.TeacherAssignment.ClassId == classId);

            if (teacherId.HasValue)
                query = query.Where(s => s.TeacherAssignment.TeacherId == teacherId);

            if (status.HasValue)
                query = query.Where(s => s.Status == status);

            if (startTimeFrom.HasValue)
                query = query.Where(s => s.StartTime >= startTimeFrom);

            if (startTimeTo.HasValue)
                query = query.Where(s => s.StartTime <= startTimeTo);

            var totalRecords = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "starttime" => sortDesc ? query.OrderByDescending(s => s.StartTime)
                                        : query.OrderBy(s => s.StartTime),
                "endtime"   => sortDesc ? query.OrderByDescending(s => s.EndTime)
                                        : query.OrderBy(s => s.EndTime),
                "lesson"    => sortDesc ? query.OrderByDescending(s => s.Lesson)
                                        : query.OrderBy(s => s.Lesson),
                "createdat" => sortDesc ? query.OrderByDescending(s => s.CreatedAt)
                                        : query.OrderBy(s => s.CreatedAt),
                _           => query.OrderBy(s => s.StartTime)
            };

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return (data, totalRecords);
        }

        public async Task<bool> IsRoomConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeId = null)
        {
            var query = _dbSet.Where(s =>
                s.RoomId == roomId &&
                !s.IsDeleted &&
                s.StartTime < endTime &&
                s.EndTime > startTime
            );

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> IsTeacherConflictAsync(int teacherId, DateTime startTime, DateTime endTime, int? excludeId = null)
        {
            var query = _dbSet
                .Include(s => s.TeacherAssignment)
                .Where(s =>
                    s.TeacherAssignment.TeacherId == teacherId &&
                    !s.IsDeleted &&
                    s.StartTime < endTime &&
                    s.EndTime > startTime
                );

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }
        public async Task<TeacherAssignment?> GetTeacherAssignmentAsync(int sessionId)
        {
            var session = await _dbSet
                .Include(s => s.TeacherAssignment)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
            return session?.TeacherAssignment;
        }

        public async Task<Class?> GetClassBySessionIdAsync(int sessionId)
        {
            var session = await _dbSet
                .Include(s => s.TeacherAssignment)
                .ThenInclude(ta => ta.Class)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
            return session?.TeacherAssignment?.Class;
        }
    }
}
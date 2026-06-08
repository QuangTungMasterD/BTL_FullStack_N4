using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(CourseScheduleDbContext context) : base(context) { }

        public async Task<Room?> IsRoomNameExistsAsync(string roomName, int? excludeId = null)
        {
            var query = _dbSet.Where(r => r.RoomName == roomName && !r.IsDeleted);
            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Room> Data, int TotalRecords)> GetPagedRoomsAsync(
            int page, int pageSize, string? search,
            RoomType? roomType, RoomStatus? status,
            string? sortBy, bool sortDesc, bool? IsDeleted = false)
        {
            var query = _dbSet.Where(r => r.IsDeleted == IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r => r.RoomName.Contains(search));

            if (roomType.HasValue)
                query = query.Where(r => r.RoomType == roomType);

            if (status.HasValue)
                query = query.Where(r => r.Status == status);

            var totalRecords = await query.CountAsync();

            query = sortBy?.ToLower() switch
            {
                "roomname"  => sortDesc ? query.OrderByDescending(r => r.RoomName)
                                        : query.OrderBy(r => r.RoomName),
                "roomtype"  => sortDesc ? query.OrderByDescending(r => r.RoomType)
                                        : query.OrderBy(r => r.RoomType),
                "createdat" => sortDesc ? query.OrderByDescending(r => r.CreatedAt)
                                        : query.OrderBy(r => r.CreatedAt),
                _           => query.OrderByDescending(r => r.CreatedAt)
            };

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return (data, totalRecords);
        }

    public async Task<Room?> IsRoomIdExistsAsync(int Id)
    {
      return await _dbSet.FirstOrDefaultAsync(x =>
            EF.Property<int>(x, "Id") == Id);
    }
  }
}
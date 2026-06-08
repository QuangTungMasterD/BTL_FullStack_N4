using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Domain.Interfaces.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Task<Room?> IsRoomNameExistsAsync(string roomName, int? excludeId = null);
        Task<Room?> IsRoomIdExistsAsync(int Id);

        Task<(IEnumerable<Room> Data, int TotalRecords)> GetPagedRoomsAsync(
            int page, int pageSize, string? search,
            RoomType? roomType, RoomStatus? status,
            string? sortBy, bool sortDesc, bool? IsDeleted = false
        );
    }
}
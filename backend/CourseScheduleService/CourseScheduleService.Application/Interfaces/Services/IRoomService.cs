using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.RoomDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
  public interface IRoomService
  {
    Task<ApiResponse<RoomResDto?>> GetOneByIdAsync(int id);
    Task<ApiResponse<IEnumerable<RoomResDto>>> GetAllRoomAsync();
    Task<ApiResponse<RoomResDto?>> CreateRoomAsync(RoomReqDto roomReqDto);
    Task<ApiResponse<RoomResDto?>> UpdateRoomAsync(int id, RoomReqDto roomReqDto);
    Task<ApiResponse<bool>> DeleteRoomAsync(int id);
    Task<ApiResponse<bool>> HardDeleteRoomAsync(int id);
    Task<ApiResponse<RoomResDto?>> RestoreRoomAsync(int id);
    Task<ApiResponse<PagedResponse<RoomResDto>>> GetPagedRoomsAsync(RoomFilterRequest req);
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.RoomDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
  public class RoomService : IRoomService
  {
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IMapper mapper)
    {
      _roomRepository = roomRepository;
      _mapper = mapper;
    }

    public async Task<ApiResponse<RoomResDto?>> CreateRoomAsync(RoomReqDto roomReqDto)
    {
      var exists = await _roomRepository.IsRoomNameExistsAsync(roomReqDto.RoomName);
      if (exists != null)
      {
        return ApiResponse<RoomResDto?>.ErrorResponse(
            $"Tên phòng '{roomReqDto.RoomName}' đã tồn tại",
            new Dictionary<string, string[]> { { "RoomName", new[] { "Tên phòng đã tồn tại" } } },
            409
        );
      }

      var room = _mapper.Map<Room>(roomReqDto);
      await _roomRepository.AddAsync(room);
      await _roomRepository.SaveChangeAsync();

      return ApiResponse<RoomResDto?>.SuccessResponse(
          _mapper.Map<RoomResDto>(room), "Tạo phòng thành công", 201
      );
    }

    public async Task<ApiResponse<bool>> DeleteRoomAsync(int id)
    {
      var room = await _roomRepository.GetByIdAsync(id);
      if (room == null)
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy phòng {id}.", statusCode: 404);

      room.IsDeleted = true;
      _roomRepository.UpdateAsync(room);
      await _roomRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa phòng {id} thành công.");
    }

    public async Task<ApiResponse<bool>> HardDeleteRoomAsync(int id)
    {
      var room = await _roomRepository.IsRoomIdExistsAsync(id);
      if (room == null)
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy phòng {id}.", statusCode: 404);

      if (!room.IsDeleted)
        return ApiResponse<bool>.ErrorResponse(
            $"Phòng {id} chưa được xóa mềm, không thể xóa vĩnh viễn.", statusCode: 409
        );

      _roomRepository.DeleteAsync(room);
      await _roomRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa vĩnh viễn phòng {id} thành công.");
    }

    public async Task<ApiResponse<RoomResDto?>> RestoreRoomAsync(int id)
    {
      var room = await _roomRepository.IsRoomIdExistsAsync(id);
      if (room == null)
        return ApiResponse<RoomResDto?>.ErrorResponse($"Không tìm thấy phòng {id}.", statusCode: 404);

      if (!room.IsDeleted)
        return ApiResponse<RoomResDto?>.ErrorResponse(
            $"Phòng {id} chưa bị xóa, không cần khôi phục.", statusCode: 409
        );

      room.IsDeleted = false;
      _roomRepository.UpdateAsync(room);
      await _roomRepository.SaveChangeAsync();

      return ApiResponse<RoomResDto?>.SuccessResponse(
          _mapper.Map<RoomResDto>(room), $"Khôi phục phòng {id} thành công."
      );
    }

    public async Task<ApiResponse<IEnumerable<RoomResDto>>> GetAllRoomAsync()
    {
      var rooms = await _roomRepository.GetAllAsync();
      return ApiResponse<IEnumerable<RoomResDto>>.SuccessResponse(
          _mapper.Map<IEnumerable<RoomResDto>>(rooms)
      );
    }

    public async Task<ApiResponse<RoomResDto?>> GetOneByIdAsync(int id)
    {
      var room = await _roomRepository.GetByIdAsync(id);
      if (room == null)
        return ApiResponse<RoomResDto?>.ErrorResponse($"Không tìm thấy phòng {id}.", statusCode: 404);

      return ApiResponse<RoomResDto?>.SuccessResponse(_mapper.Map<RoomResDto>(room));
    }

    public async Task<ApiResponse<RoomResDto?>> UpdateRoomAsync(int id, RoomReqDto roomReqDto)
    {
      var room = await _roomRepository.GetByIdAsync(id);
      if (room == null)
        return ApiResponse<RoomResDto?>.ErrorResponse($"Không tìm thấy phòng {id}.", statusCode: 404);

      var exists = await _roomRepository.IsRoomNameExistsAsync(roomReqDto.RoomName, id);
      if (exists != null)
      {
        return ApiResponse<RoomResDto?>.ErrorResponse(
            $"Tên phòng '{roomReqDto.RoomName}' đã tồn tại",
            new Dictionary<string, string[]> { { "RoomName", new[] { "Tên phòng đã tồn tại" } } },
            409
        );
      }

      room.RoomName = roomReqDto.RoomName;
      room.RoomType = roomReqDto.RoomType;
      room.Descrt = roomReqDto.Descrt;
      room.Status = roomReqDto.Status;
      room.UpdatedAt = DateTime.Now;

      _roomRepository.UpdateAsync(room);
      await _roomRepository.SaveChangeAsync();

      return ApiResponse<RoomResDto?>.SuccessResponse(
          _mapper.Map<RoomResDto>(room), "Cập nhật phòng thành công."
      );
    }

    public async Task<ApiResponse<PagedResponse<RoomResDto>>> GetPagedRoomsAsync(RoomFilterRequest req)
    {
      var (data, totalRecords) = await _roomRepository.GetPagedRoomsAsync(
          req.Page, req.PageSize, req.Search,
          req.RoomType, req.Status,
          req.SortBy, req.SortDesc, req.IsDeleted
      );

      var result = new PagedResponse<RoomResDto>
      {
        Data = _mapper.Map<IEnumerable<RoomResDto>>(data),
        Page = req.Page,
        PageSize = req.PageSize,
        TotalRecords = totalRecords
      };

      return ApiResponse<PagedResponse<RoomResDto>>.SuccessResponse(result);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassSessionDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
    public class ClassSessionService : IClassSessionService
    {
        private readonly IClassSessionRepository _sessionRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ITeacherAssignmentRepository _assignmentRepository;
        private readonly IMapper _mapper;

        public ClassSessionService(
            IClassSessionRepository sessionRepository,
            IRoomRepository roomRepository,
            ITeacherAssignmentRepository assignmentRepository,
            IMapper mapper)
        {
            _sessionRepository = sessionRepository;
            _roomRepository = roomRepository;
            _assignmentRepository = assignmentRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ClassSessionResDto?>> CreateAsync(ClassSessionReqDto reqDto)
        {
            if (reqDto.EndTime <= reqDto.StartTime)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Thời gian kết thúc phải sau thời gian bắt đầu",
                    new Dictionary<string, string[]> { { "EndTime", new[] { "Thời gian không hợp lệ" } } },
                    400
                );

            var room = await _roomRepository.GetByIdAsync(reqDto.RoomId);
            if (room == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy phòng {reqDto.RoomId}.", statusCode: 404
                );

            var assignment = await _assignmentRepository.GetByIdAsync(reqDto.TeacherAssignmentId);
            if (assignment == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy phân công {reqDto.TeacherAssignmentId}.", statusCode: 404
                );

            var roomConflict = await _sessionRepository.IsRoomConflictAsync(
                reqDto.RoomId, reqDto.StartTime, reqDto.EndTime
            );
            if (roomConflict)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Phòng học đã có lịch trong khung giờ này.",
                    new Dictionary<string, string[]> { { "RoomId", new[] { "Phòng bị trùng lịch" } } },
                    409
                );

            var teacherConflict = await _sessionRepository.IsTeacherConflictAsync(
                assignment.TeacherId, reqDto.StartTime, reqDto.EndTime
            );
            if (teacherConflict)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Giáo viên đã có lịch dạy trong khung giờ này.",
                    new Dictionary<string, string[]> { { "TeacherAssignmentId", new[] { "Giáo viên bị trùng lịch" } } },
                    409
                );

            var session = _mapper.Map<ClassSession>(reqDto);
            await _sessionRepository.AddAsync(session);
            await _sessionRepository.SaveChangeAsync();

            return ApiResponse<ClassSessionResDto?>.SuccessResponse(
                _mapper.Map<ClassSessionResDto>(session), "Tạo buổi học thành công.", 201
            );
        }

        public async Task<ApiResponse<ClassSessionResDto?>> UpdateAsync(int id, ClassSessionReqDto reqDto)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy buổi học {id}.", statusCode: 404
                );

            if (reqDto.EndTime <= reqDto.StartTime)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Thời gian kết thúc phải sau thời gian bắt đầu",
                    new Dictionary<string, string[]> { { "EndTime", new[] { "Thời gian không hợp lệ" } } },
                    400
                );

            var room = await _roomRepository.GetByIdAsync(reqDto.RoomId);
            if (room == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy phòng {reqDto.RoomId}.", statusCode: 404
                );

            var assignment = await _assignmentRepository.GetByIdAsync(reqDto.TeacherAssignmentId);
            if (assignment == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy phân công {reqDto.TeacherAssignmentId}.", statusCode: 404
                );

            var roomConflict = await _sessionRepository.IsRoomConflictAsync(
                reqDto.RoomId, reqDto.StartTime, reqDto.EndTime, id
            );
            if (roomConflict)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Phòng học đã có lịch trong khung giờ này.",
                    new Dictionary<string, string[]> { { "RoomId", new[] { "Phòng bị trùng lịch" } } },
                    409
                );

            var teacherConflict = await _sessionRepository.IsTeacherConflictAsync(
                assignment.TeacherId, reqDto.StartTime, reqDto.EndTime, id
            );
            if (teacherConflict)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    "Giáo viên đã có lịch dạy trong khung giờ này.",
                    new Dictionary<string, string[]> { { "TeacherAssignmentId", new[] { "Giáo viên bị trùng lịch" } } },
                    409
                );

            session.StartTime = reqDto.StartTime;
            session.EndTime = reqDto.EndTime;
            session.Lesson = reqDto.Lesson;
            session.Status = reqDto.Status;
            session.RoomId = reqDto.RoomId;
            session.TeacherAssignmentId = reqDto.TeacherAssignmentId;
            session.UpdatedAt = DateTime.Now;

            _sessionRepository.UpdateAsync(session);
            await _sessionRepository.SaveChangeAsync();

            return ApiResponse<ClassSessionResDto?>.SuccessResponse(
                _mapper.Map<ClassSessionResDto>(session), "Cập nhật buổi học thành công."
            );
        }

        public async Task<ApiResponse<ClassSessionResDto?>> UpdateStatusAsync(int id, int status)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy buổi học {id}.", statusCode: 404
                );

            session.Status = (ClassSessionStatus)status; 
            session.UpdatedAt = DateTime.Now;

            _sessionRepository.UpdateAsync(session);
            await _sessionRepository.SaveChangeAsync();

            return ApiResponse<ClassSessionResDto?>.SuccessResponse(
                _mapper.Map<ClassSessionResDto>(session), "Cập nhật trạng thái buổi học thành công."
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
                return ApiResponse<bool>.ErrorResponse($"Không tìm thấy buổi học {id}.", statusCode: 404);

            session.IsDeleted = true;
            _sessionRepository.UpdateAsync(session);
            await _sessionRepository.SaveChangeAsync();

            return ApiResponse<bool>.SuccessResponse(true, $"Xóa buổi học {id} thành công.");
        }

        public async Task<ApiResponse<IEnumerable<ClassSessionResDto>>> GetAllAsync()
        {
            var sessions = await _sessionRepository.GetAllAsync();
            return ApiResponse<IEnumerable<ClassSessionResDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<ClassSessionResDto>>(sessions)
            );
        }

        public async Task<ApiResponse<ClassSessionResDto?>> GetOneByIdAsync(int id)
        {
            var session = await _sessionRepository.GetByIdAsync(id);
            if (session == null)
                return ApiResponse<ClassSessionResDto?>.ErrorResponse(
                    $"Không tìm thấy buổi học {id}.", statusCode: 404
                );

            return ApiResponse<ClassSessionResDto?>.SuccessResponse(_mapper.Map<ClassSessionResDto>(session));
        }

        public async Task<ApiResponse<PagedResponse<ClassSessionResDto>>> GetPagedSessionsAsync(ClassSessionFilterRequest req)
        {
            var (data, totalRecords) = await _sessionRepository.GetPagedSessionsAsync(
                req.Page, req.PageSize, req.Search,
                req.RoomId, req.ClassId, req.TeacherId,
                req.Status, req.StartTimeFrom, req.StartTimeTo,
                req.SortBy, req.SortDesc
            );

            var result = new PagedResponse<ClassSessionResDto>
            {
                Data = _mapper.Map<IEnumerable<ClassSessionResDto>>(data),
                Page = req.Page,
                PageSize = req.PageSize,
                TotalRecords = totalRecords
            };

            return ApiResponse<PagedResponse<ClassSessionResDto>>.SuccessResponse(result);
        }
    }
}
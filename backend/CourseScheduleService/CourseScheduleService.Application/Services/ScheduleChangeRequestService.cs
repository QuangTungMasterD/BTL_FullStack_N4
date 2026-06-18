using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
    public class ScheduleChangeRequestService : IScheduleChangeRequestService
    {
        private readonly IScheduleChangeRequestRepository _requestRepository;
        private readonly IClassSessionRepository _classSessionRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IMapper _mapper;

        public ScheduleChangeRequestService(
            IScheduleChangeRequestRepository requestRepository,
            IClassSessionRepository classSessionRepository,
            IRoomRepository roomRepository,
            ITeacherRepository teacherRepository,
            IMapper mapper)
        {
            _requestRepository = requestRepository;
            _classSessionRepository = classSessionRepository;
            _roomRepository = roomRepository;
            _teacherRepository = teacherRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ScheduleChangeRequestResDto>> CreateRequestAsync(int teacherId, ScheduleChangeRequestReqDto reqDto)
        {
            var session = await _classSessionRepository.GetByIdAsync(reqDto.ClassSessionId);
            if (session == null)
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Buổi học không tồn tại", statusCode: 404);

            var assignment = await _classSessionRepository.GetTeacherAssignmentAsync(reqDto.ClassSessionId);
            if (assignment == null || assignment.TeacherId != teacherId)
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Bạn không có quyền yêu cầu đổi lịch buổi này", statusCode: 403);

            var existing = await _requestRepository.GetByTeacherIdAsync(teacherId);
            if (existing.Any(r => r.ClassSessionId == reqDto.ClassSessionId && r.Status == "Pending"))
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Đã có yêu cầu đang chờ xử lý", statusCode: 409);

            // Validate PreferredSession
            if (reqDto.RequestType == "Change" && string.IsNullOrEmpty(reqDto.PreferredSession))
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Vui lòng chọn sáng hoặc chiều", statusCode: 400);

            var request = new ScheduleChangeRequest
            {
                ClassSessionId = reqDto.ClassSessionId,
                TeacherId = teacherId,
                RequestType = reqDto.RequestType,
                Reason = reqDto.Reason,
                SuggestedDate = reqDto.SuggestedDate.HasValue ? DateOnly.FromDateTime(reqDto.SuggestedDate.Value) : null,
                PreferredSession = reqDto.PreferredSession,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangeAsync();

            var result = await MapToResponseDto(request);
            return ApiResponse<ScheduleChangeRequestResDto>.SuccessResponse(result, "Yêu cầu đã được gửi", 201);
        }

        public async Task<ApiResponse<List<ScheduleChangeRequestResDto>>> GetMyRequestsAsync(int teacherId)
        {
            var requests = await _requestRepository.GetByTeacherIdAsync(teacherId);
            var result = new List<ScheduleChangeRequestResDto>();
            foreach (var req in requests)
                result.Add(await MapToResponseDto(req));
            return ApiResponse<List<ScheduleChangeRequestResDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<List<ScheduleChangeRequestResDto>>> GetAllPendingRequestsAsync()
        {
            var requests = await _requestRepository.GetPendingRequestsAsync();
            var result = new List<ScheduleChangeRequestResDto>();
            foreach (var req in requests)
                result.Add(await MapToResponseDto(req));
            return ApiResponse<List<ScheduleChangeRequestResDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<ScheduleChangeRequestResDto>> ProcessRequestAsync(int adminId, AdminActionDto actionDto)
        {
            var request = await _requestRepository.GetWithDetailsAsync(actionDto.RequestId);
            if (request == null)
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Không tìm thấy yêu cầu", statusCode: 404);

            if (request.Status.ToLower() != "pending")
                return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Yêu cầu đã được xử lý", statusCode: 409);

            request.AdminNote = actionDto.AdminNote;
            request.ProcessedAt = DateTime.Now;
            request.ProcessedBy = adminId;

            if (actionDto.Action.ToLower() == "approve")
            {
                request.Status = "Approved";
                var oldSession = request.ClassSession;
                var teacherAssignment = oldSession.TeacherAssignment;

                // Đánh dấu buổi cũ là Cancelled
                oldSession.Status = ClassSessionStatus.Cancelled;
                _classSessionRepository.UpdateAsync(oldSession);
                await _classSessionRepository.SaveChangeAsync();
                Console.WriteLine("==========================OK");
                if (request.RequestType.ToLower() == "cancel")
                {
                    // Chỉ cần hủy, không tạo mới
                }
                else if (request.RequestType.ToLower() == "change")
                {
                    Console.WriteLine("======== INTO CHANGE");
                    if (!request.SuggestedDate.HasValue || string.IsNullOrEmpty(request.PreferredSession))
                    {
                        return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse("Thiếu ngày hoặc ca đề xuất", statusCode: 400);
                    }
                    var newSession = await CreateSessionOnSuggestedDate(oldSession, request.PreferredSession, request.SuggestedDate.Value);
                    Console.WriteLine("===============================");
                    Console.WriteLine("===============================");
                    Console.WriteLine("===============================");
                    Console.WriteLine("===============================");
                    Console.WriteLine("===============================");
                    Console.WriteLine(newSession?.RoomId);
                    if (newSession == null)
                    {
                        return ApiResponse<ScheduleChangeRequestResDto>.ErrorResponse(
                            "Không thể xếp lịch vào ngày và ca đã chọn (phòng hoặc giáo viên bận)",
                            statusCode: 409);
                    }
                    await _classSessionRepository.AddAsync(newSession);
                    await _classSessionRepository.SaveChangeAsync();
                }
            }
            else
            {
                request.Status = "Rejected";
            }

            _requestRepository.UpdateAsync(request);
            await _requestRepository.SaveChangeAsync();

            var result = await MapToResponseDto(request);
            return ApiResponse<ScheduleChangeRequestResDto>.SuccessResponse(result, $"Yêu cầu đã được {actionDto.Action.ToLower()}");
        }

        private async Task<ClassSession?> CreateSessionOnSuggestedDate(ClassSession oldSession, string preferredSession, DateOnly suggestedDate)
        {
            var teacherId = oldSession.TeacherAssignment.TeacherId;
            var targetDate = suggestedDate.ToDateTime(TimeOnly.MinValue);

            DateTime slotStart, slotEnd;
            if (preferredSession.ToLower() == "morning")
            {
                slotStart = targetDate.Date.AddHours(7).AddMinutes(20);
                slotEnd = slotStart.AddHours(3);
            }
            else if (preferredSession.ToLower() == "afternoon")
            {
                slotStart = targetDate.Date.AddHours(13).AddMinutes(15);
                slotEnd = slotStart.AddHours(3);
            }
            else return null;

            // Lấy danh sách phòng Available
            var allRooms = await _roomRepository.GetAllAsync();
            var availableRooms = allRooms.Where(r => r.Status == RoomStatus.Available && !r.IsDeleted).ToList();
            if (availableRooms.Count == 0) return null;

            // Tìm phòng trống tại slot đó
            Room? freeRoom = null;
            foreach (var room in availableRooms)
            {
                bool roomBusy = await _classSessionRepository.IsRoomConflictAsync(room.Id, slotStart, slotEnd);
                if (!roomBusy)
                {
                    freeRoom = room;
                    break;
                }
            }
            if (freeRoom == null) return null;

            // Kiểm tra giáo viên trống
            bool teacherBusy = await _classSessionRepository.IsTeacherConflictAsync(teacherId, slotStart, slotEnd);
            if (teacherBusy) return null;

            return new ClassSession
            {
                StartTime = slotStart,
                EndTime = slotEnd,
                Lesson = oldSession.Lesson,
                Status = ClassSessionStatus.Scheduled,
                RoomId = freeRoom.Id,
                TeacherAssignmentId = oldSession.TeacherAssignmentId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };
        }

        private async Task<ScheduleChangeRequestResDto> MapToResponseDto(ScheduleChangeRequest request)
        {
            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId);
            var session = request.ClassSession;
            var originalRoom = await _roomRepository.GetByIdAsync(session.RoomId);
            var classEntity = await _classSessionRepository.GetClassBySessionIdAsync(session.Id);

            return new ScheduleChangeRequestResDto
            {
                Id = request.Id,
                ClassSessionId = request.ClassSessionId,
                TeacherId = request.TeacherId,
                TeacherName = teacher?.FullName ?? "",
                RequestType = request.RequestType,
                Reason = request.Reason,
                PreferredSession = request.PreferredSession,
                Status = request.Status,
                AdminNote = request.AdminNote,
                SuggestedDate = request.SuggestedDate,
                ProcessedAt = request.ProcessedAt,
                CreatedAt = request.CreatedAt,
                OriginalStartTime = session.StartTime,
                OriginalEndTime = session.EndTime,
                OriginalRoomId = session.RoomId,
                OriginalRoomName = originalRoom?.RoomName ?? "",
                ClassName = classEntity?.ClassName ?? ""
            };
        }
    }
}
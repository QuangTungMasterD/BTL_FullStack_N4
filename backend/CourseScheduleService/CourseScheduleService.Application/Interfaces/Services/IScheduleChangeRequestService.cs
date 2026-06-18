using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface IScheduleChangeRequestService
    {
        Task<ApiResponse<ScheduleChangeRequestResDto>> CreateRequestAsync(int teacherId, ScheduleChangeRequestReqDto reqDto);
        Task<ApiResponse<List<ScheduleChangeRequestResDto>>> GetMyRequestsAsync(int teacherId);
        Task<ApiResponse<List<ScheduleChangeRequestResDto>>> GetAllPendingRequestsAsync();
        Task<ApiResponse<ScheduleChangeRequestResDto>> ProcessRequestAsync(int adminId, AdminActionDto actionDto);
    }
}
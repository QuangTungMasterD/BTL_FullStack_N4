using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/v1/schedule-change-requests")]
    public class ScheduleChangeRequestController : ControllerBase
    {
        private readonly IScheduleChangeRequestService _requestService;

        public ScheduleChangeRequestController(IScheduleChangeRequestService requestService)
        {
            _requestService = requestService;
        }

        // Teacher endpoints
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ScheduleChangeRequestResDto>>> CreateRequest([FromBody] ScheduleChangeRequestReqDto reqDto)
        {
            // Lấy teacherId từ token (giả sử có claim "TeacherId" hoặc từ HeaderClaimsMiddleware)
            var teacherId = GetCurrentTeacherId();
            if (teacherId == 0) return Unauthorized();
            var result = await _requestService.CreateRequestAsync(teacherId, reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<ApiResponse<List<ScheduleChangeRequestResDto>>>> GetMyRequests()
        {
            var teacherId = GetCurrentTeacherId();
            if (teacherId == 0) return Unauthorized();
            var result = await _requestService.GetMyRequestsAsync(teacherId);
            return StatusCode(result.StatusCode, result);
        }

        // Admin endpoints (có thể thêm role check)
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<List<ScheduleChangeRequestResDto>>>> GetPendingRequests()
        {
            // Kiểm tra role admin
            var result = await _requestService.GetAllPendingRequestsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("process")]
        public async Task<ActionResult<ApiResponse<ScheduleChangeRequestResDto>>> ProcessRequest([FromBody] AdminActionDto actionDto)
        {
            // Lấy adminId từ token
            var adminId = GetCurrentAdminId();
            if (adminId == 0) return Unauthorized();
            var result = await _requestService.ProcessRequestAsync(adminId, actionDto);
            return StatusCode(result.StatusCode, result);
        }

        private int GetCurrentTeacherId()
        {
            // Lấy từ claims, mặc định dùng header X-User-Id (từ HeaderClaimsMiddleware)
            return 1;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userId, out int id) ? id : 0;
        }

        private int GetCurrentAdminId() => GetCurrentTeacherId(); // tạm thời
    }
}
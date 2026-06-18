using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassSessionDtos;
using CourseScheduleService.Domain.Enums;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface IClassSessionService
    {
        Task<ApiResponse<ClassSessionResDto?>> GetOneByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ClassSessionResDto>>> GetAllAsync();
        Task<ApiResponse<ClassSessionResDto?>> CreateAsync(ClassSessionReqDto reqDto);
        Task<ApiResponse<ClassSessionResDto?>> UpdateAsync(int id, ClassSessionReqDto reqDto);
        Task<ApiResponse<ClassSessionResDto?>> UpdateStatusAsync(int id, int status);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<PagedResponse<ClassSessionResDto>>> GetPagedSessionsAsync(ClassSessionFilterRequest req);
    }
}
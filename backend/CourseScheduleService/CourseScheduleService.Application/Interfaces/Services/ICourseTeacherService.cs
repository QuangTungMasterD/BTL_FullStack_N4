using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseTeacherDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface ICourseTeacherService
    {
        Task<ApiResponse<IEnumerable<CourseTeacherResDto>>> GetAllAsync();
        Task<ApiResponse<CourseTeacherResDto?>> CreateAsync(CourseTeacherReqDto reqDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
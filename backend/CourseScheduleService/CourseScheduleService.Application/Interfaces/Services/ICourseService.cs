using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;

namespace CourseScheduleService.interfaces.services
{
    public interface ICourseService
    {
        Task<ApiResponse<CourseResDto?>> GetOneByIdAsync(int id);
        Task<ApiResponse<IEnumerable<CourseResDto>>> GetAllCourseAsync();
        Task<ApiResponse<CourseResDto?>> CreateCourseAsync(CourseReqDto courseReq);
        Task<ApiResponse<CourseResDto?>> UpdateCourseAsync(int id, CourseReqDto courseReq);
        Task<ApiResponse<bool>> DeteleCourseAsync(int id);
        Task<ApiResponse<bool>> HardDeleteCourseAsync(int id);
        Task<ApiResponse<CourseResDto?>> RestoreCourseAsync(int id);
        Task<ApiResponse<PagedResponse<CourseResDto>>> GetPagedCoursesAsync(CourseFilterRequest req);
    }
}
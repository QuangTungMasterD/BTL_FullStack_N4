using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherAssignmentDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface ITeacherAssignmentService
    {
        Task<ApiResponse<IEnumerable<TeacherAssignmentResDto>>> GetAllAsync();
        Task<ApiResponse<TeacherAssignmentResDto?>> CreateAsync(TeacherAssignmentReqDto reqDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherSpecializationDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface ITeacherSpecializationService
    {
        Task<ApiResponse<IEnumerable<TeacherSpecializationResDto>>> GetAllAsync();
        Task<ApiResponse<TeacherSpecializationResDto?>> CreateAsync(TeacherSpecializationReqDto reqDto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
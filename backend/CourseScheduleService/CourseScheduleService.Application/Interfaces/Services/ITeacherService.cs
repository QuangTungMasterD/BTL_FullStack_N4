using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassDtos;
using CourseScheduleService.Application.DTOs.TeacherDtos;

namespace CourseScheduleService.Application.Interfaces.Services
{
    public interface ITeacherService
    {
        Task<ApiResponse<TeacherResDto?>> GetOneByIdAsync(int id);
        Task<ApiResponse<IEnumerable<TeacherResDto>>> GetAllTeacherAsync();
        Task<ApiResponse<TeacherResDto?>> CreateTeacherAsync(TeacherReqDto teacherReqDto);
        Task<ApiResponse<bool>> HardDeleteTeacherAsync(int id);
        Task<ApiResponse<TeacherResDto?>> RestoreTeacherAsync(int id);
        Task<ApiResponse<TeacherResDto?>> UpdateTeacherAsync(int id, TeacherReqDto teacherReqDto);
        Task<ApiResponse<bool>> DeteleTeacherAsync(int id);
        Task<ApiResponse<PagedResponse<TeacherResDto>>> GetPagedTeachersAsync(TeacherFilterRequest req);
        Task<ApiResponse<IEnumerable<ClassResDto>>> GetClassesByTeacherAsync(int teacherId);
    }
}
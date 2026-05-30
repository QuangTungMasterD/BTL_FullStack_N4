using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.SpecializationDtos;

namespace CourseScheduleService.interfaces.services
{
    public interface ISpecializationService
    {
        Task<ApiResponse<SpecializationResDto?>> GetOneByIdAsync(int id);
        Task<ApiResponse<IEnumerable<SpecializationResDto>>> GetAllSpecializationAsync();
        Task<ApiResponse<SpecializationResDto>> CreateSpecializationAsync(SpecializationReqDto specializationReq);
        Task<ApiResponse<SpecializationResDto?>> UpdateSpecializationAsync(int id, SpecializationReqDto specializationReq);
        Task<ApiResponse<bool>> DeteleSpecializationAsync(int id);
    }
}
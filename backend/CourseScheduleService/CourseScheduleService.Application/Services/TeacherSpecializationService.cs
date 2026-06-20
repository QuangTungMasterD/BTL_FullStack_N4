using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherSpecializationDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
    public class TeacherSpecializationService : ITeacherSpecializationService
    {
        private readonly ITeacherSpecializationRepository _tsRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IMapper _mapper;

        public TeacherSpecializationService(
            ITeacherSpecializationRepository tsRepository,
            ITeacherRepository teacherRepository,
            ISpecializationRepository specializationRepository,
            IMapper mapper)
        {
            _tsRepository = tsRepository;
            _teacherRepository = teacherRepository;
            _specializationRepository = specializationRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<TeacherSpecializationResDto>>> GetAllAsync()
        {
            var list = await _tsRepository.GetAllAsync();
            return ApiResponse<IEnumerable<TeacherSpecializationResDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<TeacherSpecializationResDto>>(list)
            );
        }

        public async Task<ApiResponse<TeacherSpecializationResDto?>> CreateAsync(TeacherSpecializationReqDto reqDto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(reqDto.TeacherId);
            if (teacher == null)
                return ApiResponse<TeacherSpecializationResDto?>.ErrorResponse(
                    $"Không tìm thấy giáo viên {reqDto.TeacherId}.", statusCode: 404
                );

            var spec = await _specializationRepository.GetByIdAsync(reqDto.SpecializationId);
            if (spec == null)
                return ApiResponse<TeacherSpecializationResDto?>.ErrorResponse(
                    $"Không tìm thấy chuyên ngành {reqDto.SpecializationId}.", statusCode: 404
                );

            var exists = await _tsRepository.GetByTeacherAndSpecializationAsync(reqDto.TeacherId, reqDto.SpecializationId);
            if (exists != null)
                return ApiResponse<TeacherSpecializationResDto?>.ErrorResponse(
                    $"Giáo viên {reqDto.TeacherId} đã có chuyên ngành {reqDto.SpecializationId}.",
                    new Dictionary<string, string[]> { { "TeacherSpecialization", new[] { "Liên kết đã tồn tại" } } },
                    409
                );

            var entity = _mapper.Map<TeacherSpecialization>(reqDto);
            await _tsRepository.AddAsync(entity);
            await _tsRepository.SaveChangeAsync();

            return ApiResponse<TeacherSpecializationResDto?>.SuccessResponse(
                _mapper.Map<TeacherSpecializationResDto>(entity), "Thêm chuyên ngành cho giáo viên thành công.", 201
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _tsRepository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse($"Không tìm thấy liên kết {id}.", statusCode: 404);

            entity.IsDeleted = true;
            _tsRepository.UpdateAsync(entity);
            await _tsRepository.SaveChangeAsync();

            return ApiResponse<bool>.SuccessResponse(true, $"Xóa liên kết {id} thành công.");
        }
    }
}
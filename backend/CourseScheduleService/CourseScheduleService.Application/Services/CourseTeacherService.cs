using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseTeacherDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
    public class CourseTeacherService : ICourseTeacherService
    {
        private readonly ICourseTeacherRepository _ctRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseTeacherService(
            ICourseTeacherRepository ctRepository,
            ITeacherRepository teacherRepository,
            ICourseRepository courseRepository,
            IMapper mapper)
        {
            _ctRepository = ctRepository;
            _teacherRepository = teacherRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<CourseTeacherResDto>>> GetAllAsync()
        {
            var list = await _ctRepository.GetAllAsync();
            return ApiResponse<IEnumerable<CourseTeacherResDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<CourseTeacherResDto>>(list)
            );
        }

        public async Task<ApiResponse<CourseTeacherResDto?>> CreateAsync(CourseTeacherReqDto reqDto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(reqDto.TeacherId);
            if (teacher == null)
                return ApiResponse<CourseTeacherResDto?>.ErrorResponse(
                    $"Không tìm thấy giáo viên {reqDto.TeacherId}.", statusCode: 404
                );

            var spec = await _courseRepository.GetByIdAsync(reqDto.CourseId);
            if (spec == null)
                return ApiResponse<CourseTeacherResDto?>.ErrorResponse(
                    $"Không tìm thấy khóa học {reqDto.CourseId}.", statusCode: 404
                );

            var exists = await _ctRepository.GetByTeacherAndCourseAsync(reqDto.TeacherId, reqDto.CourseId);
            if (exists != null)
                return ApiResponse<CourseTeacherResDto?>.ErrorResponse(
                    $"Giáo viên {reqDto.TeacherId} đã có khóa học {reqDto.CourseId}.",
                    new Dictionary<string, string[]> { { "CourseTeacher", new[] { "Liên kết đã tồn tại" } } },
                    409
                );

            var entity = _mapper.Map<CourseTeacher>(reqDto);
            await _ctRepository.AddAsync(entity);
            await _ctRepository.SaveChangeAsync();

            return ApiResponse<CourseTeacherResDto?>.SuccessResponse(
                _mapper.Map<CourseTeacherResDto>(entity), "Thêm khóa học cho giáo viên thành công.", 201
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await _ctRepository.GetByIdAsync(id);
            if (entity == null)
                return ApiResponse<bool>.ErrorResponse($"Không tìm thấy liên kết {id}.", statusCode: 404);

            entity.IsDeleted = true;
            _ctRepository.UpdateAsync(entity);
            await _ctRepository.SaveChangeAsync();

            return ApiResponse<bool>.SuccessResponse(true, $"Xóa liên kết {id} thành công.");
        }
    }
}
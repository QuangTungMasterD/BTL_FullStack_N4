using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherAssignmentDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
    public class TeacherAssignmentService : ITeacherAssignmentService
    {
        private readonly ITeacherAssignmentRepository _assignmentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IClassRepository _classRepository;
        private readonly IMapper _mapper;

        public TeacherAssignmentService(
            ITeacherAssignmentRepository assignmentRepository,
            ITeacherRepository teacherRepository,
            IClassRepository classRepository,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _teacherRepository = teacherRepository;
            _classRepository = classRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<TeacherAssignmentResDto>>> GetAllAsync()
        {
            var assignments = await _assignmentRepository.GetAllAsync();
            return ApiResponse<IEnumerable<TeacherAssignmentResDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<TeacherAssignmentResDto>>(assignments)
            );
        }

        public async Task<ApiResponse<TeacherAssignmentResDto?>> CreateAsync(TeacherAssignmentReqDto reqDto)
        {
            var teacher = await _teacherRepository.GetByIdAsync(reqDto.TeacherId);
            if (teacher == null)
                return ApiResponse<TeacherAssignmentResDto?>.ErrorResponse(
                    $"Không tìm thấy giáo viên {reqDto.TeacherId}.", statusCode: 404
                );

            var cls = await _classRepository.GetByIdAsync(reqDto.ClassId);
            if (cls == null)
                return ApiResponse<TeacherAssignmentResDto?>.ErrorResponse(
                    $"Không tìm thấy lớp học {reqDto.ClassId}.", statusCode: 404
                );

            // Kiểm tra đã assign chưa
            var exists = await _assignmentRepository.GetByTeacherAndClassAsync(reqDto.TeacherId, reqDto.ClassId);
            if (exists != null)
                return ApiResponse<TeacherAssignmentResDto?>.ErrorResponse(
                    $"Giáo viên {reqDto.TeacherId} đã được phân công lớp {reqDto.ClassId}.",
                    new Dictionary<string, string[]> { { "Assignment", new[] { "Phân công đã tồn tại" } } },
                    409
                );

            var assignment = _mapper.Map<TeacherAssignment>(reqDto);
            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangeAsync();

            return ApiResponse<TeacherAssignmentResDto?>.SuccessResponse(
                _mapper.Map<TeacherAssignmentResDto>(assignment), "Phân công giáo viên thành công.", 201
            );
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
                return ApiResponse<bool>.ErrorResponse($"Không tìm thấy phân công {id}.", statusCode: 404);

            assignment.IsDeleted = true;
            _assignmentRepository.UpdateAsync(assignment);
            await _assignmentRepository.SaveChangeAsync();

            return ApiResponse<bool>.SuccessResponse(true, $"Hủy phân công {id} thành công.");
        }
    }
}
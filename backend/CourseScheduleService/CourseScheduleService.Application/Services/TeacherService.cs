using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherDtos;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
  public class TeacherService : ITeacherService
  {
    private readonly ITeacherRepository _teacherRepository;
    private readonly IMapper _mapper;

    public TeacherService(ITeacherRepository teacherRepository, IMapper mapper)
    {
      _teacherRepository = teacherRepository;
      _mapper = mapper;
    }

    public async Task<ApiResponse<TeacherResDto?>> CreateTeacherAsync(TeacherReqDto teacherReqDto)
    {
      var emailExists = await _teacherRepository.IsEmailExistsAsync(teacherReqDto.Email);
      if (emailExists != null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse(
          $"Email '{teacherReqDto.Email}' đã được sử dụng",
          new Dictionary<string, string[]> { { "Email", new string[] { "Email đã tồn tại" } } },
          409
        );
      }

      var phoneExists = await _teacherRepository.IsPhoneExistsAsync(teacherReqDto.Phone);
      if (phoneExists != null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse(
          $"Số điện thoại '{teacherReqDto.Phone}' đã được sử dụng",
          new Dictionary<string, string[]> { { "Phone", new string[] { "Số điện thoại đã tồn tại" } } },
          409
        );
      }

      Teacher newTeacher = _mapper.Map<Teacher>(teacherReqDto);

      await _teacherRepository.AddAsync(newTeacher);
      await _teacherRepository.SaveChangeAsync();

      TeacherResDto teacherRes = _mapper.Map<TeacherResDto>(newTeacher);

      return ApiResponse<TeacherResDto?>.SuccessResponse(teacherRes, "Tạo giáo viên thành công", 201);
    }

    public async Task<ApiResponse<bool>> DeteleTeacherAsync(int id)
    {
      var teacher = await _teacherRepository.GetByIdAsync(id);
      if (teacher == null)
      {
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy giáo viên {id}.");
      }

      teacher.IsDeleted = true;
      _teacherRepository.UpdateAsync(teacher);
      await _teacherRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa giáo viên {id} thành công.");
    }

    public async Task<ApiResponse<IEnumerable<TeacherResDto>>> GetAllTeacherAsync()
    {
      var teachers = await _teacherRepository.GetAllAsync();

      var teacherRes = _mapper.Map<IEnumerable<TeacherResDto>>(teachers);
      return ApiResponse<IEnumerable<TeacherResDto>>.SuccessResponse(teacherRes);
    }

    public async Task<ApiResponse<TeacherResDto?>> GetOneByIdAsync(int id)
    {
      var teacher = await _teacherRepository.GetByIdAsync(id);
      if (teacher == null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse($"Không tìm thấy giáo viên {id}.");
      }

      TeacherResDto teacherRes = _mapper.Map<TeacherResDto>(teacher);
      return ApiResponse<TeacherResDto?>.SuccessResponse(teacherRes, $"Lấy thông tin giáo viên {id} thành công");
    }

    public async Task<ApiResponse<bool>> HardDeleteTeacherAsync(int id)
    {
      var teacher = await _teacherRepository.GetByIdAsync(id);
      if (teacher == null)
      {
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy giáo viên {id}.", statusCode: 404);
      }

      if (!teacher.IsDeleted)
      {
        return ApiResponse<bool>.ErrorResponse(
          $"Giáo viên {id} chưa được xóa mềm, không thể xóa vĩnh viễn.",
          statusCode: 409
        );
      }

      _teacherRepository.DeleteAsync(teacher);
      await _teacherRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa vĩnh viễn giáo viên {id} thành công.");
    }

    public async Task<ApiResponse<TeacherResDto?>> RestoreTeacherAsync(int id)
    {
      var teacher = await _teacherRepository.GetByIdAsync(id);
      if (teacher == null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse($"Không tìm thấy giáo viên {id}.", statusCode: 404);
      }

      if (!teacher.IsDeleted)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse(
          $"Giáo viên {id} chưa bị xóa, không cần khôi phục.",
          statusCode: 409
        );
      }

      teacher.IsDeleted = false;
      _teacherRepository.UpdateAsync(teacher);
      await _teacherRepository.SaveChangeAsync();

      return ApiResponse<TeacherResDto?>.SuccessResponse(
        _mapper.Map<TeacherResDto>(teacher),
        $"Khôi phục giáo viên {id} thành công."
      );
    }

    public async Task<ApiResponse<TeacherResDto?>> UpdateTeacherAsync(int id, TeacherReqDto teacherReqDto)
    {
      var teacher = await _teacherRepository.GetByIdAsync(id);
      if (teacher == null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse($"Không tìm thấy giáo viên {id}.");
      }

      var emailExists = await _teacherRepository.IsEmailExistsAsync(teacherReqDto.Email, id);
      if (emailExists != null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse(
          $"Email '{teacherReqDto.Email}' đã được sử dụng",
          new Dictionary<string, string[]> { { "Email", new string[] { "Email đã tồn tại" } } },
          409
        );
      }

      var phoneExists = await _teacherRepository.IsPhoneExistsAsync(teacherReqDto.Phone, id);
      if (phoneExists != null)
      {
        return ApiResponse<TeacherResDto?>.ErrorResponse(
            $"Số điện thoại '{teacherReqDto.Phone}' đã được sử dụng",
            new Dictionary<string, string[]> { { "Phone", new string[] { "Số điện thoại đã tồn tại" } } },
            409
        );
      }

      teacher.FullName = teacherReqDto.FullName;
      teacher.Email = teacherReqDto.Email;
      teacher.Phone = teacherReqDto.Phone;
      teacher.YoB = teacherReqDto.YoB;
      teacher.IsActive = teacherReqDto.IsActive;
      teacher.UpdatedAt = DateTime.Now;

      _teacherRepository.UpdateAsync(teacher);
      await _teacherRepository.SaveChangeAsync();

      return ApiResponse<TeacherResDto?>.SuccessResponse(
        _mapper.Map<TeacherResDto>(teacher),
        "Cập nhật giáo viên thành công"
      );
    }

    public async Task<ApiResponse<PagedResponse<TeacherResDto>>> GetPagedTeachersAsync(TeacherFilterRequest req)
    {
      var (data, totalRecords) = await _teacherRepository.GetPagedTeachersAsync(
          req.Page, req.PageSize, req.Search,
          req.IsActive, req.YoBFrom, req.YoBTo,
          req.SortBy, req.SortDesc
      );

      var result = new PagedResponse<TeacherResDto>
      {
        Data = _mapper.Map<IEnumerable<TeacherResDto>>(data),
        Page = req.Page,
        PageSize = req.PageSize,
        TotalRecords = totalRecords
      };

      return ApiResponse<PagedResponse<TeacherResDto>>.SuccessResponse(result);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassDtos;
using CourseScheduleService.Application.Events;
using CourseScheduleService.Application.Interfaces.EventBus;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
  public class ClassService : IClassService
  {
    private readonly IClassRepository _classRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;

    public ClassService(IClassRepository classRepository, ICourseRepository courseRepository, IEventBus eventBus, IMapper mapper)
    {
      _classRepository = classRepository;
      _courseRepository = courseRepository;
      _eventBus = eventBus;
      _mapper = mapper;
    }

    public async Task<ApiResponse<ClassResDto?>> CreateClassAsync(ClassReqDto classReqDto)
    {
      var exists = await _classRepository.IsClassNameExistsAsync(classReqDto.ClassName);
      if (exists != null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            $"Tên lớp '{classReqDto.ClassName}' đã tồn tại",
            new Dictionary<string, string[]> { { "ClassName", new[] { "Tên lớp đã tồn tại" } } }
        );
      }

      var course = await _courseRepository.GetByIdAsync(classReqDto.CourseId);
      if (course == null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            $"Khóa học ID {classReqDto.CourseId} không tồn tại",
            new Dictionary<string, string[]> { { "CourseId", new[] { "Khóa học không tồn tại" } } },
            404
        );
      }

      if (classReqDto.EndDate <= classReqDto.StartDate)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            "Ngày kết thúc phải sau ngày bắt đầu",
            new Dictionary<string, string[]> { { "EndDate", new[] { "Ngày kết thúc không hợp lệ" } } },
            400
        );
      }

      Class newClass = _mapper.Map<Class>(classReqDto);
      await _classRepository.AddAsync(newClass);
      await _classRepository.SaveChangeAsync();

      var classOpenedEvent = new ClassOpenedEvent
      {
          ClassId = newClass.Id,
          ClassName = newClass.ClassName,
          CourseId = newClass.CourseId,
          StartDate = newClass.StartDate.ToDateTime(TimeOnly.MinValue),
          EndDate = newClass.EndDate.ToDateTime(TimeOnly.MinValue),
          MaxStudent = newClass.MaxStudent
      };

      await _eventBus.PublishAsync("class.opened", classOpenedEvent);

      return ApiResponse<ClassResDto?>.SuccessResponse(
          _mapper.Map<ClassResDto>(newClass),
          "Tạo lớp học thành công",
          201
      );
    }

    public async Task<ApiResponse<bool>> DeleteClassAsync(int id)
    {
      var cls = await _classRepository.GetByIdAsync(id);
      if (cls == null)
      {
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy lớp học {id}.", statusCode: 404);
      }

      cls.IsDeleted = true;
      _classRepository.UpdateAsync(cls);
      await _classRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa lớp học {id} thành công.");
    }

    public async Task<ApiResponse<bool>> HardDeleteClassAsync(int id)
    {
      var cls = await _classRepository.IsClassIdExistsAsync(id);
      if (cls == null)
      {
        return ApiResponse<bool>.ErrorResponse($"Không tìm thấy lớp học {id}.", statusCode: 404);
      }

      if (!cls.IsDeleted)
      {
        return ApiResponse<bool>.ErrorResponse(
            $"Lớp học {id} chưa được xóa mềm, không thể xóa vĩnh viễn.",
            statusCode: 409
        );
      }

      _classRepository.DeleteAsync(cls);
      await _classRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, $"Xóa vĩnh viễn lớp học {id} thành công.");
    }

    public async Task<ApiResponse<ClassResDto?>> RestoreClassAsync(int id)
    {
      var cls = await _classRepository.IsClassIdExistsAsync(id);
      if (cls == null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse($"Không tìm thấy lớp học {id}.", statusCode: 404);
      }

      if (!cls.IsDeleted)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            $"Lớp học {id} chưa bị xóa, không cần khôi phục.",
            statusCode: 409
        );
      }

      cls.IsDeleted = false;
      _classRepository.UpdateAsync(cls);
      await _classRepository.SaveChangeAsync();

      return ApiResponse<ClassResDto?>.SuccessResponse(
          _mapper.Map<ClassResDto>(cls),
          $"Khôi phục lớp học {id} thành công."
      );
    }

    public async Task<ApiResponse<IEnumerable<ClassResDto>>> GetAllClassAsync()
    {
      var classes = await _classRepository.GetAllAsync();
      return ApiResponse<IEnumerable<ClassResDto>>.SuccessResponse(
          _mapper.Map<IEnumerable<ClassResDto>>(classes)
      );
    }

    public async Task<ApiResponse<ClassResDto?>> GetOneByIdAsync(int id)
    {
      var cls = await _classRepository.GetByIdAsync(id);
      if (cls == null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse($"Không tìm thấy lớp học {id}.", statusCode: 404);
      }

      return ApiResponse<ClassResDto?>.SuccessResponse(
          _mapper.Map<ClassResDto>(cls),
          $"Lấy thông tin lớp học {id} thành công."
      );
    }

    public async Task<ApiResponse<ClassResDto?>> UpdateClassAsync(int id, ClassReqDto classReqDto)
    {
      var cls = await _classRepository.GetByIdAsync(id);
      if (cls == null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse($"Không tìm thấy lớp học {id}.", statusCode: 404);
      }

      var exists = await _classRepository.IsClassNameExistsAsync(classReqDto.ClassName, id);
      if (exists != null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            $"Tên lớp '{classReqDto.ClassName}' đã tồn tại",
            new Dictionary<string, string[]> { { "ClassName", new[] { "Tên lớp đã tồn tại" } } },
            409
        );
      }

      var course = await _courseRepository.GetByIdAsync(classReqDto.CourseId);
      if (course == null)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            $"Khóa học ID {classReqDto.CourseId} không tồn tại",
            new Dictionary<string, string[]> { { "CourseId", new[] { "Khóa học không tồn tại" } } },
            404
        );
      }

      if (classReqDto.EndDate <= classReqDto.StartDate)
      {
        return ApiResponse<ClassResDto?>.ErrorResponse(
            "Ngày kết thúc phải sau ngày bắt đầu",
            new Dictionary<string, string[]> { { "EndDate", new[] { "Ngày kết thúc không hợp lệ" } } },
            400
        );
      }

      cls.ClassName = classReqDto.ClassName;
      cls.MaxStudent = classReqDto.MaxStudent;
      cls.StartDate = classReqDto.StartDate;
      cls.EndDate = classReqDto.EndDate;
      cls.Status = classReqDto.Status;
      cls.Lesson = classReqDto.Lesson;
      cls.CourseId = classReqDto.CourseId;
      cls.UpdatedAt = DateTime.Now;

      _classRepository.UpdateAsync(cls);
      await _classRepository.SaveChangeAsync();

      return ApiResponse<ClassResDto?>.SuccessResponse(
          _mapper.Map<ClassResDto>(cls),
          "Cập nhật lớp học thành công."
      );
    }

    public async Task<ApiResponse<IEnumerable<ClassResDto>>> GetClassesByCourseAsync(int courseId)
    {
      var classes = await _classRepository.GetClassesByCourseAsync(courseId);
      return ApiResponse<IEnumerable<ClassResDto>>.SuccessResponse(
          _mapper.Map<IEnumerable<ClassResDto>>(classes)
      );
    }

    public async Task<ApiResponse<PagedResponse<ClassResDto>>> GetPagedClassesAsync(ClassFilterRequest req)
    {
      var (data, totalRecords) = await _classRepository.GetPagedClassesAsync(
          req.Page, req.PageSize, req.Search,
          req.CourseId, req.Status,
          req.StartDateFrom, req.StartDateTo,
          req.SortBy, req.SortDesc, req.IsDeleted
      );

      var result = new PagedResponse<ClassResDto>
      {
        Data = _mapper.Map<IEnumerable<ClassResDto>>(data),
        Page = req.Page,
        PageSize = req.PageSize,
        TotalRecords = totalRecords
      };

      return ApiResponse<PagedResponse<ClassResDto>>.SuccessResponse(result);
    }
  }
}
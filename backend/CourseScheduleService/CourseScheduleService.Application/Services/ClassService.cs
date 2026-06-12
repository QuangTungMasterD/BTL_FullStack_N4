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
using CourseScheduleService.Domain.Enums;
using CourseScheduleService.Domain.Interfaces.Repositories;

namespace CourseScheduleService.Application.Services
{
  public class ClassService : IClassService
  {
    private readonly IClassRepository _classRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEventBus _eventBus;
    private readonly IMapper _mapper;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IClassSessionRepository _classSessionRepository;
    private readonly ITeacherAssignmentRepository _teacherAssignmentRepository;

    public ClassService(
        IClassRepository classRepository,
        ICourseRepository courseRepository,
        IEventBus eventBus,
        IMapper mapper,
        ITeacherRepository teacherRepository,
        IRoomRepository roomRepository,
        IClassSessionRepository classSessionRepository,
        ITeacherAssignmentRepository teacherAssignmentRepository)
    {
        _classRepository = classRepository;
        _courseRepository = courseRepository;
        _eventBus = eventBus;
        _mapper = mapper;
        _teacherRepository = teacherRepository;
        _roomRepository = roomRepository;
        _classSessionRepository = classSessionRepository;
        _teacherAssignmentRepository = teacherAssignmentRepository;
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

      if (classReqDto.AutoSchedule)
      {
          try
          {
              await AutoAssignTeacherAndSchedule(newClass);
          }
          catch (Exception ex)
          {
              // Xóa class vừa tạo nếu xếp lịch thất bại
              _classRepository.DeleteAsync(newClass);
              await _classRepository.SaveChangeAsync();
              return ApiResponse<ClassResDto?>.ErrorResponse(ex.Message, statusCode: 400);
          }
      }

      _ = PublishClassOpenedEventAsync(newClass);

      return ApiResponse<ClassResDto?>.SuccessResponse(
          _mapper.Map<ClassResDto>(newClass),
          "Tạo lớp học thành công",
          201
      );
    }

    private async Task PublishClassOpenedEventAsync(Class newClass)
    {
        try
        {
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to publish event: {ex.Message}");
        }
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

    private async Task AutoAssignTeacherAndSchedule(Class newClass)
    {
        var course = await _courseRepository.GetByIdAsync(newClass.CourseId);
        if (course == null)
            throw new Exception($"Không tìm thấy khóa học ID {newClass.CourseId}");

        if (course.SpecializationId == null)
            throw new Exception("Khóa học chưa có chuyên ngành, không thể tự động phân công giáo viên");

        var startDateTime = newClass.StartDate.ToDateTime(TimeOnly.MinValue);
        var endDateTime = newClass.EndDate.ToDateTime(TimeOnly.MaxValue);

        var availableTeachers = await _teacherRepository.GetAvailableTeachersBySpecializationAsync(
            course.SpecializationId.Value, startDateTime, endDateTime);

        if (availableTeachers == null || availableTeachers.Count == 0)
            throw new Exception($"Không có giáo viên nào phù hợp với chuyên ngành ID {course.SpecializationId} và rảnh trong thời gian lớp học");

        var orderedTeachers = availableTeachers.OrderBy(t => t.TeacherAssignments?.Count ?? 0).ToList();

        TeacherAssignment selectedAssignment = null;
        List<ClassSession> generatedSessions = null;

        foreach (var teacher in orderedTeachers)
        {
            var (success, sessions) = await TryScheduleClassSessions(newClass, teacher.Id);
            if (success)
            {
                selectedAssignment = new TeacherAssignment
                {
                    TeacherId = teacher.Id,
                    ClassId = newClass.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };
                generatedSessions = sessions;
                break;
            }
        }

        if (selectedAssignment == null)
            throw new Exception("Không thể xếp lịch cho bất kỳ giáo viên nào");

        await _teacherAssignmentRepository.AddAsync(selectedAssignment);
        await _teacherAssignmentRepository.SaveChangeAsync();

        foreach (var session in generatedSessions)
        {
            session.TeacherAssignmentId = selectedAssignment.Id;
            await _classSessionRepository.AddAsync(session);
        }
        await _classSessionRepository.SaveChangeAsync();
    }

    private async Task<(bool Success, List<ClassSession> Sessions)> TryScheduleClassSessions(Class newClass, int teacherId)
    {
        int totalLessons = newClass.Lesson;
        int lessonsPerSession = 4;
        int numberOfSessions = (int)Math.Ceiling((double)totalLessons / lessonsPerSession);
        int remainingLessons = totalLessons;

        DateTime startDate = newClass.StartDate.ToDateTime(TimeOnly.MinValue);
        DateTime endDate = newClass.EndDate.ToDateTime(TimeOnly.MaxValue);
        var sessions = new List<ClassSession>();

        var allRooms = await _roomRepository.GetAllAsync();
        var availableRooms = allRooms.Where(r => r.Status == RoomStatus.Available && !r.IsDeleted).ToList();
        if (availableRooms.Count == 0) return (false, null);

        DateTime currentCursor = startDate;

        for (int i = 0; i < numberOfSessions; i++)
        {
            int lessonCount = Math.Min(lessonsPerSession, remainingLessons);
            bool scheduled = false;
            DateTime sessionStart = default, sessionEnd = default;
            int? chosenRoomId = null;

            DateTime cursor = currentCursor;
            while (cursor <= endDate && !scheduled)
            {
                if (cursor.DayOfWeek != DayOfWeek.Sunday)
                {
                    // Ca sáng 7:20 - 10:20
                    var morningStart = cursor.Date.AddHours(7).AddMinutes(20);
                    var morningEnd = morningStart.AddHours(3);
                    if (morningEnd <= endDate)
                    {
                        var freeRoom = await FindFreeRoom(morningStart, morningEnd, availableRooms);
                        if (freeRoom != null)
                        {
                            bool teacherFree = !(await _classSessionRepository.IsTeacherConflictAsync(teacherId, morningStart, morningEnd));
                            if (teacherFree)
                            {
                                chosenRoomId = freeRoom.Id;
                                sessionStart = morningStart;
                                sessionEnd = morningEnd;
                                scheduled = true;
                                break;
                            }
                        }
                    }

                    // Ca chiều 13:15 - 16:15
                    var afternoonStart = cursor.Date.AddHours(13).AddMinutes(15);
                    var afternoonEnd = afternoonStart.AddHours(3);
                    if (afternoonEnd <= endDate)
                    {
                        var freeRoom = await FindFreeRoom(afternoonStart, afternoonEnd, availableRooms);
                        if (freeRoom != null)
                        {
                            bool teacherFree = !(await _classSessionRepository.IsTeacherConflictAsync(teacherId, afternoonStart, afternoonEnd));
                            if (teacherFree)
                            {
                                chosenRoomId = freeRoom.Id;
                                sessionStart = afternoonStart;
                                sessionEnd = afternoonEnd;
                                scheduled = true;
                                break;
                            }
                        }
                    }
                }
                cursor = cursor.AddDays(1);
            }

            if (!scheduled) return (false, null);

            sessions.Add(new ClassSession
            {
                StartTime = sessionStart,
                EndTime = sessionEnd,
                Lesson = lessonCount,
                Status = ClassSessionStatus.Scheduled,
                RoomId = chosenRoomId.Value,
                TeacherAssignmentId = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            });

            remainingLessons -= lessonCount;
            currentCursor = sessionStart.AddDays(7);
        }
        return (true, sessions);
    }

    private async Task<Room> FindFreeRoom(DateTime start, DateTime end, List<Room> availableRooms)
    {
        foreach (var room in availableRooms)
        {
            bool isBusy = await _classSessionRepository.IsRoomConflictAsync(room.Id, start, end);
            if (!isBusy) return room;
        }
        return null;
    }
  }
}
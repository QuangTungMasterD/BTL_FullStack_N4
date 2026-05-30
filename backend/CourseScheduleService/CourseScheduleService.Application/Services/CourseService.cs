using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.interfaces.services;

namespace CourseScheduleService.Application.Services
{
  public class CourseService : ICourseService
  {
    private readonly IRepository<Course> _courseRepository;
    private readonly IMapper _mapper;

    public CourseService(IRepository<Course> courseRepository, IMapper mapper)
    {
        this._courseRepository = courseRepository;
        this._mapper = mapper;
    } 

    public async Task<ApiResponse<CourseResDto>> CreateCourseAsync(CourseReqDto courseReq)
    {
        Course newCourse = this._mapper.Map<Course>(courseReq);

        await this._courseRepository.AddAsync(newCourse);
        await this._courseRepository.SaveChangeAsync();

        CourseResDto courseRes = this._mapper.Map<CourseResDto>(newCourse);

        return ApiResponse<CourseResDto>.SuccessResponse(courseRes);
    }

    public async Task<ApiResponse<bool>> DeteleCourseAsync(int id)
    {
      var course = await this._courseRepository.GetByIdAsync(id);
      if(course == null)
      {
        return ApiResponse<bool>.ErrorResponse(String.Format($"Not found course {id}."));
      }
      course.IsDeleted = true;
      await this._courseRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, String.Format($"Deleted course {id} succcess."));
    }

    public async Task<ApiResponse<IEnumerable<CourseResDto>>> GetAllCourseAsync()
    {
      var courses = await _courseRepository.GetAllAsync();
      var courseRes = _mapper.Map<IEnumerable<CourseResDto>>(courses);
      return ApiResponse<IEnumerable<CourseResDto>>.SuccessResponse(courseRes);
    }

    public async Task<ApiResponse<CourseResDto?>> GetOneByIdAsync(int id)
    {
      var course = await this._courseRepository.GetByIdAsync(id);
      if(course == null)
      {
        return ApiResponse<CourseResDto?>.ErrorResponse($"Not found course {id}.");
      }
      CourseResDto courseRes = _mapper.Map<CourseResDto>(course);
      return ApiResponse<CourseResDto?>.SuccessResponse(courseRes, $"Get course {id} success");
    }

    public async Task<ApiResponse<CourseResDto?>> UpdateCourseAsync(int id, CourseReqDto courseReq)
    {
      throw new Exception();
      // var course = await this._courseRepository.GetByIdAsync(id);
      // if(course == null)
      // {
      //   return ApiResponse<CourseResDto?>.ErrorResponse($"Not found course {id}.");
      // }

      // var exists = await this._courseRepository.GetQueryable()
      //   .AnyAsync(c => c.CourseName == courseReq.CourseName && c.Id != id);
    
      // if (exists)
      // {
      //     return ApiResponse<CourseResDto?>.ErrorResponse($"Tên khóa học '{courseReq.CourseName}' đã tồn tại");
      // }
      
      // // 3. Kiểm tra Specialization tồn tại
      // var spec = await this._specRepo.GetByIdAsync(courseReq.SpecializationId);
      // if (spec == null)
      // {
      //     return ApiResponse<CourseResDto?>.ErrorResponse($"Chuyên ngành ID {courseReq.SpecializationId} không tồn tại");
      // }
      
      // // 4. Cập nhật thông tin
      // course.CourseName = courseReq.CourseName;
      // course.Desct = courseReq.Desct;
      // course.TuitionFee = courseReq.TuitionFee;
      // course.Level = (int)courseReq.Level;
      // course.Lesson = courseReq.Lesson;
      // course.IsActive = courseReq.IsActive;
      // course.SpecializationId = courseReq.SpecializationId;
      // course.UpdatedAt = DateTime.Now;
      
      // // 5. Lưu thay đổi
      // this._courseRepository.UpdateAsync(course);
      // await this._courseRepository.SaveChangeAsync();
      
      // // 6. Map sang DTO để trả về
      // var courseRes = this._mapper.Map<CourseResDto>(course);
      // courseRes.SpecializationName = spec.SpecializationName;
      
      // // 7. Trả về kết quả thành công
      // return ApiResponse<CourseResDto?>.SuccessResponse(courseRes, "Cập nhật khóa học thành công");
    }
  }
}
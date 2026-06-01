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
// using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Application.Services
{
  public class CourseService : ICourseService
  {
    private readonly ICourseRepository _courseRepository;
    private readonly ISpecializationRepository _specializationRepository;
    private readonly IMapper _mapper;

    public CourseService(ICourseRepository courseRepository, ISpecializationRepository specializationRepository, IMapper mapper)
    {
        this._courseRepository = courseRepository;
        this._mapper = mapper;
        _specializationRepository = specializationRepository;
    } 

    public async Task<ApiResponse<CourseResDto?>> CreateCourseAsync(CourseReqDto courseReq)
    {
      var exists = await this._courseRepository.IsCourseNameExistsAsync(courseReq.CourseName);
    
      if (exists != null)
      {
          return ApiResponse<CourseResDto?>.ErrorResponse(
            $"Course name '{courseReq.CourseName}' exist",
            new Dictionary<String, String[]> { {"CourseName", new String[] {"Tên khóa học đã tồn tại"}} }
          );
      }
      Course newCourse = this._mapper.Map<Course>(courseReq);

      await this._courseRepository.AddAsync(newCourse);
      await this._courseRepository.SaveChangeAsync();

      CourseResDto courseRes = this._mapper.Map<CourseResDto>(newCourse);

      return ApiResponse<CourseResDto?>.SuccessResponse(courseRes);
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

    public async Task<ApiResponse<CourseResDto?>> RestoreCourseAsync(int id)
    {
      var course = await this._courseRepository.GetByIdAsync(id);
      if(course == null)
      {
        return ApiResponse<CourseResDto?>.ErrorResponse($"Not found course {id}.");
      }
      course.IsDeleted = false;
      _courseRepository.UpdateAsync(course);
      await _courseRepository.SaveChangeAsync();
      CourseResDto courseRes = _mapper.Map<CourseResDto>(course);
      return ApiResponse<CourseResDto?>.SuccessResponse(courseRes, $"Restore course {id} success");
    }

    public async Task<ApiResponse<CourseResDto?>> UpdateCourseAsync(int id, CourseReqDto courseReq)
    {
      var course = await this._courseRepository.GetByIdAsync(id);
      if(course == null)
      {
        return ApiResponse<CourseResDto?>.ErrorResponse($"Not found course {id}.");
      }

      var exists = await this._courseRepository.IsCourseNameExistsAsync(course.CourseName, course.Id);
    
      if (exists != null)
      {
          return ApiResponse<CourseResDto?>.ErrorResponse(
            $"Course name '{courseReq.CourseName}' exist",
            new Dictionary<String, String[]> { {"CourseName", new String[] {"Tên khóa học đã tồn tại"}} },
            409
          );
      }
      
      var spec = await this._specializationRepository.GetByIdAsync(courseReq.SpecializationId);
      if (spec == null)
      {
          return ApiResponse<CourseResDto?>.ErrorResponse(
            $"Chuyên ngành ID {courseReq.SpecializationId} không tồn tại",
            new Dictionary<String, String[]> { {"SpecializationId", new string[] {"Chuyên ngành không tồn tại"}} } 
          );
      }
      
      course.CourseName = courseReq.CourseName;
      course.Desct = courseReq.Desct;
      course.TuitionFee = courseReq.TuitionFee;
      course.Level = courseReq.Level;
      course.Lesson = courseReq.Lesson;
      course.IsActive = courseReq.IsActive;
      course.SpecializationId = courseReq.SpecializationId;
      course.UpdatedAt = DateTime.Now;
      
      this._courseRepository.UpdateAsync(course);
      await this._courseRepository.SaveChangeAsync();
      
      var courseRes = this._mapper.Map<CourseResDto>(course);
      courseRes.SpecializationId = spec.Id;
      
      return ApiResponse<CourseResDto?>.SuccessResponse(courseRes, "Cập nhật khóa học thành công");
    }

    public async Task<ApiResponse<IEnumerable<CourseResDto>>> GetCoursesBySpecializationAsync(int idSpecialization)
    {
      var courses = _courseRepository.GetCoursesBySpecializationAsync(idSpecialization);

      var courseRes = _mapper.Map<IEnumerable<CourseResDto>>(courses);
      return ApiResponse<IEnumerable<CourseResDto>>.SuccessResponse(courseRes);
    }
  }
}
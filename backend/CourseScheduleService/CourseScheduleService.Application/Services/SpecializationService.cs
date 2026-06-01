using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.SpecializationDtos;
using CourseScheduleService.Domain.Entities;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.interfaces.services;

namespace CourseScheduleService.Application.Services
{
  public class SpecializationService : ISpecializationService
  {
    private readonly ISpecializationRepository _specializationRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IMapper _mapper;

    public SpecializationService(ISpecializationRepository specializationRepository, ICourseRepository courseRepository, IMapper mapper)
    {
        this._specializationRepository = specializationRepository;
        this._mapper = mapper;
        _courseRepository = courseRepository;
    }

    public async Task<ApiResponse<SpecializationResDto?>> CreateSpecializationAsync(SpecializationReqDto specializationReq)
    {
      var specialization = await this._specializationRepository.IsSpecializationNameExistsAsync(specializationReq.SpecializationName);
      if(specialization != null)
      {
        return ApiResponse<SpecializationResDto?>.ErrorResponse(
          "Tên chuyên môn đã tồn tại",
          new Dictionary<String, String[]>
          {
            {"SpecializationName", new String[] {"Tên chuyên môn đã tồn tại"}}
          }
        );
      }

      Specialization newSpecialization = this._mapper.Map<Specialization>(specializationReq);

      await this._specializationRepository.AddAsync(newSpecialization);
      await this._specializationRepository.SaveChangeAsync();

      SpecializationResDto specializationResDto = _mapper.Map<SpecializationResDto>(newSpecialization);

      return ApiResponse<SpecializationResDto?>.SuccessResponse(specializationResDto);
    }

    public async Task<ApiResponse<bool>> DeteleSpecializationAsync(int id)
    {
      var specialization = await _specializationRepository.GetByIdAsync(id);

      if (specialization == null)
      {
        return ApiResponse<bool>.ErrorResponse($"Not found specialization {id}");
      }

      var courses = await _courseRepository.GetCoursesBySpecializationAsync(id);

      foreach (var course in courses)
      {
          course.SpecializationId = null;
          course.UpdatedAt = DateTime.Now;
          _courseRepository.UpdateAsync(course);
      }

      _specializationRepository.DeleteAsync(specialization);

      await _specializationRepository.SaveChangeAsync();

      return ApiResponse<bool>.SuccessResponse(true, 
        $"Đã xóa chuyên môn ID {id} và cập nhật {courses.Count()} khóa học liên quan.");
    }

    public async Task<ApiResponse<IEnumerable<SpecializationResDto>>> GetAllSpecializationAsync()
    {
      var specializations = await _specializationRepository.GetAllAsync();
      var specializationsRes = _mapper.Map<IEnumerable<SpecializationResDto>>(specializations);
      return ApiResponse<IEnumerable<SpecializationResDto>>.SuccessResponse(specializationsRes);
    }

    public async Task<ApiResponse<SpecializationResDto?>> GetOneByIdAsync(int id)
    {
      var specialization = await this._specializationRepository.GetByIdAsync(id);
      if(specialization == null)
      {
        return ApiResponse<SpecializationResDto?>.ErrorResponse($"Not found specialization ${id}.");
      }
      SpecializationResDto specializationResDto = this._mapper.Map<SpecializationResDto>(specialization);
      return ApiResponse<SpecializationResDto?>.SuccessResponse(specializationResDto);
    }

    public async Task<ApiResponse<SpecializationResDto?>> UpdateSpecializationAsync(int id, SpecializationReqDto specializationReq)
    {
      var specialization = await this._specializationRepository.GetByIdAsync(id);
      if(specialization == null)
      {
        return ApiResponse<SpecializationResDto?>.ErrorResponse($"Not found specialization ${id}.");
      }
      specialization.SpecializationName = specializationReq.SpecializationName;
      specialization.Descrt = specializationReq.Descrt;
      specialization.IsActive = specializationReq.IsActive;
      specialization.UpdatedAt = DateTime.Now;

      _specializationRepository.UpdateAsync(specialization);
      await _specializationRepository.SaveChangeAsync();
      SpecializationResDto specializationResDto = this._mapper.Map<SpecializationResDto>(specialization);
      return ApiResponse<SpecializationResDto?>.SuccessResponse(specializationResDto, "Update specialization success");
    }
  }
}
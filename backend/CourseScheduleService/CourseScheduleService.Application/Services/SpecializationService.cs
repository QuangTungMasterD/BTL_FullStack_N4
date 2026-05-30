using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IRepository<Specialization> _specializationRepository;
    private readonly IMapper _mapper;

    public SpecializationService(IRepository<Specialization> specializationRepository, IMapper mapper)
    {
        this._specializationRepository = specializationRepository;
        this._mapper = mapper;
    }

    public Task<ApiResponse<SpecializationResDto>> CreateSpecializationAsync(SpecializationReqDto specializationReq)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeteleSpecializationAsync(int id)
    {
      throw new NotImplementedException();
    }

    public Task<ApiResponse<IEnumerable<SpecializationResDto>>> GetAllSpecializationAsync()
    {
      throw new NotImplementedException();
    }

    public async Task<ApiResponse<SpecializationResDto?>> GetOneByIdAsync(int id)
    {
      var specialization = await this._specializationRepository.GetByIdAsync(id);
      if(specialization == null)
      {
        return ApiResponse<SpecializationResDto?>.ErrorResponse($"Not found specialization ${id}.");
      }
      throw new Exception();
    }

    public Task<ApiResponse<SpecializationResDto?>> UpdateSpecializationAsync(int id, SpecializationReqDto specializationReq)
    {
      throw new NotImplementedException();
    }
  }
}
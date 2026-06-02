using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.SpecializationDtos;
using CourseScheduleService.interfaces.services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
  [ApiController]
  [Route("api/[controller]s")]
  public class SpecializationController : ControllerBase
  {
    private readonly ISpecializationService _specializationService;

    public SpecializationController(ISpecializationService specializationService)
    {
      _specializationService = specializationService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SpecializationResDto?>>> CreateCourse([FromBody] SpecializationReqDto specializationReqDto)
    {
      var result = await _specializationService.CreateSpecializationAsync(specializationReqDto);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SpecializationResDto>>>> GetAllSpecialization()
    {
      var result = await _specializationService.GetAllSpecializationAsync();
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponse<SpecializationResDto>>>> GetPagedSpecializations(
    [FromQuery] SpecializationFilterRequest req)
    {
      var result = await _specializationService.GetPagedSpecializationsAsync(req);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<SpecializationResDto>>> GetSpecializationById([FromRoute] int id)
    {
      var result = await _specializationService.GetOneByIdAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}/permanent")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSpecialization([FromRoute] int id)
    {
      var result = await this._specializationService.DeteleSpecializationAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<SpecializationResDto?>>> UpdateSpecialization([FromRoute] int id, [FromBody] SpecializationReqDto specializationReqDto)
    {
      var result = await this._specializationService.UpdateSpecializationAsync(id, specializationReqDto);
      return StatusCode(result.StatusCode, result);
    }
  }
}
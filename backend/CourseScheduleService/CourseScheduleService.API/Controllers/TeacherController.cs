using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
  [ApiController]
  [Route("api/[controller]s")]
  public class TeacherController : ControllerBase
  {
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
      _teacherService = teacherService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> GetOneById([FromRoute] int id)
    {
      var result = await _teacherService.GetOneByIdAsync(id);
      if (result.Success)
      {
          return StatusCode(200, result);
      }
      return StatusCode(404, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeacherResDto>>>> GetAllTeacher()
    {
      var result = await _teacherService.GetAllTeacherAsync();
      return StatusCode(200, result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> CreateTeacher([FromBody] TeacherReqDto teacherReqDto)
    {
      var result = await _teacherService.CreateTeacherAsync(teacherReqDto);
      if (result.Success)
      {
        return StatusCode(201, result);
      }
      return StatusCode(409, result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTeacher([FromRoute] int id)
    {
      var result = await _teacherService.DeteleTeacherAsync(id);
      if (result.Success)
      {
        return StatusCode(200, result);
      }
      return StatusCode(404, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> UpdateTeacher([FromRoute] int id, [FromBody] TeacherReqDto teacherReqDto)
    {
      var result = await _teacherService.UpdateTeacherAsync(id, teacherReqDto);
      if (result.Success)
      {
        return StatusCode(200, result);
      }
      return StatusCode(404, result);
    }
  }
}
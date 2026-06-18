using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassDtos;
using CourseScheduleService.Application.DTOs.TeacherDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
  [ApiController]
  [Route("api/v1/teachers")]
  public class TeacherController : ControllerBase
  {
    private readonly ITeacherService _teacherService;

    public TeacherController(ITeacherService teacherService)
    {
      _teacherService = teacherService;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponse<TeacherResDto>>>> GetPagedTeachers(
    [FromQuery] TeacherFilterRequest req)
    {
      var result = await _teacherService.GetPagedTeachersAsync(req);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}/classes")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClassResDto>>>> GetClassesByTeacher(int id)
    {
        var result = await _teacherService.GetClassesByTeacherAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> GetOneById([FromRoute] int id)
    {
      var result = await _teacherService.GetOneByIdAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TeacherResDto>>>> GetAllTeacher()
    {
      var result = await _teacherService.GetAllTeacherAsync();
      return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> CreateTeacher([FromBody] TeacherReqDto teacherReqDto)
    {
      var result = await _teacherService.CreateTeacherAsync(teacherReqDto);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTeacher([FromRoute] int id)
    {
      var result = await _teacherService.DeteleTeacherAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}/permanent")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> HardDeleteCourse([FromRoute] int id)
    {
      var result = await this._teacherService.HardDeleteTeacherAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<TeacherReqDto?>>> RestoreCourseAsync(int id)
    {
      var result = await _teacherService.RestoreTeacherAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<TeacherResDto?>>> UpdateTeacher([FromRoute] int id, [FromBody] TeacherReqDto teacherReqDto)
    {
      var result = await _teacherService.UpdateTeacherAsync(id, teacherReqDto);
      return StatusCode(result.StatusCode, result);
    }
  }
}
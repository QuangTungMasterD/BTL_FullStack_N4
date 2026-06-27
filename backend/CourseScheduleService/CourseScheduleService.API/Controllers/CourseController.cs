using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;
using CourseScheduleService.interfaces.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CourseScheduleService.API.Controllers
{
  [ApiController]
  [Route("api/v1/courses")]
  public class CourseController : ControllerBase
  {
    private readonly ICourseService _courseService;

    [HttpPost("{id}/upload-image")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(int id, IFormFile file)
    {
        var result = await _courseService.UploadImageAsync(id, file);
        return StatusCode(result.StatusCode, result);
    }

    public CourseController(ICourseService courseService)
    {
      this._courseService = courseService;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponse<CourseResDto>>>> GetPagedCourses(
        [FromQuery] CourseFilterRequest req)
    {
      var result = await _courseService.GetPagedCoursesAsync(req);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CourseResDto?>>> GetOneById([FromRoute] int id)
    {
      var result = await this._courseService.GetOneByIdAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CourseResDto>>>> GetAllCourse()
    {
      var result = await _courseService.GetAllCourseAsync();
      return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<CourseResDto?>>> CreateCourse([FromBody] CourseReqDto courseReq)
    {
      var result = await _courseService.CreateCourseAsync(courseReq);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse([FromRoute] int id)
    {
      var result = await this._courseService.DeteleCourseAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}/permanent")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<bool>>> HardDeleteCourse([FromRoute] int id)
    {
      var result = await this._courseService.HardDeleteCourseAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<CourseResDto?>>> RestoreCourseAsync(int id)
    {
      var result = await _courseService.RestoreCourseAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<ApiResponse<CourseResDto>>> UpdateCourse([FromRoute] int id, [FromBody] CourseReqDto courseReqDto)
    {
      var result = await _courseService.UpdateCourseAsync(id, courseReqDto);
      return StatusCode(result.StatusCode, result);
    }

    
  }
}
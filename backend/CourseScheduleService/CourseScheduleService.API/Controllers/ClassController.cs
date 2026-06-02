using System.Collections.Generic;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
  [ApiController]
  [Route("api/[controller]es")]
  public class ClassController : ControllerBase
  {
    private readonly IClassService _classService;

    public ClassController(IClassService classService)
    {
      _classService = classService;
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponse<ClassResDto>>>> GetPagedClasses(
    [FromQuery] ClassFilterRequest req)
    {
      var result = await _classService.GetPagedClassesAsync(req);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClassResDto?>>> GetOneById([FromRoute] int id)
    {
      var result = await _classService.GetOneByIdAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClassResDto>>>> GetAllClass()
    {
      var result = await _classService.GetAllClassAsync();
      return StatusCode(result.StatusCode, result);
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ClassResDto>>>> GetClassesByCourse([FromRoute] int courseId)
    {
      var result = await _classService.GetClassesByCourseAsync(courseId);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClassResDto?>>> CreateClass([FromBody] ClassReqDto classReqDto)
    {
      var result = await _classService.CreateClassAsync(classReqDto);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClassResDto?>>> UpdateClass([FromRoute] int id, [FromBody] ClassReqDto classReqDto)
    {
      var result = await _classService.UpdateClassAsync(id, classReqDto);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteClass([FromRoute] int id)
    {
      var result = await _classService.DeleteClassAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpDelete("{id}/permanent")]
    public async Task<ActionResult<ApiResponse<bool>>> HardDeleteClass([FromRoute] int id)
    {
      var result = await _classService.HardDeleteClassAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    [HttpPatch("{id}/restore")]
    public async Task<ActionResult<ApiResponse<ClassResDto?>>> RestoreClass([FromRoute] int id)
    {
      var result = await _classService.RestoreClassAsync(id);
      return StatusCode(result.StatusCode, result);
    }

    
  }
}
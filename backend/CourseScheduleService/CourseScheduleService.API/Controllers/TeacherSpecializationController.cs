using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherSpecializationDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/v1/teacherspecializations")]
    public class TeacherSpecializationController : ControllerBase
    {
        private readonly ITeacherSpecializationService _tsService;

        public TeacherSpecializationController(ITeacherSpecializationService tsService)
        {
            _tsService = tsService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeacherSpecializationResDto>>>> GetAll()
        {
            var result = await _tsService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<TeacherSpecializationResDto?>>> Create(
            [FromBody] TeacherSpecializationReqDto reqDto)
        {
            var result = await _tsService.CreateAsync(reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var result = await _tsService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
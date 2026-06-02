using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherSpecializationDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
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
        public async Task<ActionResult<ApiResponse<TeacherSpecializationResDto?>>> Create(
            [FromBody] TeacherSpecializationReqDto reqDto)
        {
            var result = await _tsService.CreateAsync(reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var result = await _tsService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
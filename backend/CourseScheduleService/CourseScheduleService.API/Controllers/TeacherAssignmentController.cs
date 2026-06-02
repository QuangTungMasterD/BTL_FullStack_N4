using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.TeacherAssignmentDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class TeacherAssignmentController : ControllerBase
    {
        private readonly ITeacherAssignmentService _assignmentService;

        public TeacherAssignmentController(ITeacherAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeacherAssignmentResDto>>>> GetAll()
        {
            var result = await _assignmentService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TeacherAssignmentResDto?>>> Create(
            [FromBody] TeacherAssignmentReqDto reqDto)
        {
            var result = await _assignmentService.CreateAsync(reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var result = await _assignmentService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
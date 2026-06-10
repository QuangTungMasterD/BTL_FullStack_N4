using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.ClassSessionDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/v1/classsessions")]
    public class ClassSessionController : ControllerBase
    {
        private readonly IClassSessionService _sessionService;

        public ClassSessionController(IClassSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClassSessionResDto>>>> GetAll()
        {
            var result = await _sessionService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClassSessionResDto?>>> GetOneById([FromRoute] int id)
        {
            var result = await _sessionService.GetOneByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<PagedResponse<ClassSessionResDto>>>> GetPaged(
            [FromQuery] ClassSessionFilterRequest req)
        {
            var result = await _sessionService.GetPagedSessionsAsync(req);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClassSessionResDto?>>> Create(
            [FromBody] ClassSessionReqDto reqDto)
        {
            var result = await _sessionService.CreateAsync(reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ClassSessionResDto?>>> Update(
            [FromRoute] int id, [FromBody] ClassSessionReqDto reqDto)
        {
            var result = await _sessionService.UpdateAsync(id, reqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<ClassSessionResDto?>>> UpdateStatus(
            [FromRoute] int id, [FromBody] int status)
        {
            var result = await _sessionService.UpdateStatusAsync(id, status);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
        {
            var result = await _sessionService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
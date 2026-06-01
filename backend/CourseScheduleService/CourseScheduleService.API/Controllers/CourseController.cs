using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.CourseDtos;
using CourseScheduleService.interfaces.services;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            this._courseService = courseService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResDto?>>> GetOneById([FromRoute] int id)
        {
            var result = await this._courseService.GetOneByIdAsync(id);
            if(result.Success == true)
            {
                return StatusCode(200, result);
            }
            return StatusCode(404, result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseResDto>>>> GetAllCourse()
        {
            var result = await _courseService.GetAllCourseAsync();
            return StatusCode(200, result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CourseResDto?>>> CreateCourse([FromBody]CourseReqDto courseReq)
        {
            var result = await _courseService.CreateCourseAsync(courseReq);
            if(result.Success)
            {
                return StatusCode(201, result);
            }
            return StatusCode(409, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCourse([FromRoute] int id)
        {
            var result = await this._courseService.DeteleCourseAsync(id);
            if(result.Success == true)
            {
                return StatusCode(200, result);
            }
            return StatusCode(404, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CourseResDto>>> UpdateCourse([FromRoute] int id, [FromBody] CourseReqDto courseReqDto)
        {
            var result = await _courseService.UpdateCourseAsync(id, courseReqDto);
            if(result.Success == true)
            {
                return StatusCode(200, result);
            }
            return StatusCode(404, result);
        }
    }
}
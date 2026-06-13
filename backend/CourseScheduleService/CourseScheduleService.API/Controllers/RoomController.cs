using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseScheduleService.Application.Common;
using CourseScheduleService.Application.DTOs.RoomDtos;
using CourseScheduleService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseScheduleService.API.Controllers
{
    [ApiController]
    [Route("api/v1/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoomResDto>>>> GetAllRoom()
        {
            var result = await _roomService.GetAllRoomAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<ApiResponse<PagedResponse<RoomResDto>>>> GetPagedRooms(
            [FromQuery] RoomFilterRequest req)
        {
            var result = await _roomService.GetPagedRoomsAsync(req);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RoomResDto?>>> GetOneById([FromRoute] int id)
        {
            var result = await _roomService.GetOneByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<RoomResDto?>>> CreateRoom([FromBody] RoomReqDto roomReqDto)
        {
            var result = await _roomService.CreateRoomAsync(roomReqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<RoomResDto?>>> UpdateRoom(
            [FromRoute] int id, [FromBody] RoomReqDto roomReqDto)
        {
            var result = await _roomService.UpdateRoomAsync(id, roomReqDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRoom([FromRoute] int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<bool>>> HardDeleteRoom([FromRoute] int id)
        {
            var result = await _roomService.HardDeleteRoomAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ApiResponse<RoomResDto?>>> RestoreRoom([FromRoute] int id)
        {
            var result = await _roomService.RestoreRoomAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
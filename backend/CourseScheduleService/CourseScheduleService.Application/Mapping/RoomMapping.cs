using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.RoomDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class RoomMapping : Profile
    {
        public RoomMapping()
        {
            CreateMap<Room, RoomResDto>();
            CreateMap<RoomReqDto, Room>();
        }
    }
}
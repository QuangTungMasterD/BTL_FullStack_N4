using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.ScheduleChangeRequestDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class ScheduleChangeRequestMapping : Profile
    {
        public ScheduleChangeRequestMapping()
        {
            CreateMap<ScheduleChangeRequest, ScheduleChangeRequestResDto>();
            CreateMap<ScheduleChangeRequestReqDto, ScheduleChangeRequest>();
        }
    }
}
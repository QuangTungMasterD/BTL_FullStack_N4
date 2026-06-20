using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.SpecializationDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class SpecializationMapping : Profile
    {
        public SpecializationMapping()
        {
            CreateMap<Specialization, SpecializationResDto>();
            CreateMap<SpecializationReqDto, Specialization>();
        }
    }
}
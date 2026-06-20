using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseScheduleService.Application.DTOs.ClassDtos;
using CourseScheduleService.Domain.Entities;

namespace CourseScheduleService.Application.Mapping
{
    public class ClassMapping : Profile
    {
        public ClassMapping()
        {
            CreateMap<Class, ClassResDto>();
            CreateMap<ClassReqDto, Class>();
        }
    }
}